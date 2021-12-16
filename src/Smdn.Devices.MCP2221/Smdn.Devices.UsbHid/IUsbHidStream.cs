// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT

using System;
using System.Threading.Tasks;

namespace Smdn.Devices.UsbHid;

public interface IUsbHidStream :
  IDisposable,
  IAsyncDisposable
{
  bool RequiresPacketOnly { get; }

  ValueTask WriteAsync(ReadOnlyMemory<byte> buffer);
  void Write(ReadOnlySpan<byte> buffer);

  ValueTask<int> ReadAsync(Memory<byte> buffer);
  int Read(Span<byte> buffer);
}

