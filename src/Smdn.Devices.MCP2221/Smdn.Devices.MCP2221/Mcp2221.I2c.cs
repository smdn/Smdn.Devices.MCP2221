// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT

#pragma warning disable CA1848, CA1873, CA2254

using System;
using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

namespace Smdn.Devices.MCP2221;

#pragma warning disable IDE0040
partial class Mcp2221 {
#pragma warning restore IDE0040
  public I2cFunctionality I2c { get; }

#pragma warning disable CA1034
  public sealed partial class I2cFunctionality {
#pragma warning restore CA1034
    public const int MaxBlockLength = 0xFFFF;
    private const int MaxTransferLengthPerCommand = 64 - 4;

    private readonly Mcp2221 device;

    public I2cBusSpeed BusSpeed { get; set; } = I2cBusSpeed.Default;

    internal I2cFunctionality(Mcp2221 device)
    {
      this.device = device;
    }

    private static readonly EventId EventIdI2cCommand = new(10, "I2C command");
    private static readonly EventId EventIdI2cEngineState = new(11, "I2C engine state");

    private static class CancelTransferCommand {
      [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1316:TupleElementNamesShouldUseCorrectCasing", Justification = "Not a publicly-exposed type or member.")]
#pragma warning disable IDE0060 // [IDE0060] Remove unused parameter
      public static void ConstructCommand(Span<byte> comm, ReadOnlySpan<byte> userData, (I2cAddress address, Exception exceptionCauseOfCancellation) args)
#pragma warning restore IDE0060
      {
        // [MCP2221A] 3.1.1 STATUS/SET PARAMETERS
        comm[0] = 0x10; // Status/Set Parameters
        comm[1] = 0x00; // Don't care
        comm[2] = 0x10; // Cancel current I2C/SMBus transfer
      }

      [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1316:TupleElementNamesShouldUseCorrectCasing", Justification = "Not a publicly-exposed type or member.")]
      public static I2cEngineState ParseResponse(ReadOnlySpan<byte> resp, (I2cAddress address, Exception exceptionCauseOfCancellation) args)
      {
        if (resp[1] != 0x00) // Command completed successfully
          throw new CommandException($"unexpected response (0x{resp[1]:X2})", args.exceptionCauseOfCancellation);

        var state = I2cEngineState.Parse(resp);
        var isBusStatusDefined =
#if SYSTEM_ENUM_ISDEFINED_OF_TENUM
          Enum.IsDefined<I2cEngineState.TransferStatus>(state.BusStatus);
#else
          Enum.IsDefined(typeof(I2cEngineState.TransferStatus), state.BusStatus);
#endif

        if (!isBusStatusDefined) {
          throw new I2cCommandException(
            args.address,
            $"unexpected response while transfer cancellation (0x{resp[2]:X2})",
            args.exceptionCauseOfCancellation
          );
        }

        return state;
      }
    }

    private static async ValueTask CancelAsync(Mcp2221 device, I2cAddress address, Exception exceptionCauseOfCancellation)
    {
      var engineState = await device.CommandAsync(
        userData: default,
        arg: (address, exceptionCauseOfCancellation),
        cancellationToken: default,
        constructCommand: CancelTransferCommand.ConstructCommand,
        parseResponse: CancelTransferCommand.ParseResponse
      ).ConfigureAwait(false);

      device.logger?.LogWarning(EventIdI2cEngineState, $"CANCEL TRANSFER: {engineState}");
    }

    private static void Cancel(Mcp2221 device, I2cAddress address, Exception exceptionCauseOfCancellation)
    {
      var engineState = device.Command(
        userData: default,
        arg: (address, exceptionCauseOfCancellation),
        cancellationToken: default,
        constructCommand: CancelTransferCommand.ConstructCommand,
        parseResponse: CancelTransferCommand.ParseResponse
      );

      device.logger?.LogWarning(EventIdI2cEngineState, $"CANCEL TRANSFER: {engineState}");
    }

    public ValueTask WriteAsync(
      I2cAddress address,
      byte[] buffer,
      int offset,
      int count,
      CancellationToken cancellationToken = default
    )
      => WriteAsync(
        address,
        (buffer ?? throw new ArgumentNullException(nameof(buffer))).AsMemory(offset, count),
        cancellationToken
      );

    /// <remarks>An empty buffer can be specified to <paramref name="buffer"/>. This method issues writing command with 0-length in this case.</remarks>
    public async ValueTask WriteAsync(
      I2cAddress address,
      ReadOnlyMemory<byte> buffer,
      CancellationToken cancellationToken = default
    )
    {
      if (MaxBlockLength < buffer.Length)
        throw new ArgumentException($"transfer length must be up to {MaxBlockLength} bytes", nameof(buffer));

      try {
        device.logger?.LogInformation(EventIdI2cCommand, $"I2C Write {buffer.Length} bytes to 0x{address}");

        for (; ; ) {
          var lengthToTransfer = Math.Min(buffer.Length, MaxTransferLengthPerCommand);

          foreach (var (constructCommand, parseResponse) in new OperationContext(device.logger, BusSpeed).IterateWriteCommands()) {
            await device.CommandAsync(
              userData: buffer.Slice(0, lengthToTransfer),
              arg: (address, Memory<byte>.Empty),
              cancellationToken: cancellationToken,
              constructCommand: constructCommand,
              parseResponse: parseResponse
            ).ConfigureAwait(false);
          }

          buffer = buffer.Slice(lengthToTransfer);

          if (buffer.IsEmpty)
            break;
        }
      }
      catch (Exception ex) {
        device.logger?.LogError(EventIdI2cCommand, $"I2C Write to 0x{address} failed: {ex.Message}");
        if (ex is not I2cNackException)
          await CancelAsync(device, address, ex).ConfigureAwait(false);
        throw;
      }
    }

    public void Write(
      I2cAddress address,
      byte[] buffer,
      int offset,
      int count,
      CancellationToken cancellationToken = default
    )
      => Write(
        address,
        (buffer ?? throw new ArgumentNullException(nameof(buffer))).AsSpan(offset, count),
        cancellationToken
      );

    /// <remarks>An empty buffer can be specified to <paramref name="buffer"/>. This method issues writing command with 0-length in this case.</remarks>
    public void Write(
      I2cAddress address,
      ReadOnlySpan<byte> buffer,
      CancellationToken cancellationToken = default
    )
    {
      if (MaxBlockLength < buffer.Length)
        throw new ArgumentException($"transfer length must be up to {MaxBlockLength} bytes", nameof(buffer));

      try {
        device.logger?.LogInformation(EventIdI2cCommand, $"I2C Write {buffer.Length} bytes to 0x{address}");

        for (; ; ) {
          var lengthToTransfer = Math.Min(buffer.Length, MaxTransferLengthPerCommand);

          foreach (var (constructCommand, parseResponse) in new OperationContext(device.logger, BusSpeed).IterateWriteCommands()) {
            device.Command(
              userData: buffer.Slice(0, lengthToTransfer),
              arg: (address, Memory<byte>.Empty),
              cancellationToken: cancellationToken,
              constructCommand: constructCommand,
              parseResponse: parseResponse
            );
          }

          buffer = buffer.Slice(lengthToTransfer);

          if (buffer.IsEmpty)
            break;
        }
      }
      catch (Exception ex) {
        device.logger?.LogError(EventIdI2cCommand, $"I2C Write to 0x{address} failed: {ex.Message}");
        if (ex is not I2cNackException)
          Cancel(device, address, ex);
        throw;
      }
    }

    public ValueTask<int> ReadAsync(
      I2cAddress address,
      byte[] buffer,
      int offset,
      int count,
      CancellationToken cancellationToken = default
    )
      => ReadAsync(
        address,
        (buffer ?? throw new ArgumentNullException(nameof(buffer))).AsMemory(offset, count),
        cancellationToken
      );

    /// <remarks>An empty buffer can be specified to <paramref name="buffer"/>. This method issues reading command with 0-length in this case.</remarks>
    public async ValueTask<int> ReadAsync(
      I2cAddress address,
      Memory<byte> buffer,
      CancellationToken cancellationToken = default
    )
    {
      if (MaxBlockLength < buffer.Length)
        throw new ArgumentException($"transfer length must be up to {MaxBlockLength} bytes", nameof(buffer));

      try {
        device.logger?.LogInformation(EventIdI2cCommand, $"I2C Read {buffer.Length} bytes from 0x{address}");

        var readBuffer = ArrayPool<byte>.Shared.Rent(MaxTransferLengthPerCommand);

        try {
          var totalReadLength = 0;

          for (; ; ) {
            var lengthToTransfer = Math.Min(buffer.Length, MaxTransferLengthPerCommand);
            var readBufferMemory = readBuffer.AsMemory(0, lengthToTransfer);

            var context = new OperationContext(device.logger, BusSpeed);

            foreach (var (constructCommand, parseResponse) in context.IterateReadCommands()) {
              await device.CommandAsync(
                userData: buffer.Slice(0, lengthToTransfer),
                arg: (address, readBufferMemory),
                cancellationToken: cancellationToken,
                constructCommand: constructCommand,
                parseResponse: parseResponse
              ).ConfigureAwait(false);
            }

            if (context.ReadLength < 0)
              break;

            readBufferMemory.Slice(0, context.ReadLength).CopyTo(buffer);

            buffer = buffer.Slice(context.ReadLength);

            if (context.ReadLength < lengthToTransfer)
              break;
            if (buffer.IsEmpty)
              break;
          }

          return totalReadLength;
        }
        finally {
          ArrayPool<byte>.Shared.Return(readBuffer);
        }
      }
      catch (Exception ex) {
        device.logger?.LogError(EventIdI2cCommand, $"I2C Read from 0x{address} failed: {ex.Message}");
        if (ex is not I2cReadException)
          await CancelAsync(device, address, ex).ConfigureAwait(false);
        throw;
      }
    }

    public int Read(
      I2cAddress address,
      byte[] buffer,
      int offset,
      int count,
      CancellationToken cancellationToken = default
    )
      => Read(
        address,
        (buffer ?? throw new ArgumentNullException(nameof(buffer))).AsSpan(offset, count),
        cancellationToken
      );

    /// <remarks>An empty buffer can be specified to <paramref name="buffer"/>. This method issues reading command with 0-length in this case.</remarks>
    public int Read(
      I2cAddress address,
      Span<byte> buffer,
      CancellationToken cancellationToken = default
    )
    {
      if (MaxBlockLength < buffer.Length)
        throw new ArgumentException($"transfer length must be up to {MaxBlockLength} bytes", nameof(buffer));

      try {
        device.logger?.LogInformation(EventIdI2cCommand, $"I2C Read {buffer.Length} bytes from 0x{address}");

        var readBuffer = ArrayPool<byte>.Shared.Rent(MaxTransferLengthPerCommand);

        try {
          var totalReadLength = 0;

          for (; ; ) {
            var lengthToTransfer = Math.Min(buffer.Length, MaxTransferLengthPerCommand);
            var readBufferMemory = readBuffer.AsMemory(0, lengthToTransfer);

            var context = new OperationContext(device.logger, BusSpeed);

            foreach (var (constructCommand, parseResponse) in context.IterateReadCommands()) {
              device.Command(
                userData: buffer.Slice(0, lengthToTransfer),
                arg: (address, readBufferMemory),
                cancellationToken: cancellationToken,
                constructCommand: constructCommand,
                parseResponse: parseResponse
              );
            }

            if (context.ReadLength < 0)
              break;

            readBufferMemory.Span.Slice(0, context.ReadLength).CopyTo(buffer);

            buffer = buffer.Slice(context.ReadLength);

            if (context.ReadLength < lengthToTransfer)
              break;
            if (buffer.IsEmpty)
              break;
          }

          return totalReadLength;
        }
        finally {
          ArrayPool<byte>.Shared.Return(readBuffer);
        }
      }
      catch (Exception ex) {
        device.logger?.LogError(EventIdI2cCommand, $"I2C Read from 0x{address} failed: {ex.Message}");
        if (ex is not I2cReadException)
          Cancel(device, address, ex);
        throw;
      }
    }

    public async ValueTask WriteByteAsync(
      I2cAddress address,
      byte value,
      CancellationToken cancellationToken = default
    )
    {
      var buffer = ArrayPool<byte>.Shared.Rent(1);

      try {
        buffer[0] = value;

        await WriteAsync(address, buffer.AsMemory(0, 1), cancellationToken).ConfigureAwait(false);
      }
      finally {
        ArrayPool<byte>.Shared.Return(buffer);
      }
    }

    public void WriteByte(
      I2cAddress address,
      byte value,
      CancellationToken cancellationToken = default
    )
      => Write(address, [value], cancellationToken);

    public async ValueTask<int> ReadByteAsync(
      I2cAddress address,
      CancellationToken cancellationToken = default
    )
    {
      var buffer = ArrayPool<byte>.Shared.Rent(1);

      try {
        if (0 == await ReadAsync(address, buffer.AsMemory(0, 1), cancellationToken).ConfigureAwait(false))
          return -1;
        else
          return buffer[0];
      }
      finally {
        ArrayPool<byte>.Shared.Return(buffer);
      }
    }

    public int ReadByte(
      I2cAddress address,
      CancellationToken cancellationToken = default
    )
    {
      Span<byte> buffer = stackalloc byte[1];

      if (0 == Read(address, buffer, cancellationToken))
        return -1;
      else
        return buffer[0];
    }
  }
}
