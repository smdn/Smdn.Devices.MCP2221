// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT

#if USBHIDDRIVER_HIDSHARP

using System;
using System.Buffers;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

using HidSharp;
using HidStream = HidSharp.HidStream;

using Smdn.Devices.UsbHid;

namespace Smdn.Devices.UsbHid.HidSharp {
  internal class Stream : IUsbHidStream {
    private HidStream _hidStream;
    private HidStream HidStream => _hidStream ?? throw new ObjectDisposedException(GetType().Name);

    private int MaxOutputReportLength => HidStream.Device.GetMaxOutputReportLength();
    private int MaxInputReportLength => HidStream.Device.GetMaxInputReportLength();

    public bool RequiresPacketOnly => false;

    internal Stream(HidStream hidStream)
    {
      this._hidStream = hidStream;
    }

    public void Dispose()
    {
      _hidStream?.Dispose();
      _hidStream = null;
    }

    public async ValueTask DisposeAsync()
    {
      if (_hidStream != null) {
        await _hidStream.DisposeAsync().ConfigureAwait(false);
        _hidStream = null;
      }
    }

    public void Write(ReadOnlySpan<byte> buffer)
    {
      if (MaxOutputReportLength < buffer.Length)
        throw new ArgumentException($"length of the buffer must be less than or equals to maximum output report length ({MaxOutputReportLength})", nameof(buffer));

      var len = buffer.Length;
      var buf = ArrayPool<byte>.Shared.Rent(MaxOutputReportLength);

      try {
        buffer.CopyTo(buf);

        HidStream.Write(buf, 0, len);
      }
      finally {
        ArrayPool<byte>.Shared.Return(buf);
      }
    }

    public ValueTask WriteAsync(ReadOnlyMemory<byte> buffer)
    {
      if (MaxOutputReportLength < buffer.Length)
        throw new ArgumentException($"length of the buffer must be less than or equals to maximum output report length ({MaxOutputReportLength})", nameof(buffer));

      var len = buffer.Length;
      var buf = ArrayPool<byte>.Shared.Rent(MaxOutputReportLength);

      try {
        buffer.CopyTo(buf);

        HidStream.Write(buf, 0, len);
      }
      finally {
        ArrayPool<byte>.Shared.Return(buf);
      }

#if NET5_0_OR_GREATER
      return ValueTask.CompletedTask;
#else
      return default;
#endif
    }

    public int Read(Span<byte> buffer)
    {
      if (MaxInputReportLength < buffer.Length)
        throw new ArgumentException($"length of the buffer must be less than or equals to maximum input report length ({MaxInputReportLength})", nameof(buffer));

      var len = buffer.Length;
      var buf = ArrayPool<byte>.Shared.Rent(MaxInputReportLength);

      try {
        var ret = HidStream.Read(buf, 0, len);

        buf.AsSpan(0, ret).CopyTo(buffer);

        return ret;
      }
      finally {
        ArrayPool<byte>.Shared.Return(buf);
      }
    }

    public ValueTask<int> ReadAsync(Memory<byte> buffer)
    {
      if (MaxInputReportLength < buffer.Length)
        throw new ArgumentException($"length of the buffer must be less than or equals to maximum input report length ({MaxInputReportLength})", nameof(buffer));

      var len = buffer.Length;
      byte[] rentBuffer = null;

      if (!MemoryMarshal.TryGetArray<byte>(buffer, out var buf)) {
        rentBuffer = ArrayPool<byte>.Shared.Rent(MaxInputReportLength);

        buf = new ArraySegment<byte>(rentBuffer, 0, len);
      }

      try {
        return
#if NET5_0_OR_GREATER
        ValueTask.FromResult<int>
#else
        new ValueTask<int>
#endif
        (HidStream.Read(buf.Array, buf.Offset, buf.Count));
      }
      finally {
        if (rentBuffer != null) {
          buf.AsMemory().CopyTo(buffer);

          ArrayPool<byte>.Shared.Return(rentBuffer);
        }
      }
    }
  }
}

#endif // USBHIDDRIVER_HIDSHARP
