// SPDX-FileCopyrightText: 2026 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;

using NUnit.Framework;

namespace Smdn.Devices.Mcp2221A.Peripherals.I2c;

[TestFixture]
public class Mcp2221AI2cDeviceTests {
  private static Mcp2221A CreateFromPseudoDevice()
  {
    var baseDevice = Mcp2221ATests.CreatePseudoDevice();

    return Mcp2221A.Create(baseDevice, shouldDisposeUsbHidDevice: true);
  }

  [TestCase(1)]
  [TestCase(100)]
  [TestCase(400)]
  [TestCase(1000)]
  [TestCase(int.MaxValue)]
  public void TransmissionSpeedInKbps(
    int newTransmissionSpeedKbps
  )
  {
    using var device = CreateFromPseudoDevice();
    using var i2cBus = device.I2c.CreateI2cBusAdapter();
    using var i2cDevice = i2cBus.CreateDevice(0x40);

    Assert.That(() => i2cDevice.TransmissionSpeedInKbps = newTransmissionSpeedKbps, Throws.Nothing);
    Assert.That(i2cDevice.TransmissionSpeedInKbps, Is.EqualTo(newTransmissionSpeedKbps));
  }

  [TestCase(0)]
  [TestCase(-1)]
  [TestCase(int.MinValue)]
  public void TransmissionSpeedKbps_OutOfRange(
    int newTransmissionSpeedKbps
  )
  {
    using var device = CreateFromPseudoDevice();
    using var i2cBus = device.I2c.CreateI2cBusAdapter();
    using var i2cDevice = i2cBus.CreateDevice(0x40);

    Assert.That(
      () => i2cDevice.TransmissionSpeedInKbps = newTransmissionSpeedKbps,
      Throws
        .TypeOf<ArgumentOutOfRangeException>()
        .With
          .Property(nameof(ArgumentOutOfRangeException.ParamName))
          .EqualTo(nameof(i2cDevice.TransmissionSpeedInKbps))
        .With
          .Property(nameof(ArgumentOutOfRangeException.ActualValue))
          .EqualTo(newTransmissionSpeedKbps)
    );
  }

  [Test]
  public void WithStandardMode()
  {
    const int InitialTransmissionSpeed = 200;
    const int ExpectedTransmissionSpeed = 100;

    using var device = CreateFromPseudoDevice();
    using var i2cBus = device.I2c.CreateI2cBusAdapter();
    using var i2cDevice = i2cBus.CreateDevice(0x40, transmissionSpeedInKbps: InitialTransmissionSpeed);

    Assert.That(i2cDevice.TransmissionSpeedInKbps, Is.EqualTo(InitialTransmissionSpeed));

    Assert.That(
      i2cDevice.WithStandardMode(),
      Is.SameAs(i2cDevice)
    );

    Assert.That(i2cDevice.TransmissionSpeedInKbps, Is.EqualTo(ExpectedTransmissionSpeed));
  }

  [Test]
  public void WithFastMode()
  {
    const int InitialTransmissionSpeed = 200;
    const int ExpectedTransmissionSpeed = 400;

    using var device = CreateFromPseudoDevice();
    using var i2cBus = device.I2c.CreateI2cBusAdapter();
    using var i2cDevice = i2cBus.CreateDevice(0x40, transmissionSpeedInKbps: InitialTransmissionSpeed);

    Assert.That(i2cDevice.TransmissionSpeedInKbps, Is.EqualTo(InitialTransmissionSpeed));

    Assert.That(
      i2cDevice.WithFastMode(),
      Is.SameAs(i2cDevice)
    );

    Assert.That(i2cDevice.TransmissionSpeedInKbps, Is.EqualTo(ExpectedTransmissionSpeed));
  }
}