// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT

using System;
using System.IO;
using System.Threading.Tasks;
using Smdn.Devices.MCP2221;

namespace Smdn.Devices.UsbHid;

class PseudoUsbHidStream : IUsbHidStream {
  public Stream WriteStream { get; }
  public Stream ReadStream { get; }
  public bool RequiresPacketOnly => false;

  public PseudoUsbHidStream(Stream writeStream, Stream readStream)
  {
    this.WriteStream = writeStream ?? throw new ArgumentNullException(nameof(writeStream));
    this.ReadStream = readStream ?? throw new ArgumentNullException(nameof(readStream));
  }

  public void Dispose()
  {
    WriteStream.Dispose();
    ReadStream.Dispose();
  }

  public async ValueTask DisposeAsync()
  {
    await WriteStream.DisposeAsync();
    await ReadStream.DisposeAsync();
  }

  public void Write(ReadOnlySpan<byte> buffer) => WriteStream.Write(buffer);
  public ValueTask WriteAsync(ReadOnlyMemory<byte> buffer) => WriteStream.WriteAsync(buffer);
  public int Read(Span<byte> buffer) => ReadStream.Read(buffer);
  public ValueTask<int> ReadAsync(Memory<byte> buffer) => ReadStream.ReadAsync(buffer);
}
