// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT

using System;
using NUnit.Framework;

namespace Smdn.Devices.MCP2221;

[TestFixture]
public class I2CAddressTests {
  [Test] public void Constants_Zero() => Assert.AreEqual((byte)I2CAddress.Zero, 0x00);
  [Test] public void Constants_DeviceMinValue() => Assert.AreEqual((byte)I2CAddress.DeviceMinValue, 0x08);
  [Test] public void Constants_DeviceMaxValue() => Assert.AreEqual((byte)I2CAddress.DeviceMaxValue, 0x77);

  [TestCase(0x08)]
  [TestCase(0x10)]
  [TestCase(0x70)]
  [TestCase(0x77)]
  public void Construct_FromAddress(int address)
  {
    Assert.DoesNotThrow(() => new I2CAddress(address));
    Assert.AreEqual((byte)new I2CAddress(address), address);
  }

  [TestCase(0x00)]
  [TestCase(0x07)]
  [TestCase(0x78)]
  [TestCase(0xFF)]
  [TestCase(0x100)]
  public void Construct_FromAddress_ArgumentOutOfRange(int address)
    => Assert.Throws<ArgumentOutOfRangeException>(() => new I2CAddress(address));

  [TestCase(0b_0_0001_000, 0b000, 0b_0_0001_000)]
  [TestCase(0b_0_0001_000, 0b111, 0b_0_0001_111)]
  [TestCase(0b_0_0001_111, 0b000, 0b_0_0001_000)]
  [TestCase(0b_0_1110_000, 0b000, 0b_0_1110_000)]
  [TestCase(0b_0_1110_000, 0b111, 0b_0_1110_111)]
  [TestCase(0b_0_1110_111, 0b000, 0b_0_1110_000)]
  public void Construct_FromDeviceAndHardwareAddressBits(int deviceBits, int hardwareBits, byte expectedAddress)
    => Assert.AreEqual(new I2CAddress(deviceBits, hardwareBits), new I2CAddress(expectedAddress));

  [TestCase(-1, 0)]
  [TestCase(0b_0_0000_000, 0b000)]
  [TestCase(0b_0_0000_000, 0b111)]
  [TestCase(0b_0_0000_000, 0b1000)]
  [TestCase(0b_0_1111_000, 0b000)]
  [TestCase(0b_0_1111_000, 0b111)]
  [TestCase(0b_0_1111_000, 0b1000)]
  public void Construct_FromDeviceAndHardwareAddressBits_DeviceBitsArgumentOutOfRange(int deviceBits, int hardwareBits)
  {
    var ex = Assert.Throws<ArgumentOutOfRangeException>(() => new I2CAddress(deviceBits, hardwareBits));

    Assert.That(ex.ParamName, Does.Contain("device").IgnoreCase);
  }

  [TestCase(0b_0_0001_000, -1)]
  [TestCase(0b_0_0001_000, 0b1000)]
  [TestCase(0b_0_1110_000, -1)]
  [TestCase(0b_0_1110_000, 0b1000)]
  public void Construct_FromDeviceAndHardwareAddressBits_HardwareBitsArgumentOutOfRange(int deviceBits, int hardwareBits)
  {
    var ex = Assert.Throws<ArgumentOutOfRangeException>(() => new I2CAddress(deviceBits, hardwareBits));

    Assert.That(ex.ParamName, Does.Contain("hardware").IgnoreCase);
  }



  [TestCase(0x10, 0x10, true)]
  [TestCase(0x10, 0x11, false)]
  public void Equals_ToI2CAddress(int address, int addressOther, bool expectedValue)
    => Assert.AreEqual((new I2CAddress(address)).Equals(new I2CAddress(addressOther)), expectedValue);

  [TestCase(0x10, 0x10, true)]
  [TestCase(0x10, 0x11, false)]
  public void Equals_ToInt(int address, int addressOther, bool expectedValue)
    => Assert.AreEqual((new I2CAddress(address)).Equals(addressOther), expectedValue);

  [TestCase(0x10, 0x10, true)]
  [TestCase(0x10, 0x11, false)]
  public void Equals_ToByte(int address, byte addressOther, bool expectedValue)
    => Assert.AreEqual((new I2CAddress(address)).Equals(addressOther), expectedValue);

  [Test]
  public void Equals_ToObject()
  {
    var address = new I2CAddress(0x10);

    object val0_0 = new I2CAddress(0x10); Assert.IsTrue(address.Equals(val0_0));
    object val0_1 = new I2CAddress(0x11); Assert.IsFalse(address.Equals(val0_1));
    object val1_0 = (int)0x10; Assert.IsTrue(address.Equals(val1_0));
    object val1_1 = (int)0x11; Assert.IsFalse(address.Equals(val1_1));
    object val2_0 = (byte)0x10; Assert.IsTrue(address.Equals(val2_0));
    object val2_1 = (byte)0x11; Assert.IsFalse(address.Equals(val2_1));
    object val3 = null; Assert.IsFalse(address.Equals(val3));
    object val4 = 16.0; Assert.IsFalse(address.Equals(val4));
  }

  [TestCase(0x10, 0x10, true)]
  [TestCase(0x10, 0x11, false)]
  public void OperatorEquality(int address, int addressOther, bool expectedValue)
    => Assert.AreEqual(new I2CAddress(address) == new I2CAddress(addressOther), expectedValue);

  [TestCase(0x10, 0x10, false)]
  [TestCase(0x10, 0x11, true)]
  public void OperatorInequality(int address, int addressOther, bool expectedValue)
    => Assert.AreEqual(new I2CAddress(address) != new I2CAddress(addressOther), expectedValue);



  [TestCase(0x10, 0x10, 0)]
  [TestCase(0x10, 0x11, -1)]
  [TestCase(0x11, 0x10, 1)]
  public void CompareTo_I2CAddress(int address, int addressOther, int expectedValue)
    => Assert.AreEqual((new I2CAddress(address)).CompareTo(new I2CAddress(addressOther)), expectedValue);



  [TestCase(0x08, 0x08)]
  [TestCase(0x10, 0x10)]
  [TestCase(0x70, 0x70)]
  [TestCase(0x77, 0x77)]
  public void ExplicitTypeConversion_ToByte(int address, byte expectedValue)
    => Assert.AreEqual((byte)new I2CAddress(address), expectedValue);

  [TestCase(0x08, 0x08)]
  [TestCase(0x10, 0x10)]
  [TestCase(0x70, 0x70)]
  [TestCase(0x77, 0x77)]
  public void ExplicitTypeConversion_ToByte(int address, int expectedValue)
    => Assert.AreEqual((int)new I2CAddress(address), expectedValue);

  [TestCase(0x08)]
  [TestCase(0x10)]
  [TestCase(0x70)]
  [TestCase(0x77)]
  public void ImplicitTypeConversion_FromByte(byte address)
  {
    I2CAddress addr = address;

    Assert.AreEqual((byte)addr, address);
  }
}
