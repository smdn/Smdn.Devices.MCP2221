// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT

#pragma warning disable CA1848, CA1873, CA2254

using System;
using System.Buffers;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

namespace Smdn.Devices.Mcp2221A.Peripherals.I2c;

#pragma warning disable IDE0040
partial class I2cController {
#pragma warning restore IDE0040
  public const int MaxBlockLength = 0xFFFF;
  private const int MaxTransferLengthPerCommand = 64 - 4;

  internal const int DefaultTransmissionSpeedInKbps = 100;

  /// <remarks>
  ///   <include
  ///     file="../Smdn.Devices.Mcp2221A.docs.xml"
  ///     path="docs/I2cReadWriteTransmissionSpeedParameter/remarks/*"
  ///   />
  /// </remarks>
  public ValueTask WriteAsync(
    I2cAddress address,
    int transmissionSpeedInKbps,
    byte[] buffer,
    int offset,
    int count,
    CancellationToken cancellationToken = default
  )
    => WriteAsync(
      address,
      transmissionSpeedInKbps,
      (buffer ?? throw new ArgumentNullException(nameof(buffer))).AsMemory(offset, count),
      cancellationToken
    );

  /// <remarks>
  ///   <include
  ///     file="../Smdn.Devices.Mcp2221A.docs.xml"
  ///     path="docs/I2cReadWriteTransmissionSpeedParameter/remarks/*"
  ///   />
  ///   <para>
  ///     An empty buffer can be specified to <paramref name="buffer"/>.
  ///     This method issues writing command with 0-length in this case.
  ///   </para>
  /// </remarks>
  public async ValueTask WriteAsync(
    I2cAddress address,
    int transmissionSpeedInKbps,
    ReadOnlyMemory<byte> buffer,
    CancellationToken cancellationToken = default
  )
  {
    if (MaxBlockLength < buffer.Length)
      throw new ArgumentException($"transfer length must be up to {MaxBlockLength} bytes", nameof(buffer));

    var busSpeedDivider = Device.CalculateBusSpeedDividerOrThrow(
      transmissionSpeedInKbps,
      nameof(transmissionSpeedInKbps)
    );

    try {
      logger?.LogInformation(EventIdI2cCommand, $"I2C Write {buffer.Length} bytes to 0x{address}");

      for (; ; ) {
        var lengthToTransfer = Math.Min(buffer.Length, MaxTransferLengthPerCommand);
        var stateMachine = new I2cOperationStateMachine(logger, busSpeedDivider);

        foreach (var (constructCommand, parseResponse) in stateMachine.IterateWriteCommands()) {
          await Device.CommandAsync(
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
      logger?.LogError(EventIdI2cCommand, $"I2C Write to 0x{address} failed: {ex.Message}");
      if (ex is not I2cNackException)
        await CancelAsync(address, ex).ConfigureAwait(false);
      throw;
    }
  }

  /// <remarks>
  ///   <include
  ///     file="../Smdn.Devices.Mcp2221A.docs.xml"
  ///     path="docs/I2cReadWriteTransmissionSpeedParameter/remarks/*"
  ///   />
  /// </remarks>
  public void Write(
    I2cAddress address,
    int transmissionSpeedInKbps,
    byte[] buffer,
    int offset,
    int count,
    CancellationToken cancellationToken = default
  )
    => Write(
      address,
      transmissionSpeedInKbps,
      (buffer ?? throw new ArgumentNullException(nameof(buffer))).AsSpan(offset, count),
      cancellationToken
    );

  /// <remarks>
  ///   <include
  ///     file="../Smdn.Devices.Mcp2221A.docs.xml"
  ///     path="docs/I2cReadWriteTransmissionSpeedParameter/remarks/*"
  ///   />
  ///   <para>
  ///     An empty buffer can be specified to <paramref name="buffer"/>.
  ///     This method issues writing command with 0-length in this case.
  ///   </para>
  /// </remarks>
  public void Write(
    I2cAddress address,
    int transmissionSpeedInKbps,
    ReadOnlySpan<byte> buffer,
    CancellationToken cancellationToken = default
  )
  {
    if (MaxBlockLength < buffer.Length)
      throw new ArgumentException($"transfer length must be up to {MaxBlockLength} bytes", nameof(buffer));

    var busSpeedDivider = Device.CalculateBusSpeedDividerOrThrow(
      transmissionSpeedInKbps,
      nameof(transmissionSpeedInKbps)
    );

    try {
      logger?.LogInformation(EventIdI2cCommand, $"I2C Write {buffer.Length} bytes to 0x{address}");

      for (; ; ) {
        var lengthToTransfer = Math.Min(buffer.Length, MaxTransferLengthPerCommand);
        var stateMachine = new I2cOperationStateMachine(logger, busSpeedDivider);

        foreach (var (constructCommand, parseResponse) in stateMachine.IterateWriteCommands()) {
          Device.Command(
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
      logger?.LogError(EventIdI2cCommand, $"I2C Write to 0x{address} failed: {ex.Message}");
      if (ex is not I2cNackException)
        Cancel(address, ex);
      throw;
    }
  }

  /// <remarks>
  ///   <include
  ///     file="../Smdn.Devices.Mcp2221A.docs.xml"
  ///     path="docs/I2cReadWriteTransmissionSpeedParameter/remarks/*"
  ///   />
  /// </remarks>
  public ValueTask<int> ReadAsync(
    I2cAddress address,
    int transmissionSpeedInKbps,
    byte[] buffer,
    int offset,
    int count,
    CancellationToken cancellationToken = default
  )
    => ReadAsync(
      address,
      transmissionSpeedInKbps,
      (buffer ?? throw new ArgumentNullException(nameof(buffer))).AsMemory(offset, count),
      cancellationToken
    );

  /// <remarks>
  ///   <include
  ///     file="../Smdn.Devices.Mcp2221A.docs.xml"
  ///     path="docs/I2cReadWriteTransmissionSpeedParameter/remarks/*"
  ///   />
  ///   <para>
  ///     An empty buffer can be specified to <paramref name="buffer"/>.
  ///     This method issues reading command with 0-length in this case.
  ///   </para>
  /// </remarks>
  public async ValueTask<int> ReadAsync(
    I2cAddress address,
    int transmissionSpeedInKbps,
    Memory<byte> buffer,
    CancellationToken cancellationToken = default
  )
  {
    if (MaxBlockLength < buffer.Length)
      throw new ArgumentException($"transfer length must be up to {MaxBlockLength} bytes", nameof(buffer));

    var busSpeedDivider = Device.CalculateBusSpeedDividerOrThrow(
      transmissionSpeedInKbps,
      nameof(transmissionSpeedInKbps)
    );

    try {
      logger?.LogInformation(EventIdI2cCommand, $"I2C Read {buffer.Length} bytes from 0x{address}");

      var readBuffer = ArrayPool<byte>.Shared.Rent(MaxTransferLengthPerCommand);

      try {
        var totalReadLength = 0;

        for (; ; ) {
          var lengthToTransfer = Math.Min(buffer.Length, MaxTransferLengthPerCommand);
          var readBufferMemory = readBuffer.AsMemory(0, lengthToTransfer);
          var stateMachine = new I2cOperationStateMachine(logger, busSpeedDivider);

          foreach (var (constructCommand, parseResponse) in stateMachine.IterateReadCommands()) {
            await Device.CommandAsync(
              userData: buffer.Slice(0, lengthToTransfer),
              arg: (address, readBufferMemory),
              cancellationToken: cancellationToken,
              constructCommand: constructCommand,
              parseResponse: parseResponse
            ).ConfigureAwait(false);
          }

          if (stateMachine.ReadLength < 0)
            break;

          readBufferMemory.Slice(0, stateMachine.ReadLength).CopyTo(buffer);

          buffer = buffer.Slice(stateMachine.ReadLength);

          if (stateMachine.ReadLength < lengthToTransfer)
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
      logger?.LogError(EventIdI2cCommand, $"I2C Read from 0x{address} failed: {ex.Message}");
      if (ex is not I2cReadException)
        await CancelAsync(address, ex).ConfigureAwait(false);
      throw;
    }
  }

  /// <remarks>
  ///   <include
  ///     file="../Smdn.Devices.Mcp2221A.docs.xml"
  ///     path="docs/I2cReadWriteTransmissionSpeedParameter/remarks/*"
  ///   />
  /// </remarks>
  public int Read(
    I2cAddress address,
    int transmissionSpeedInKbps,
    byte[] buffer,
    int offset,
    int count,
    CancellationToken cancellationToken = default
  )
    => Read(
      address,
      transmissionSpeedInKbps,
      (buffer ?? throw new ArgumentNullException(nameof(buffer))).AsSpan(offset, count),
      cancellationToken
    );

  /// <remarks>
  ///   <include
  ///     file="../Smdn.Devices.Mcp2221A.docs.xml"
  ///     path="docs/I2cReadWriteTransmissionSpeedParameter/remarks/*"
  ///   />
  ///   <para>
  ///     An empty buffer can be specified to <paramref name="buffer"/>.
  ///     This method issues reading command with 0-length in this case.
  ///   </para>
  /// </remarks>
  public int Read(
    I2cAddress address,
    int transmissionSpeedInKbps,
    Span<byte> buffer,
    CancellationToken cancellationToken = default
  )
  {
    if (MaxBlockLength < buffer.Length)
      throw new ArgumentException($"transfer length must be up to {MaxBlockLength} bytes", nameof(buffer));

    var busSpeedDivider = Device.CalculateBusSpeedDividerOrThrow(
      transmissionSpeedInKbps,
      nameof(transmissionSpeedInKbps)
    );

    try {
      logger?.LogInformation(EventIdI2cCommand, $"I2C Read {buffer.Length} bytes from 0x{address}");

      var readBuffer = ArrayPool<byte>.Shared.Rent(MaxTransferLengthPerCommand);

      try {
        var totalReadLength = 0;

        for (; ; ) {
          var lengthToTransfer = Math.Min(buffer.Length, MaxTransferLengthPerCommand);
          var readBufferMemory = readBuffer.AsMemory(0, lengthToTransfer);
          var stateMachine = new I2cOperationStateMachine(logger, busSpeedDivider);

          foreach (var (constructCommand, parseResponse) in stateMachine.IterateReadCommands()) {
            Device.Command(
              userData: buffer.Slice(0, lengthToTransfer),
              arg: (address, readBufferMemory),
              cancellationToken: cancellationToken,
              constructCommand: constructCommand,
              parseResponse: parseResponse
            );
          }

          if (stateMachine.ReadLength < 0)
            break;

          readBufferMemory.Span.Slice(0, stateMachine.ReadLength).CopyTo(buffer);

          buffer = buffer.Slice(stateMachine.ReadLength);

          if (stateMachine.ReadLength < lengthToTransfer)
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
      logger?.LogError(EventIdI2cCommand, $"I2C Read from 0x{address} failed: {ex.Message}");
      if (ex is not I2cReadException)
        Cancel(address, ex);
      throw;
    }
  }

  /// <remarks>
  ///   <include
  ///     file="../Smdn.Devices.Mcp2221A.docs.xml"
  ///     path="docs/I2cReadWriteTransmissionSpeedParameter/remarks/*"
  ///   />
  /// </remarks>
  public async ValueTask WriteByteAsync(
    I2cAddress address,
    int transmissionSpeedInKbps,
    byte value,
    CancellationToken cancellationToken = default
  )
  {
    var buffer = ArrayPool<byte>.Shared.Rent(1);

    try {
      buffer[0] = value;

      await WriteAsync(
        address,
        transmissionSpeedInKbps,
        buffer.AsMemory(0, 1),
        cancellationToken
      ).ConfigureAwait(false);
    }
    finally {
      ArrayPool<byte>.Shared.Return(buffer);
    }
  }

  /// <remarks>
  ///   <include
  ///     file="../Smdn.Devices.Mcp2221A.docs.xml"
  ///     path="docs/I2cReadWriteTransmissionSpeedParameter/remarks/*"
  ///   />
  /// </remarks>
  public void WriteByte(
    I2cAddress address,
    int transmissionSpeedInKbps,
    byte value,
    CancellationToken cancellationToken = default
  )
    => Write(address, transmissionSpeedInKbps, [value], cancellationToken);

  /// <remarks>
  ///   <include
  ///     file="../Smdn.Devices.Mcp2221A.docs.xml"
  ///     path="docs/I2cReadWriteTransmissionSpeedParameter/remarks/*"
  ///   />
  /// </remarks>
  public async ValueTask<int> ReadByteAsync(
    I2cAddress address,
    int transmissionSpeedInKbps,
    CancellationToken cancellationToken = default
  )
  {
    var buffer = ArrayPool<byte>.Shared.Rent(1);

    try {
      var ret = await ReadAsync(
        address,
        transmissionSpeedInKbps,
        buffer.AsMemory(0, 1),
        cancellationToken
      ).ConfigureAwait(false);

      return 0 == ret ? -1 : buffer[0];
    }
    finally {
      ArrayPool<byte>.Shared.Return(buffer);
    }
  }

  /// <remarks>
  ///   <include
  ///     file="../Smdn.Devices.Mcp2221A.docs.xml"
  ///     path="docs/I2cReadWriteTransmissionSpeedParameter/remarks/*"
  ///   />
  /// </remarks>
  public int ReadByte(
    I2cAddress address,
    int transmissionSpeedInKbps,
    CancellationToken cancellationToken = default
  )
  {
    Span<byte> buffer = stackalloc byte[1];

    var ret = Read(address, transmissionSpeedInKbps, buffer, cancellationToken);

    return 0 == ret ? -1 : buffer[0];
  }
}
