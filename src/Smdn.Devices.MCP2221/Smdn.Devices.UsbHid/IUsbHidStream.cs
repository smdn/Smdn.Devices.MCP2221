// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
#pragma warning disable CA1711 // CA1711: Identifiers should not have incorrect suffix

using System;
using System.Threading.Tasks;

namespace Smdn.Devices.UsbHid;

#pragma warning disable IDE0055
public interface IUsbHidStream :
  IDisposable,
  IAsyncDisposable
{
#pragma warning restore IDE0055
  bool RequiresPacketOnly { get; }

  ValueTask WriteAsync(ReadOnlyMemory<byte> buffer);
  void Write(ReadOnlySpan<byte> buffer);

  ValueTask<int> ReadAsync(Memory<byte> buffer);
  int Read(Span<byte> buffer);
}
