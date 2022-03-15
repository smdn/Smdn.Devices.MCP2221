// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT

#if USBHIDDRIVER_LIBUSBDOTNET

using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

using LibUsbDotNet.LibUsb;

using Smdn.Devices.UsbHid;

namespace Smdn.Devices.UsbHid.LibUsbDotNet;

internal class Stream : IUsbHidStream {
  internal const int DefaultReadBufferSize = 0x100; // XXX
  private static readonly TimeSpan defaultTimeout = TimeSpan.FromSeconds(10);

  private UsbEndpointWriter _writer;
  private UsbEndpointWriter Writer => _writer ?? throw new ObjectDisposedException(GetType().Name);

  private UsbEndpointReader _reader;
  private UsbEndpointReader Reader => _reader ?? throw new ObjectDisposedException(GetType().Name);

  private readonly int maxOutPacketSize;
  private readonly int maxInPacketSize;

  public bool RequiresPacketOnly => true;

  internal Stream(
    UsbEndpointWriter writer,
    int maxOutPacketSize,
    UsbEndpointReader reader,
    int maxInPacketSize
  )
  {
    this._writer = writer;
    this._reader = reader;
    this.maxOutPacketSize = maxOutPacketSize;
    this.maxInPacketSize = maxInPacketSize;
  }

  public void Dispose()
  {
    // UsbEndpointWriter/UsbEndpointReader does not implement IDisposable
    _writer = null;
    _reader = null;
  }

  public ValueTask DisposeAsync()
  {
    _writer = null;
    _reader = null;

#if SYSTEM_THREADING_TASKS_VALUETASK_COMPLETEDTASK
    return ValueTask.CompletedTask;
#else
    return new ValueTask(Task.CompletedTask);
#endif
  }

  public unsafe void Write(ReadOnlySpan<byte> buffer)
  {
    if (maxOutPacketSize < buffer.Length)
      throw new ArgumentException($"length of the buffer must be less than or equals to maximum output packet length ({maxOutPacketSize})", nameof(buffer));

    Span<byte> buf = stackalloc byte[buffer.Length];

    buffer.CopyTo(buf);

    var err = Writer.Write(
      pBuffer: (IntPtr)Unsafe.AsPointer(ref buf.GetPinnableReference()),
      offset: 0,
      count: buf.Length,
      timeout: (int)defaultTimeout.TotalMilliseconds,
      out var transferLength
    );
  }

  public unsafe ValueTask WriteAsync(ReadOnlyMemory<byte> buffer)
  {
    if (maxOutPacketSize < buffer.Length)
      throw new ArgumentException($"length of the buffer must be less than or equals to maximum output packet length ({maxOutPacketSize})", nameof(buffer));

    Span<byte> buf = stackalloc byte[buffer.Length];

    buffer.Span.CopyTo(buf);

    // TODO: SubmitAsyncTransfer
    var err = Writer.Write(
      pBuffer: (IntPtr)Unsafe.AsPointer(ref buf.GetPinnableReference()),
      offset: 0,
      count: buf.Length,
      timeout: (int)defaultTimeout.TotalMilliseconds,
      out var transferLength
    );

#if SYSTEM_THREADING_TASKS_VALUETASK_COMPLETEDTASK
    return ValueTask.CompletedTask;
#else
    return default;
#endif
  }

  public unsafe int Read(Span<byte> buffer)
  {
    if (maxInPacketSize < buffer.Length)
      throw new ArgumentException($"length of the buffer must be less than or equals to maximum input packet length ({maxInPacketSize})", nameof(buffer));

    var err = Reader.Read(
      buffer: (IntPtr)Unsafe.AsPointer(ref buffer.GetPinnableReference()),
      offset: 0,
      count: buffer.Length,
      timeout: (int)defaultTimeout.TotalMilliseconds,
      out var transferLength
    );

    return transferLength;
  }

  public unsafe ValueTask<int> ReadAsync(Memory<byte> buffer)
  {
    if (maxInPacketSize < buffer.Length)
      throw new ArgumentException($"length of the buffer must be less than or equals to maximum input packet length ({maxInPacketSize})", nameof(buffer));

    // TODO: SubmitAsyncTransfer
    var err = Reader.Read(
      buffer: (IntPtr)Unsafe.AsPointer(ref buffer.Span.GetPinnableReference()),
      offset: 0,
      count: buffer.Length,
      timeout: (int)defaultTimeout.TotalMilliseconds,
      out var transferLength
    );

    return
#if SYSTEM_THREADING_TASKS_VALUETASK_FROMRESULT
    ValueTask.FromResult<int>
#else
    new ValueTask<int>
#endif
    (transferLength);
  }
}

#endif // USBHIDDRIVER_LIBUSBDOTNET
