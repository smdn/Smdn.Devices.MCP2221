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
            // [MCP2221A] 3.1.2 READ FLASH DATA - TABLE 3-7 RESPONSE STRUCTURE - READ USB MANUFACTURER DESCRIPTOR STRING SUB-COMMAND
            ReportInput, 0xB0, 0x00, 0x34, 0x03, 0x4D, 0x00, 0x69, 0x00, 0x63, 0x00, 0x72, 0x00, 0x6F, 0x00, 0x63, 0x00, 0x68, 0x00, 0x69, 0x00, 0x70, 0x00, 0x20, 0x00, 0x54, 0x00, 0x65, 0x00, 0x63, 0x00, 0x68, 0x00, 0x6E, 0x00, 0x6F, 0x00, 0x6C, 0x00, 0x6F, 0x00, 0x67, 0x00, 0x79, 0x00, 0x20, 0x00, 0x49, 0x00, 0x6E, 0x00, 0x63, 0x00, 0x2E, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
          });

          readStream.Write(new byte[] {
            // [MCP2221A] 3.1.2 READ FLASH DATA - TABLE 3-8 RESPONSE STRUCTURE - READ USB PRODUCT DESCRIPTOR STRING SUB-COMMAND
            ReportInput, 0xB0, 0x00, 0x36, 0x03, 0x4D, 0x00, 0x43, 0x00, 0x50, 0x00, 0x32, 0x00, 0x32, 0x00, 0x32, 0x00, 0x31, 0x00, 0x20, 0x00, 0x55, 0x00, 0x53, 0x00, 0x42, 0x00, 0x2D, 0x00, 0x49, 0x00, 0x32, 0x00, 0x43, 0x00, 0x2F, 0x00, 0x55, 0x00, 0x41, 0x00, 0x52, 0x00, 0x54, 0x00, 0x20, 0x00, 0x43, 0x00, 0x6F, 0x00, 0x6D, 0x00, 0x62, 0x00, 0x6F, 0x00, 0x79, 0x03, 0x7A, 0x02, 0x00, 0x00, 0x00, 0x00
          });

          readStream.Write(new byte[] {
            // [MCP2221A] 3.1.2 READ FLASH DATA - TABLE 3-9 RESPONSE STRUCTURE - READ USB SERIAL NUMBER DESCRIPTOR STRING SUB-COMMAND
            ReportInput, 0xB0, 0x00, 0x16, 0x03, 0x58, 0x00, 0x58, 0x00, 0x58, 0x00, 0x58, 0x00, 0x58, 0x00, 0x58, 0x00, 0x58, 0x00, 0x58, 0x00, 0x58, 0x00, 0x58, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
          });

          readStream.Write(new byte[] {
            // [MCP2221A] 3.1.2 READ FLASH DATA - TABLE 3-10 RESPONSE STRUCTURE - READ CHIP FACTORY SERIAL NUMBER SUB-COMMAND
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
      Assert.IsNotNull(device.ManufacturerDescriptor);
      Assert.IsNotNull(device.ProductDescriptor);
      Assert.IsNotNull(device.SerialNumberDescriptor);
      Assert.IsNotNull(device.ChipFactorySerialNumber);

      Assert.AreEqual("1.2", device.FirmwareRevision, nameof(device.FirmwareRevision));
      Assert.AreEqual("A.6", device.HardwareRevision, nameof(device.HardwareRevision));
      Assert.AreEqual("Microchip Technology Inc.", device.ManufacturerDescriptor, nameof(device.ManufacturerDescriptor));
      Assert.AreEqual("MCP2221 USB-I2C/UART Combo", device.ProductDescriptor, nameof(device.ProductDescriptor));
      Assert.AreEqual("XXXXXXXXXX", device.SerialNumberDescriptor, nameof(device.SerialNumberDescriptor));
      Assert.AreEqual("01234567", device.ChipFactorySerialNumber, nameof(device.ChipFactorySerialNumber));
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
      Assert.DoesNotThrow(() => Assert.IsNotNull(device.ManufacturerDescriptor));
      Assert.DoesNotThrow(() => Assert.IsNotNull(device.ProductDescriptor));
      Assert.DoesNotThrow(() => Assert.IsNotNull(device.SerialNumberDescriptor));
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
      Assert.DoesNotThrow(() => Assert.IsNotNull(device.ManufacturerDescriptor));
      Assert.DoesNotThrow(() => Assert.IsNotNull(device.ProductDescriptor));
      Assert.DoesNotThrow(() => Assert.IsNotNull(device.SerialNumberDescriptor));
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