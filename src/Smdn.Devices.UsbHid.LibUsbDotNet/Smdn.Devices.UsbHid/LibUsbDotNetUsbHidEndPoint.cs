// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT

using System;
using System.Buffers;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

using LibUsbDotNet.LibUsb;

namespace Smdn.Devices.UsbHid;

/// <summary>
/// An implementation of <see cref="IUsbHidEndPoint"/> that uses
/// <see cref="UsbEndpointReader"/> and <see cref="UsbEndpointWriter"/> of
/// LibUsbDotNet as the backend.
/// </summary>
public sealed class LibUsbDotNetUsbHidEndPoint : IUsbHidEndPoint<UsbEndpointReader, UsbEndpointWriter> {
  private readonly bool shouldDisposeDevice;

  private LibUsbDotNetUsbHidDevice? device;

  /// <inheritdoc/>
  public IUsbHidDevice Device => device ?? throw new ObjectDisposedException(GetType().Name);

  private bool IsDisposed => device is null;

  /// <inheritdoc/>
  public bool CanWrite => !IsDisposed && WriteEndPoint is not null;

  /// <inheritdoc/>
  public bool CanRead => !IsDisposed && ReadEndPoint is not null;

  /// <inheritdoc/>
  [CLSCompliant(false)]
  public UsbEndpointWriter? WriteEndPoint { get; private set; }

  /// <inheritdoc/>
  [CLSCompliant(false)]
  public UsbEndpointReader? ReadEndPoint { get; private set; }

  private readonly int maxOutEndPointPacketSize;
  private readonly int maxInEndPointPacketSize;

  private readonly int writeEndPointTimeoutInMilliseconds;
  private readonly int readEndPointTimeoutInMilliseconds;

  internal LibUsbDotNetUsbHidEndPoint(
    LibUsbDotNetUsbHidDevice device,
    UsbEndpointWriter? endPointWriter,
    int maxOutEndPointPacketSize,
    TimeSpan writeEndPointTimeout,
    UsbEndpointReader? endPointReader,
    int maxInEndPointPacketSize,
    TimeSpan readEndPointTimeout,
    bool shouldDisposeDevice
  )
  {
    this.device = device ?? throw new ArgumentNullException(nameof(device));
    ReadEndPoint = endPointReader;
    WriteEndPoint = endPointWriter;
    this.maxInEndPointPacketSize = maxInEndPointPacketSize;
    readEndPointTimeoutInMilliseconds = (int)readEndPointTimeout.TotalMilliseconds;
    this.maxOutEndPointPacketSize = maxOutEndPointPacketSize;
    writeEndPointTimeoutInMilliseconds = (int)writeEndPointTimeout.TotalMilliseconds;
    this.shouldDisposeDevice = shouldDisposeDevice;
  }

  private void ThrowIfDisposed()
  {
    if (device is null)
      throw new ObjectDisposedException(GetType().FullName);
  }

  /// <inheritdoc/>
  public void Dispose()
  {
    if (shouldDisposeDevice) {
      device?.Dispose();
      device = null;
    }

    // UsbEndpointWriter/UsbEndpointReader does not implement IDisposable
    WriteEndPoint = null;
    ReadEndPoint = null;
  }

  /// <inheritdoc/>
  /// <remarks>
  /// This implementation performs a synchronous disposal, as the
  /// underlying <see cref="UsbEndpointReader"/> and <see cref="UsbEndpointWriter"/>
  /// do not support asynchronous disposal.
  /// </remarks>
#if NET || NETSTANDARD2_1_OR_GREATER
  public async ValueTask DisposeAsync()
  {
    if (shouldDisposeDevice && device is not null) {
      await device.DisposeAsync().ConfigureAwait(false);
      device = null;
    }

    // UsbEndpointWriter/UsbEndpointReader does not implement IDisposable
    WriteEndPoint = null;
    ReadEndPoint = null;
  }
#else
  public ValueTask DisposeAsync()
  {
    Dispose();

    return default;
  }
#endif

  /// <inheritdoc/>
  /// <remarks>
  /// The first byte of the <paramref name="buffer"/>, which is for the Report ID, is ignored
  /// because the underlying LibUsbDotNet API does not require it.
  /// </remarks>
  public void Write(ReadOnlySpan<byte> buffer, CancellationToken cancellationToken)
  {
    if (buffer.IsEmpty)
      return;

    buffer = buffer.Slice(1); // get the slice of the payload only, excluding the report ID

    if (maxOutEndPointPacketSize < buffer.Length)
      throw new ArgumentException($"length of the buffer must be less than or equals to maximum output packet length ({maxOutEndPointPacketSize})", nameof(buffer));

    ThrowIfDisposed();

    if (WriteEndPoint is null)
      throw new InvalidOperationException("can not write");

    cancellationToken.ThrowIfCancellationRequested();

    WriteCore(
      WriteEndPoint,
      buffer,
      writeEndPointTimeoutInMilliseconds
    );

    static void WriteCore(
      UsbEndpointWriter writer,
      ReadOnlySpan<byte> buffer,
      int timeout
    )
    {
      // since UsbEndpointWriter.Write() requires Span<byte> rather than ReadOnlySpan<byte>,
      // copy the data to be written into a separately allocated Span<byte>
      var rentBuffer = ArrayPool<byte>.Shared.Rent(buffer.Length);
      var buf = rentBuffer.AsSpan(0, buffer.Length);

      buffer.CopyTo(buf);

      try {
        _ = writer.Write(
          buffer: buf,
          timeout: timeout,
          transferLength: out var transferLength
        );
      }
      finally {
        if (rentBuffer is not null)
          ArrayPool<byte>.Shared.Return(rentBuffer);
      }
    }
  }

  /// <inheritdoc/>
  /// <remarks>
  /// The first byte of the <paramref name="buffer"/>, which is for the Report ID, is ignored
  /// because the underlying LibUsbDotNet API does not require it.
  /// </remarks>
  public ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken)
  {
    if (buffer.IsEmpty)
      return default;

    buffer = buffer.Slice(1); // get the slice of the payload only, excluding the report ID

    if (maxOutEndPointPacketSize < buffer.Length)
      throw new ArgumentException($"length of the buffer must be less than or equals to maximum output packet length ({maxOutEndPointPacketSize})", nameof(buffer));

    ThrowIfDisposed();

    if (WriteEndPoint is null)
      throw new InvalidOperationException("can not write");

    cancellationToken.ThrowIfCancellationRequested();

    return WriteAsyncCore(
      WriteEndPoint,
      buffer,
      writeEndPointTimeoutInMilliseconds
    );

    static async ValueTask WriteAsyncCore(
      UsbEndpointWriter writer,
      ReadOnlyMemory<byte> buffer,
      int timeout
    )
    {
      byte[]? rentBuffer = null;

      if (!MemoryMarshal.TryGetArray(buffer, out var bufferSegment)) {
        rentBuffer = ArrayPool<byte>.Shared.Rent(buffer.Length);

        bufferSegment = new ArraySegment<byte>(rentBuffer, 0, buffer.Length);

        buffer.CopyTo(bufferSegment.AsMemory(0, buffer.Length));
      }

      try {
        _ = await writer.WriteAsync(
          buffer: bufferSegment.Array,
          offset: bufferSegment.Offset,
          length: bufferSegment.Count,
          timeout: timeout
        ).ConfigureAwait(false);
      }
      finally {
        if (rentBuffer is not null)
          ArrayPool<byte>.Shared.Return(rentBuffer);
      }
    }
  }

  /// <inheritdoc/>
  /// <remarks>
  /// The first byte of the <paramref name="buffer"/>, which is for the Report ID, is ignored
  /// because the underlying LibUsbDotNet API does not require it.
  /// </remarks>
  public int Read(Span<byte> buffer, CancellationToken cancellationToken)
  {
    if (buffer.IsEmpty)
      return 0;

    buffer = buffer.Slice(1); // get the slice of the payload only, excluding the report ID

    if (maxInEndPointPacketSize < buffer.Length)
      throw new ArgumentException($"length of the buffer must be less than or equals to maximum input packet length ({maxInEndPointPacketSize})", nameof(buffer));

    ThrowIfDisposed();

    if (ReadEndPoint is null)
      throw new InvalidOperationException("can not write");

    cancellationToken.ThrowIfCancellationRequested();

    _ = ReadEndPoint.Read(
      buffer: buffer,
      timeout: readEndPointTimeoutInMilliseconds,
      out var transferLength
    );

    return transferLength;
  }

  /// <inheritdoc/>
  /// <remarks>
  /// The first byte of the <paramref name="buffer"/>, which is for the Report ID, is ignored
  /// because the underlying LibUsbDotNet API does not require it.
  /// </remarks>
  public ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken)
  {
    if (buffer.IsEmpty) {
      return
#if SYSTEM_THREADING_TASKS_VALUETASK_FROMRESULT
        ValueTask.FromResult(result: 0);
#else
        new(result: 0);
#endif
    }

    buffer = buffer.Slice(1); // get the slice of the payload only, excluding the report ID

    if (maxInEndPointPacketSize < buffer.Length)
      throw new ArgumentException($"length of the buffer must be less than or equals to maximum input packet length ({maxInEndPointPacketSize})", nameof(buffer));

    ThrowIfDisposed();

    if (ReadEndPoint is null)
      throw new InvalidOperationException("can not write");

    cancellationToken.ThrowIfCancellationRequested();

    return ReadAsyncCore(
      ReadEndPoint,
      buffer,
      readEndPointTimeoutInMilliseconds
    );

    static async ValueTask<int> ReadAsyncCore(
      UsbEndpointReader reader,
      Memory<byte> buffer,
      int timeout
    )
    {
      var (_, transferLength) = await reader.ReadAsync(
        buffer: buffer,
        timeout: timeout
      ).ConfigureAwait(false);

      return transferLength;
    }
  }

  public override string? ToString()
    => $"{GetType().FullName} (Device='{device}', ReadEndPoint='{ReadEndPoint}', WriteEndPoint='{WriteEndPoint}')";
}
