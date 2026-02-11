// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT

using System;
using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;
using Smdn.Devices.UsbHid;

namespace Smdn.Devices.MCP2221;

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
    MCP2221? device = null;

    Assert.That(async () => device = await MCP2221.OpenAsync(CreatePreudoDevice), Throws.Nothing);

    Assert.That(device, Is.Not.Null);
    Assert.That(device.HidDevice, Is.Not.Null);
    Assert.That(device.FirmwareRevision, Is.Not.Null);
    Assert.That(device.HardwareRevision, Is.Not.Null);
    Assert.That(device.ManufacturerDescriptor, Is.Not.Null);
    Assert.That(device.ProductDescriptor, Is.Not.Null);
    Assert.That(device.SerialNumberDescriptor, Is.Not.Null);
    Assert.That(device.ChipFactorySerialNumber, Is.Not.Null);

    Assert.That(device.FirmwareRevision, Is.EqualTo("1.2"), nameof(device.FirmwareRevision));
    Assert.That(device.HardwareRevision, Is.EqualTo("A.6"), nameof(device.HardwareRevision));
    Assert.That(device.ManufacturerDescriptor, Is.EqualTo("Microchip Technology Inc."), nameof(device.ManufacturerDescriptor));
    Assert.That(device.ProductDescriptor, Is.EqualTo("MCP2221 USB-I2C/UART Combo"), nameof(device.ProductDescriptor));
    Assert.That(device.SerialNumberDescriptor, Is.EqualTo("XXXXXXXXXX"), nameof(device.SerialNumberDescriptor));
    Assert.That(device.ChipFactorySerialNumber, Is.EqualTo("01234567"), nameof(device.ChipFactorySerialNumber));
  }

  [Test]
  public void OpenAsync_ArgumentNull()
    => Assert.That(async () => await MCP2221.OpenAsync((Func<IUsbHidDevice>)null!), Throws.ArgumentNullException);

  [Test]
  public void OpenAsync_CreateDeviceReturnNull()
    => Assert.That(async () => await MCP2221.OpenAsync(() => (IUsbHidDevice)null!), Throws.TypeOf<DeviceNotFoundException>());



  [Test] public async Task Dispose() => await TestDispose(d => { d.Dispose(); return Task.CompletedTask; });

  [Test] public async Task DisposeAsync() => await TestDispose(async d => await d.DisposeAsync());

  private async Task TestDispose(Func<MCP2221, Task> disposeAction)
  {
    await using var device = await MCP2221.OpenAsync(CreatePreudoDevice);

    Assert.That(() => Assert.That(device.HidDevice, Is.Not.Null), Throws.Nothing);

    Assert.That(() => Assert.That(device.HardwareRevision, Is.Not.Null), Throws.Nothing);
    Assert.That(() => Assert.That(device.FirmwareRevision, Is.Not.Null), Throws.Nothing);
    Assert.That(() => Assert.That(device.ManufacturerDescriptor, Is.Not.Null), Throws.Nothing);
    Assert.That(() => Assert.That(device.ProductDescriptor, Is.Not.Null), Throws.Nothing);
    Assert.That(() => Assert.That(device.SerialNumberDescriptor, Is.Not.Null), Throws.Nothing);
    Assert.That(() => Assert.That(device.ChipFactorySerialNumber, Is.Not.Null), Throws.Nothing);
    Assert.That(() => Assert.That(device.GPs, Is.Not.Null), Throws.Nothing);
    Assert.That(() => Assert.That(device.GP0, Is.Not.Null), Throws.Nothing);
    Assert.That(() => Assert.That(device.GP1, Is.Not.Null), Throws.Nothing);
    Assert.That(() => Assert.That(device.GP2, Is.Not.Null), Throws.Nothing);
    Assert.That(() => Assert.That(device.GP3, Is.Not.Null), Throws.Nothing);
    Assert.That(() => Assert.That(device.I2C, Is.Not.Null), Throws.Nothing);

    await disposeAction(device);

    Assert.That(() => Assert.That(device.HidDevice, Is.Not.Null), Throws.TypeOf<ObjectDisposedException>());
    Assert.That(async () => await device.GP0.SetValueAsync(default), Throws.TypeOf<ObjectDisposedException>());
    Assert.That(async () => await device.GP0.GetValueAsync(), Throws.TypeOf<ObjectDisposedException>());
    Assert.That(async () => await device.I2C.WriteAsync(default, default), Throws.TypeOf<ObjectDisposedException>());
    Assert.That(async () => await device.I2C.ReadAsync(default, default), Throws.TypeOf<ObjectDisposedException>());

    Assert.That(() => Assert.That(device.HardwareRevision, Is.Not.Null), Throws.Nothing);
    Assert.That(() => Assert.That(device.FirmwareRevision, Is.Not.Null), Throws.Nothing);
    Assert.That(() => Assert.That(device.ManufacturerDescriptor, Is.Not.Null), Throws.Nothing);
    Assert.That(() => Assert.That(device.ProductDescriptor, Is.Not.Null), Throws.Nothing);
    Assert.That(() => Assert.That(device.SerialNumberDescriptor, Is.Not.Null), Throws.Nothing);
    Assert.That(() => Assert.That(device.ChipFactorySerialNumber, Is.Not.Null), Throws.Nothing);
    Assert.That(() => Assert.That(device.GPs, Is.Not.Null), Throws.Nothing);
    Assert.That(() => Assert.That(device.GP0, Is.Not.Null), Throws.Nothing);
    Assert.That(() => Assert.That(device.GP1, Is.Not.Null), Throws.Nothing);
    Assert.That(() => Assert.That(device.GP2, Is.Not.Null), Throws.Nothing);
    Assert.That(() => Assert.That(device.GP3, Is.Not.Null), Throws.Nothing);
    Assert.That(() => Assert.That(device.I2C, Is.Not.Null), Throws.Nothing);

    Assert.That(async () => await disposeAction(device), Throws.Nothing, "dispose again");
  }
}
