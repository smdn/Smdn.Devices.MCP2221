// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT

using System;
using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;
using Smdn.Devices.UsbHid;

namespace Smdn.Devices.MCP2221 {
  [TestFixture]
  public partial class MCP2221Tests {
    private const byte ReportInput = 0x00;
    private const byte ReportOutput = 0x00;

    private static PseudoUsbHidDevice CreatePreudoDevice()
      => new PseudoUsbHidDevice(
        createWriteStream: () => new MemoryStream(capacity: (1 + 64) * 5),
        createReadStream: () => {
          var readStream = new MemoryStream(capacity: (1 + 64) * 5);

          readStream.Write(new byte[] {
            // [MCP2221A] 3.1.1 STATUS/SET PARAMETERS
            ReportInput, 0x10, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x03, 0x00, 0x03, 0x00, 0x03, 0x14, 0x00, 0x40, 0x00, 0x10, 0x28, 0x00, 0x60, 0x01, 0x01, 0x00, 0x00, 0xF1, 0x79, 0xF0, 0x00, 0x00, 0x00, 0x30, 0x30, 0x0B, 0x30, 0x14, 0x23, 0x17, 0x7D, 0x06, 0x00, 0x00, 0x26, 0x94, 0x14, 0x41, 0x36, 0x31, 0x32, 0xFB, 0x03, 0x00, 0x00, 0xFA, 0x03, 0x76, 0x03, 0x5B, 0x02, 0x00, 0x00, 0x00, 0x00,
          });

          readStream.Write(new byte[] {
            // [MCP2221A] 3.1.2 READ FLASH DATA
            ReportInput, 0xB0, 0x00, 0x08, 0x00, 0x30, 0x31, 0x32, 0x33, 0x34, 0x35, 0x36, 0x37, 0x00, 0x03, 0x14, 0x00, 0x40, 0x00, 0x10, 0x28, 0x00, 0x60, 0x01, 0x01, 0x00, 0x00, 0xF1, 0x79, 0xF0, 0x00, 0x00, 0x00, 0x30, 0x30, 0x0B, 0x30, 0x14, 0x23, 0x17, 0x7D, 0x06, 0x00, 0x00, 0x26, 0x94, 0x14, 0x41, 0x36, 0x31, 0x32, 0xFB, 0x03, 0x00, 0x00, 0xFA, 0x03, 0x76, 0x03, 0x5B, 0x02, 0x00, 0x00, 0x00, 0x00
          });

          readStream.Position = 0L;

          return readStream;
        }
      );

    [Test]
    public void OpenAsync()
    {
      MCP2221 device = null;

      Assert.DoesNotThrowAsync(async () => device = await MCP2221.OpenAsync(CreatePreudoDevice));

      Assert.IsNotNull(device);
      Assert.IsNotNull(device.HidDevice);
      Assert.IsNotNull(device.FirmwareRevision);
      Assert.IsNotNull(device.HardwareRevision);
      Assert.IsNotNull(device.ChipFactorySerialNumber);
    }

    [Test]
    public void OpenAsync_ArgumentNull()
      => Assert.ThrowsAsync<ArgumentNullException>(async () => await MCP2221.OpenAsync((Func<IUsbHidDevice>)null));

    [Test]
    public void OpenAsync_CreateDeviceReturnNull()
      => Assert.ThrowsAsync<DeviceNotFoundException>(async () => await MCP2221.OpenAsync(() => (IUsbHidDevice)null));



    [Test] public async Task Dispose() => await TestDispose(d => { d.Dispose(); return Task.CompletedTask; });

    [Test] public async Task DisposeAsync() => await TestDispose(async d => await d.DisposeAsync());

    private async Task TestDispose(Func<MCP2221, Task> disposeAction)
    {
      await using var device = await MCP2221.OpenAsync(CreatePreudoDevice);

      Assert.DoesNotThrow(() => Assert.IsNotNull(device.HidDevice));

      Assert.DoesNotThrow(() => Assert.IsNotNull(device.HardwareRevision));
      Assert.DoesNotThrow(() => Assert.IsNotNull(device.FirmwareRevision));
      Assert.DoesNotThrow(() => Assert.IsNotNull(device.ChipFactorySerialNumber));
      Assert.DoesNotThrow(() => Assert.IsNotNull(device.GPs));
      Assert.DoesNotThrow(() => Assert.IsNotNull(device.GP0));
      Assert.DoesNotThrow(() => Assert.IsNotNull(device.GP1));
      Assert.DoesNotThrow(() => Assert.IsNotNull(device.GP2));
      Assert.DoesNotThrow(() => Assert.IsNotNull(device.GP3));
      Assert.DoesNotThrow(() => Assert.IsNotNull(device.I2C));

      await disposeAction(device);

      Assert.Throws<ObjectDisposedException>(() => Assert.IsNotNull(device.HidDevice));
      Assert.ThrowsAsync<ObjectDisposedException>(async () => await device.GP0.SetValueAsync(default));
      Assert.ThrowsAsync<ObjectDisposedException>(async () => await device.GP0.GetValueAsync());
      Assert.ThrowsAsync<ObjectDisposedException>(async () => await device.I2C.WriteAsync(default, default));
      Assert.ThrowsAsync<ObjectDisposedException>(async () => await device.I2C.ReadAsync(default, default));

      Assert.DoesNotThrow(() => Assert.IsNotNull(device.HardwareRevision));
      Assert.DoesNotThrow(() => Assert.IsNotNull(device.FirmwareRevision));
      Assert.DoesNotThrow(() => Assert.IsNotNull(device.ChipFactorySerialNumber));
      Assert.DoesNotThrow(() => Assert.IsNotNull(device.GPs));
      Assert.DoesNotThrow(() => Assert.IsNotNull(device.GP0));
      Assert.DoesNotThrow(() => Assert.IsNotNull(device.GP1));
      Assert.DoesNotThrow(() => Assert.IsNotNull(device.GP2));
      Assert.DoesNotThrow(() => Assert.IsNotNull(device.GP3));
      Assert.DoesNotThrow(() => Assert.IsNotNull(device.I2C));

      Assert.DoesNotThrowAsync(async () => await disposeAction(device), "dispose again");
    }
  }
}