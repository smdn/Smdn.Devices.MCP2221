// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT

#pragma warning disable CA1848

using System;
using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

namespace Smdn.Devices.MCP2221;

#pragma warning disable IDE0040
partial class MCP2221 {
#pragma warning restore IDE0040
  public I2CFunctionality I2C { get; }

  public sealed partial class I2CFunctionality {
    public const int MaxBlockLength = 0xFFFF;
    private const int MaxTransferLengthPerCommand = 64 - 4;

    private readonly MCP2221 device;

    public I2CBusSpeed BusSpeed { get; set; } = I2CBusSpeed.Default;

    internal I2CFunctionality(MCP2221 device)
    {
      this.device = device;
    }

    private static readonly EventId eventIdI2CCommand = new(10, "I2C command");
    private static readonly EventId eventIdI2CEngineState = new(11, "I2C engine state");

    private static class CancelTransferCommand {
      [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1316:TupleElementNamesShouldUseCorrectCasing", Justification = "Not a publicly-exposed type or member.")]
#pragma warning disable IDE0060 // [IDE0060] Remove unused parameter
      public static void ConstructCommand(Span<byte> comm, ReadOnlySpan<byte> userData, (I2CAddress address, Exception exceptionCauseOfCancellation) args)
#pragma warning restore IDE0060
      {
        // [MCP2221A] 3.1.1 STATUS/SET PARAMATERS
        comm[0] = 0x10; // Status/Set Parameters
        comm[1] = 0x00; // Don't care
        comm[2] = 0x10; // Cancel current I2C/SMBus transfer
      }

      [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1316:TupleElementNamesShouldUseCorrectCasing", Justification = "Not a publicly-exposed type or member.")]
      public static I2CEngineState ParseResponse(ReadOnlySpan<byte> resp, (I2CAddress address, Exception exceptionCauseOfCancellation) args)
      {
        if (resp[1] != 0x00) // Command completed successfully
          throw new CommandException($"unexpected response (0x{resp[1]:X2})", args.exceptionCauseOfCancellation);

        var state = I2CEngineState.Parse(resp);
        var isBusStatusDefined =
#if NET5_0_OR_GREATER
          Enum.IsDefined<I2CEngineState.TransferStatus>(state.BusStatus);
#else
          Enum.IsDefined(typeof(I2CEngineState.TransferStatus), state.BusStatus);
#endif

        if (!isBusStatusDefined) {
          throw new I2CCommandException(
            args.address,
            $"unexpected response while transfer cancellation (0x{resp[2]:X2})",
            args.exceptionCauseOfCancellation
          );
        }

        return state;
      }
    }

    private static async ValueTask CancelAsync(MCP2221 device, I2CAddress address, Exception exceptionCauseOfCancellation)
    {
      var engineState = await device.CommandAsync(
        userData: default,
        arg: (address, exceptionCauseOfCancellation),
        cancellationToken: default,
        constructCommand: CancelTransferCommand.ConstructCommand,
        parseResponse: CancelTransferCommand.ParseResponse
      ).ConfigureAwait(false);

      device.logger?.LogWarning(eventIdI2CEngineState, $"CANCEL TRANSFER: {engineState}");
    }

    private static void Cancel(MCP2221 device, I2CAddress address, Exception exceptionCauseOfCancellation)
    {
      var engineState = device.Command(
        userData: default,
        arg: (address, exceptionCauseOfCancellation),
        cancellationToken: default,
        constructCommand: CancelTransferCommand.ConstructCommand,
        parseResponse: CancelTransferCommand.ParseResponse
      );

      device.logger?.LogWarning(eventIdI2CEngineState, $"CANCEL TRANSFER: {engineState}");
    }

    public ValueTask WriteAsync(
      I2CAddress address,
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
      I2CAddress address,
      ReadOnlyMemory<byte> buffer,
      CancellationToken cancellationToken = default
    )
    {
      if (MaxBlockLength < buffer.Length)
        throw new ArgumentException($"transfer length must be up to {MaxBlockLength} bytes", nameof(buffer));

      try {
        device.logger?.LogInformation(eventIdI2CCommand, $"I2C Write {buffer.Length} bytes to 0x{address}");

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
        device.logger?.LogError(eventIdI2CCommand, $"I2C Write to 0x{address} failed: {ex.Message}");
        if (ex is not I2CNAckException)
          await CancelAsync(device, address, ex).ConfigureAwait(false);
        throw;
      }
    }

    public void Write(
      I2CAddress address,
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
      I2CAddress address,
      ReadOnlySpan<byte> buffer,
      CancellationToken cancellationToken = default
    )
    {
      if (MaxBlockLength < buffer.Length)
        throw new ArgumentException($"transfer length must be up to {MaxBlockLength} bytes", nameof(buffer));

      try {
        device.logger?.LogInformation(eventIdI2CCommand, $"I2C Write {buffer.Length} bytes to 0x{address}");

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
        device.logger?.LogError(eventIdI2CCommand, $"I2C Write to 0x{address} failed: {ex.Message}");
        if (ex is not I2CNAckException)
          Cancel(device, address, ex);
        throw;
      }
    }

    public ValueTask<int> ReadAsync(
      I2CAddress address,
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
      I2CAddress address,
      Memory<byte> buffer,
      CancellationToken cancellationToken = default
    )
    {
      if (MaxBlockLength < buffer.Length)
        throw new ArgumentException($"transfer length must be up to {MaxBlockLength} bytes", nameof(buffer));

      try {
        device.logger?.LogInformation(eventIdI2CCommand, $"I2C Read {buffer.Length} bytes from 0x{address}");

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
        device.logger?.LogError(eventIdI2CCommand, $"I2C Read from 0x{address} failed: {ex.Message}");
        if (ex is not I2CReadException)
          await CancelAsync(device, address, ex).ConfigureAwait(false);
        throw;
      }
    }

    public int Read(
      I2CAddress address,
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
      I2CAddress address,
      Span<byte> buffer,
      CancellationToken cancellationToken = default
    )
    {
      if (MaxBlockLength < buffer.Length)
        throw new ArgumentException($"transfer length must be up to {MaxBlockLength} bytes", nameof(buffer));

      try {
        device.logger?.LogInformation(eventIdI2CCommand, $"I2C Read {buffer.Length} bytes from 0x{address}");

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
        device.logger?.LogError(eventIdI2CCommand, $"I2C Read from 0x{address} failed: {ex.Message}");
        if (ex is not I2CReadException)
          Cancel(device, address, ex);
        throw;
      }
    }

    public async ValueTask WriteByteAsync(
      I2CAddress address,
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

    public unsafe void WriteByte(
      I2CAddress address,
      byte value,
      CancellationToken cancellationToken = default
    )
      => Write(address, stackalloc byte[1] { value }, cancellationToken);

    public async ValueTask<int> ReadByteAsync(
      I2CAddress address,
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

    public unsafe int ReadByte(
      I2CAddress address,
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
