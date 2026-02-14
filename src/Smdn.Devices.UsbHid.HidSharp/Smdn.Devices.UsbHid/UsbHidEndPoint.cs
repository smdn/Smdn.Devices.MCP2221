// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT

using System;
using System.Buffers;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

using HidSharp;

namespace Smdn.Devices.UsbHid;

internal class UsbHidEndPoint(
  UsbHidDevice openedDevice,
  HidStream openedStream,
  bool shouldDisposeDevice
) : IUsbHidEndPoint<HidStream, HidStream> {
  private UsbHidDevice? device = openedDevice ?? throw new ArgumentNullException(nameof(openedDevice));
  public IUsbHidDevice Device => device ?? throw new ObjectDisposedException(GetType().Name);

  private HidStream? endPointImplementation = openedStream ?? throw new ArgumentNullException(nameof(openedStream));
  private HidStream EndPointImplementation => endPointImplementation ?? throw new ObjectDisposedException(GetType().Name);

  public bool CanRead => EndPointImplementation.CanRead;
  public bool CanWrite => EndPointImplementation.CanWrite;

  public HidStream? ReadEndPoint => EndPointImplementation;
  public HidStream? WriteEndPoint => EndPointImplementation;

  private int MaxOutputReportLength => EndPointImplementation.Device.GetMaxOutputReportLength();
  private int MaxInputReportLength => EndPointImplementation.Device.GetMaxInputReportLength();

  private void ThrowIfDisposed()
  {
    if (device is null)
      throw new ObjectDisposedException(GetType().FullName);
  }

  public void Dispose()
  {
    if (shouldDisposeDevice) {
      device?.Dispose();
      device = null;
    }

    endPointImplementation?.Dispose();
    endPointImplementation = null;
  }

#if NET || NETSTANDARD2_1_OR_GREATER
  public async ValueTask DisposeAsync()
  {
    if (shouldDisposeDevice && device is not null) {
      await device.DisposeAsync().ConfigureAwait(false);
      device = null;
    }

    if (endPointImplementation is not null) {
      await endPointImplementation.DisposeAsync().ConfigureAwait(false);
      endPointImplementation = null;
    }
  }
#else
  public ValueTask DisposeAsync()
  {
    Dispose();

    return default;
  }
#endif

  public void Write(ReadOnlySpan<byte> buffer, CancellationToken cancellationToken)
  {
    if (MaxOutputReportLength < buffer.Length)
      throw new ArgumentException($"length of the buffer must be less than or equals to maximum output report length ({MaxOutputReportLength})", nameof(buffer));

    ThrowIfDisposed();

    cancellationToken.ThrowIfCancellationRequested();

    var len = buffer.Length;
    var buf = ArrayPool<byte>.Shared.Rent(MaxOutputReportLength);

    try {
      buffer.CopyTo(buf);

      EndPointImplementation.Write(buf, 0, len);
    }
    finally {
      ArrayPool<byte>.Shared.Return(buf);
    }
  }

  public ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken)
  {
    if (MaxOutputReportLength < buffer.Length)
      throw new ArgumentException($"length of the buffer must be less than or equals to maximum output report length ({MaxOutputReportLength})", nameof(buffer));

    ThrowIfDisposed();

    cancellationToken.ThrowIfCancellationRequested();

    var len = buffer.Length;
    var buf = ArrayPool<byte>.Shared.Rent(MaxOutputReportLength);

    try {
      buffer.CopyTo(buf);

      EndPointImplementation.Write(buf, 0, len);
    }
    finally {
      ArrayPool<byte>.Shared.Return(buf);
    }

#if SYSTEM_THREADING_TASKS_VALUETASK_COMPLETEDTASK
    return ValueTask.CompletedTask;
#else
    return default;
#endif
  }

  public int Read(Span<byte> buffer, CancellationToken cancellationToken)
  {
    if (MaxInputReportLength < buffer.Length)
      throw new ArgumentException($"length of the buffer must be less than or equals to maximum input report length ({MaxInputReportLength})", nameof(buffer));

    ThrowIfDisposed();

    cancellationToken.ThrowIfCancellationRequested();

    var len = buffer.Length;
    var buf = ArrayPool<byte>.Shared.Rent(MaxInputReportLength);

    try {
      var ret = EndPointImplementation.Read(buf, 0, len);

      buf.AsSpan(0, ret).CopyTo(buffer);

      return ret;
    }
    finally {
      ArrayPool<byte>.Shared.Return(buf);
    }
  }

  public ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken)
  {
    if (MaxInputReportLength < buffer.Length)
      throw new ArgumentException($"length of the buffer must be less than or equals to maximum input report length ({MaxInputReportLength})", nameof(buffer));

    ThrowIfDisposed();

    cancellationToken.ThrowIfCancellationRequested();

    var len = buffer.Length;
    byte[]? rentBuffer = null;

    if (!MemoryMarshal.TryGetArray<byte>(buffer, out var buf)) {
      rentBuffer = ArrayPool<byte>.Shared.Rent(MaxInputReportLength);

      buf = new ArraySegment<byte>(rentBuffer, 0, len);
    }

    try {
      return
#pragma warning disable SA1114
#if SYSTEM_THREADING_TASKS_VALUETASK_FROMRESULT
        ValueTask.FromResult<int>(
#else
        new(
#endif
#pragma warning restore SA1114
          result: EndPointImplementation.Read(buf.Array ?? throw new InvalidOperationException("destination array is null"), buf.Offset, buf.Count)
        );
    }
    finally {
      if (rentBuffer != null) {
        buf.AsMemory().CopyTo(buffer);

        ArrayPool<byte>.Shared.Return(rentBuffer);
      }
    }
  }

  public override string? ToString()
    => $"{GetType().FullName} (EndPointImplementation='{EndPointImplementation}')";
}
