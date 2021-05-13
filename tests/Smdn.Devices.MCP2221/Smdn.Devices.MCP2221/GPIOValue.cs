// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT

using System;
using NUnit.Framework;

namespace Smdn.Devices.MCP2221 {
  [TestFixture]
  public class GPIOValueTests {
    [Test]
    public void Constants_Default()
    {
      Assert.AreEqual(GPIOValue.Default, GPIOValue.Low);
    }

    [Test]
    public void Constants_Low()
    {
      Assert.AreEqual((byte)GPIOValue.Low, 0);
      Assert.AreEqual((int)GPIOValue.Low, 0);
      Assert.AreEqual((bool)GPIOValue.Low, false);
      Assert.AreEqual((GPIOLevel)GPIOValue.Low, GPIOLevel.Low);
    }

    [Test]
    public void Constants_High()
    {
      Assert.AreEqual((byte)GPIOValue.High, 1);
      Assert.AreEqual((int)GPIOValue.High, 1);
      Assert.AreEqual((bool)GPIOValue.High, true);
      Assert.AreEqual((GPIOLevel)GPIOValue.High, GPIOLevel.High);
    }

    [TestCase(0, GPIOLevel.Low)]
    [TestCase(1, GPIOLevel.High)]
    [TestCase(-1, GPIOLevel.High)]
    public void Construct_FromInt32(int val, GPIOLevel expectedLevel)
      => Assert.AreEqual((GPIOLevel)new GPIOValue(val), expectedLevel);

    [TestCase(0x00, GPIOLevel.Low)]
    [TestCase(0x01, GPIOLevel.High)]
    [TestCase(0xFF, GPIOLevel.High)]
    public void Construct_FromByte(byte val, GPIOLevel expectedLevel)
      => Assert.AreEqual((GPIOLevel)new GPIOValue(val), expectedLevel);

    [TestCase(false, GPIOLevel.Low)]
    [TestCase(true, GPIOLevel.High)]
    public void Construct_FromBoolean(bool val, GPIOLevel expectedLevel)
      => Assert.AreEqual((GPIOLevel)new GPIOValue(val), expectedLevel);

    [TestCase(GPIOLevel.Low, GPIOLevel.Low)]
    [TestCase(GPIOLevel.High, GPIOLevel.High)]
    public void Construct_FromGPIOLevel(GPIOLevel val, GPIOLevel expectedLevel)
      => Assert.AreEqual((GPIOLevel)new GPIOValue(val), expectedLevel);

    [TestCase((GPIOLevel)0x02)]
    [TestCase((GPIOLevel)0xFF)]
    public void Construct_FromGPIOLevel_ArgumentOutOfRange(GPIOLevel val)
      => Assert.Throws<ArgumentOutOfRangeException>(() => new GPIOValue(val));



    [Test]
    public void IsHigh()
    {
      Assert.IsTrue(GPIOValue.High.IsHigh);
      Assert.IsFalse(GPIOValue.Low.IsHigh);
    }

    [Test]
    public void IsLow()
    {
      Assert.IsTrue(GPIOValue.Low.IsLow);
      Assert.IsFalse(GPIOValue.High.IsLow);
    }



    [TestCase(GPIOLevel.Low, 0)]
    [TestCase(GPIOLevel.High, 1)]
    public void ExplicitTypeConversion_ToByte(GPIOLevel level, byte expectedValue)
      => Assert.AreEqual((byte)new GPIOValue(level), expectedValue);

    [TestCase(GPIOLevel.Low, 0)]
    [TestCase(GPIOLevel.High, 1)]
    public void ExplicitTypeConversion_ToInt32(GPIOLevel level, int expectedValue)
      => Assert.AreEqual((int)new GPIOValue(level), expectedValue);

    [TestCase(GPIOLevel.Low, false)]
    [TestCase(GPIOLevel.High, true)]
    public void ExplicitTypeConversion_ToBoolean(GPIOLevel level, bool expectedValue)
      => Assert.AreEqual((bool)new GPIOValue(level), expectedValue);

    [TestCase(GPIOLevel.Low, GPIOLevel.Low)]
    [TestCase(GPIOLevel.High, GPIOLevel.High)]
    public void ExplicitTypeConversion_ToGPIOLevel(GPIOLevel level, GPIOLevel expectedValue)
      => Assert.AreEqual((GPIOLevel)new GPIOValue(level), expectedValue);



    [TestCase(0x00, GPIOLevel.Low)]
    [TestCase(0x01, GPIOLevel.High)]
    [TestCase(0xFF, GPIOLevel.High)]
    public void ImplicitTypeConversion_FromByte(byte val, GPIOLevel expectedLevel)
    {
      GPIOValue v = val;

      Assert.AreEqual(v, expectedLevel == GPIOLevel.Low ? GPIOValue.Low : GPIOValue.High);
    }

    [TestCase(0, GPIOLevel.Low)]
    [TestCase(1, GPIOLevel.High)]
    [TestCase(2, GPIOLevel.High)]
    [TestCase(-1, GPIOLevel.High)]
    public void ImplicitTypeConversion_FromInt32(int val, GPIOLevel expectedLevel)
    {
      GPIOValue v = val;

      Assert.AreEqual(v, expectedLevel == GPIOLevel.Low ? GPIOValue.Low : GPIOValue.High);
    }

    [TestCase(false, GPIOLevel.Low)]
    [TestCase(true, GPIOLevel.High)]
    public void ImplicitTypeConversion_FromBoolean(bool val, GPIOLevel expectedLevel)
    {
      GPIOValue v = val;

      Assert.AreEqual(v, expectedLevel == GPIOLevel.Low ? GPIOValue.Low : GPIOValue.High);
    }

    [TestCase(GPIOLevel.Low, GPIOLevel.Low)]
    [TestCase(GPIOLevel.High, GPIOLevel.High)]
    [TestCase((GPIOLevel)0x02, GPIOLevel.High)]
    [TestCase((GPIOLevel)0xFF, GPIOLevel.High)]
    public void ImplicitTypeConversion_FromGPIOLevel(GPIOLevel val, GPIOLevel expectedLevel)
     {
      GPIOValue v = val;

      Assert.AreEqual(v, expectedLevel == GPIOLevel.Low ? GPIOValue.Low : GPIOValue.High);
    }
  }
}