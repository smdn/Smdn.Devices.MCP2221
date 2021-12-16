// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT

using System;
using System.IO;
using System.Threading.Tasks;
using Smdn.Devices.MCP2221;

namespace Smdn.Devices.UsbHid;

class PseudoUsbHidDevice : IUsbHidDevice {
  public string ProductName => nameof(PseudoUsbHidDevice);
  public string Manufacturer => typeof(PseudoUsbHidDevice).Assembly.GetName().Name;
  public int VendorID => 0xCAFE;
  public int ProductID => 0xFEED;
  public string SerialNumber => typeof(PseudoUsbHidDevice).Assembly.GetName().FullName;
  public Version ReleaseNumber => typeof(PseudoUsbHidDevice).Assembly.GetName().Version;
  public string DevicePath => "<null>";
  public string FileSystemName => "/dev/null";

  private readonly Func<Stream> createWriteStream;
  private readonly Func<Stream> createReadStream;

  private PseudoUsbHidStream stream = null;
  public PseudoUsbHidStream Stream => stream ?? throw new ObjectDisposedException(GetType().FullName);

  public PseudoUsbHidDevice(Func<Stream> createWriteStream, Func<Stream> createReadStream)
  {
    this.createWriteStream = createWriteStream;
    this.createReadStream = createReadStream;
  }

  public void Dispose()
  {
    stream?.Dispose();
    stream = null;
  }

  public async ValueTask DisposeAsync()
  {
    if (stream != null) {
      await stream.DisposeAsync().ConfigureAwait(false);
      stream = null;
    }
  }

  public ValueTask<IUsbHidStream> OpenStreamAsync()
  {
    stream = new PseudoUsbHidStream(createWriteStream?.Invoke(), createReadStream?.Invoke());

    return
#if NET5_0_OR_GREATER
    ValueTask.FromResult<IUsbHidStream>
#else
    new ValueTask<IUsbHidStream>
#endif
    (stream);
  }

  public IUsbHidStream OpenStream()
  {
    return new PseudoUsbHidStream(createWriteStream?.Invoke(), createReadStream?.Invoke());
  }
}
