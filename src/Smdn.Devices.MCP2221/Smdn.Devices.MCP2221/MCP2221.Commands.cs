// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT

using System;
using System.Buffers;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

namespace Smdn.Devices.MCP2221 {
  partial class MCP2221 {
    internal delegate void ConstructCommandAction<TArg>(Span<byte> command, ReadOnlySpan<byte> userData, TArg arg);
    internal delegate TResponse ParseResponseFunc<TArg, TResponse>(ReadOnlySpan<byte> response, TArg arg);

    private const int commandLength = 64;
    private const int responseLength = 64;
    private const int commandReportLength = 1 + commandLength;
    private const int responseReportLength = 1 + responseLength;

    private static readonly EventId eventIdCommand = new EventId(1, "sent command");
    private static readonly EventId eventIdResponse = new EventId(2, "received response");

    private static string ConvertByteSequenceToString(ReadOnlySpan<byte> sequence)
    {
      var buffer = ArrayPool<byte>.Shared.Rent(sequence.Length);

      try {
        sequence.CopyTo(buffer);

        return BitConverter.ToString(buffer, 0, sequence.Length);
      }
      finally {
        ArrayPool<byte>.Shared.Return(buffer);
      }
    }

    internal async ValueTask<TResponse> CommandAsync<TArg, TResponse>(
      ReadOnlyMemory<byte> userData,
      TArg arg,
      CancellationToken cancellationToken,
      ConstructCommandAction<TArg> constructCommand,
      ParseResponseFunc<TArg, TResponse> parseResponse
    )
    {
      if (constructCommand is null)
        throw new ArgumentNullException(nameof(constructCommand));
      if (parseResponse is null)
        throw new ArgumentNullException(nameof(parseResponse));

      ThrowIfDisposed();

      var commandReport = ArrayPool<byte>.Shared.Rent(commandReportLength);
      var responseReport = ArrayPool<byte>.Shared.Rent(responseReportLength);

      try {
        var commandReportMemory = commandReport.AsMemory(0, commandReportLength);
        var responseReportMemory = responseReport.AsMemory(0, responseReportLength);

        //commandReportMemory[0] = 0x00; // report
        commandReportMemory.Span.Clear();

        cancellationToken.ThrowIfCancellationRequested();

        constructCommand(
          commandReportMemory.Span.Slice(1, commandLength),
          userData.Span,
          arg
        );

        logger?.LogTrace(eventIdCommand, "> {0}", ConvertByteSequenceToString(commandReportMemory.Span.Slice(1, commandLength)));

        try {
          await HidStream.WriteAsync(
            commandReportMemory.Slice(HidStream.RequiresPacketOnly ? 1 : 0)
          ).ConfigureAwait(false);
        }
        catch (Exception ex) {
          throw new CommandException("writing command report failed", ex);
        }

        try {
          await HidStream.ReadAsync(
            responseReportMemory.Slice(HidStream.RequiresPacketOnly ? 1 : 0)
          ).ConfigureAwait(false);
        }
        catch (Exception ex) {
          throw new CommandException("reading response report failed", ex);
        }

        logger?.LogTrace(eventIdResponse, "< {0}", ConvertByteSequenceToString(responseReportMemory.Span.Slice(1, responseLength)));

        if (commandReportMemory.Span[0] != responseReportMemory.Span[0])
          throw new CommandException($"unexpected command echo (command code: {commandReportMemory.Span[0]:X2}, command code echo: {responseReportMemory.Span[0]:X2})");

        return parseResponse(
          responseReportMemory.Span.Slice(1, responseLength),
          arg
        );
      }
      finally {
        ArrayPool<byte>.Shared.Return(commandReport);
        ArrayPool<byte>.Shared.Return(responseReport);
      }
    }

    internal unsafe TResponse Command<TArg, TResponse>(
      ReadOnlySpan<byte> userData,
      TArg arg,
      CancellationToken cancellationToken,
      ConstructCommandAction<TArg> constructCommand,
      ParseResponseFunc<TArg, TResponse> parseResponse
    )
    {
      if (constructCommand is null)
        throw new ArgumentNullException(nameof(constructCommand));
      if (parseResponse is null)
        throw new ArgumentNullException(nameof(parseResponse));

      ThrowIfDisposed();

      Span<byte> commandReport = stackalloc byte[commandReportLength];
      Span<byte> responseReport = stackalloc byte[responseReportLength];

      //commandReport[0] = 0x00; // report
      commandReport.Clear();

      cancellationToken.ThrowIfCancellationRequested();

      constructCommand(
        commandReport.Slice(1),
        userData,
        arg
      );

      logger?.LogTrace(eventIdCommand, "> {0}", ConvertByteSequenceToString(commandReport.Slice(1)));

      try {
        HidStream.Write(commandReport.Slice(HidStream.RequiresPacketOnly ? 1 : 0));
      }
      catch (Exception ex) {
        throw new CommandException("writing command report failed", ex);
      }

      try {
        HidStream.Read(responseReport.Slice(HidStream.RequiresPacketOnly ? 1 : 0));
      }
      catch (Exception ex) {
        throw new CommandException("reading response report failed", ex);
      }

      logger?.LogTrace(eventIdResponse, "< {0}", ConvertByteSequenceToString(responseReport.Slice(1)));

      if (commandReport[0] != responseReport[0])
        throw new CommandException($"unexpected command echo (command code: {commandReport[0]:X2}, command code echo: {responseReport[0]:X2})");

      return parseResponse(
        responseReport.Slice(1),
        arg
      );
    }

#if __FUTURE_VERSION
    public ValueTask ResetAsync() => ValueTask.FromException(new NotImplementedException());
#endif

    private static class RetrieveRevisionCommand {
      public static void ConstructCommand(Span<byte> comm, ReadOnlySpan<byte> userData, int _)
      {
        // [MCP2221A] 3.1.1 STATUS/SET PARAMETERS
        comm[0] = 0x10; // Status/Set Parameter
      }

      public static (
        string firmwareRevision,
        string hardwareRevision
      ) ParseResponse(ReadOnlySpan<byte> resp, int _)
      {
        static void CreateRevisionString(Span<char> str, (char major, char minor) revision)
        {
          str[0] = revision.major;
          str[1] = '.';
          str[2] = revision.minor;
        }

        return (
#if false // XXX: string.Create does not accept ReadOnlySpan<T>, dotnet/runtime#30175
          string.Create(3, resp, (str, re) => {str[0] = (char)re[46]; str[1] = '.'; str[2] = (char)re[47]; }),
          string.Create(3, resp, (str, re) => {str[0] = (char)re[48]; str[1] = '.'; str[2] = (char)re[49]; })
#endif
          string.Create(3, ((char)resp[46], (char)resp[47]), CreateRevisionString),
          string.Create(3, ((char)resp[48], (char)resp[49]), CreateRevisionString)
        );
      }
    }

    // [MCP2221A] 3.1.2 READ FLASH DATA
    private enum ReadFlashDataSubCode : byte {
      UsbDescriptorStringManufacturer = 0x02,
      UsbDescriptorStringProduct      = 0x03,
      UsbDescriptorStringSerialNumber = 0x04,
      ChipFactorySerialNumber         = 0x05
    }

    private static class RetrieveFlashStringCommand {
      public static void ConstructCommand(Span<byte> comm, ReadOnlySpan<byte> userData, ReadFlashDataSubCode subCode)
      {
        // [MCP2221A] 3.1.2 READ FLASH DATA
        comm[0] = 0xB0; // Read Flash Data

        // Read Flash Data Sub Code
        // 0x02: Read USB Manufacturer Descriptor String
        // 0x03: Read USB Product Descriptor String
        // 0x04: Read USB Manufacturer Descriptor String
        // 0x05: Read Chip Factory Serial Number
        comm[1] = (byte)subCode;
      }

      public static unsafe string ParseResponse(ReadOnlySpan<byte> resp, ReadFlashDataSubCode subCode)
      {
        if (subCode == ReadFlashDataSubCode.ChipFactorySerialNumber) {
#if false // XXX: string.Create does not accept ReadOnlySpan<T>, dotnet/runtime#30175
          return string.Create((int)resp[2], resp, (str, re) => {
            for (var i = 0; i < str.Length; i++) {
              str[i] = (char)re[i];
            }
          }
#endif
          var length = (int)resp[2];
          Span<char> serialNumberChars = stackalloc char[length];

          for (var i = 0; i < length; i++) {
            serialNumberChars[i] = (char)resp[4 + i];
          }

          return new string(serialNumberChars);
        }
        else {
          // 0x02: The number of bytes + 2 in the provided USB Manufacturer/Product/Serial Number Descriptor String.
          var lengthInBytes = (int)resp[2] - 2;
          var length = lengthInBytes / 2;

          Span<char> descriptorStringChars = stackalloc char[length];

          for (var i = 0; i < length; i++) {
            var lower  = resp[4 + 2 * i + 0];
            var higher = resp[4 + 2 * i + 1];

            descriptorStringChars[i] = (char)(lower | (higher << 8));
          }

          return new string(descriptorStringChars);
        }
      }
    }

    private async ValueTask RetrieveChipInformationAsync(
      Action<string> validateHardwareRevision,
      Action<string> validateFirmwareRevision
    )
    {
      (HardwareRevision, FirmwareRevision) = await CommandAsync(
        userData: default,
        arg: 0,
        cancellationToken: default,
        constructCommand: RetrieveRevisionCommand.ConstructCommand,
        parseResponse: RetrieveRevisionCommand.ParseResponse
      ).ConfigureAwait(false);

      validateHardwareRevision?.Invoke(HardwareRevision);
      validateFirmwareRevision?.Invoke(FirmwareRevision);

      ManufacturerDescriptor = await CommandAsync(
        userData: default,
        arg: ReadFlashDataSubCode.UsbDescriptorStringManufacturer,
        cancellationToken: default,
        constructCommand: RetrieveFlashStringCommand.ConstructCommand,
        parseResponse: RetrieveFlashStringCommand.ParseResponse
      ).ConfigureAwait(false);

      ProductDescriptor = await CommandAsync(
        userData: default,
        arg: ReadFlashDataSubCode.UsbDescriptorStringProduct,
        cancellationToken: default,
        constructCommand: RetrieveFlashStringCommand.ConstructCommand,
        parseResponse: RetrieveFlashStringCommand.ParseResponse
      ).ConfigureAwait(false);

      SerialNumberDescriptor = await CommandAsync(
        userData: default,
        arg: ReadFlashDataSubCode.UsbDescriptorStringSerialNumber,
        cancellationToken: default,
        constructCommand: RetrieveFlashStringCommand.ConstructCommand,
        parseResponse: RetrieveFlashStringCommand.ParseResponse
      ).ConfigureAwait(false);

      ChipFactorySerialNumber = await CommandAsync(
        userData: default,
        arg: ReadFlashDataSubCode.ChipFactorySerialNumber,
        cancellationToken: default,
        constructCommand: RetrieveFlashStringCommand.ConstructCommand,
        parseResponse: RetrieveFlashStringCommand.ParseResponse
      ).ConfigureAwait(false);
    }

    private void RetrieveChipInformation(
      Action<string> validateHardwareRevision,
      Action<string> validateFirmwareRevision
    )
    {
      (HardwareRevision, FirmwareRevision) = Command(
        userData: default,
        arg: 0,
        cancellationToken: default,
        constructCommand: RetrieveRevisionCommand.ConstructCommand,
        parseResponse: RetrieveRevisionCommand.ParseResponse
      );

      validateHardwareRevision?.Invoke(HardwareRevision);
      validateFirmwareRevision?.Invoke(FirmwareRevision);

      ManufacturerDescriptor = Command(
        userData: default,
        arg: ReadFlashDataSubCode.UsbDescriptorStringManufacturer,
        cancellationToken: default,
        constructCommand: RetrieveFlashStringCommand.ConstructCommand,
        parseResponse: RetrieveFlashStringCommand.ParseResponse
      );

      ProductDescriptor = Command(
        userData: default,
        arg: ReadFlashDataSubCode.UsbDescriptorStringProduct,
        cancellationToken: default,
        constructCommand: RetrieveFlashStringCommand.ConstructCommand,
        parseResponse: RetrieveFlashStringCommand.ParseResponse
      );

      SerialNumberDescriptor = Command(
        userData: default,
        arg: ReadFlashDataSubCode.UsbDescriptorStringSerialNumber,
        cancellationToken: default,
        constructCommand: RetrieveFlashStringCommand.ConstructCommand,
        parseResponse: RetrieveFlashStringCommand.ParseResponse
      );

      ChipFactorySerialNumber = Command(
        userData: default,
        arg: ReadFlashDataSubCode.ChipFactorySerialNumber,
        cancellationToken: default,
        constructCommand: RetrieveFlashStringCommand.ConstructCommand,
        parseResponse: RetrieveFlashStringCommand.ParseResponse
      );
    }
  }
}
