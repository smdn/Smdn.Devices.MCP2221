// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT

using System;
using System.Collections.Generic;

namespace Smdn.Devices.MCP2221;

#pragma warning disable IDE0055
public readonly struct I2CAddress :
  IEquatable<I2CAddress>,
  IEquatable<int>,
  IEquatable<byte>,
  IComparable<I2CAddress>
{
#pragma warning disable IDE0055
  public static readonly I2CAddress Zero = default;
  public static readonly I2CAddress DeviceMinValue = new((byte)0b_0_0001_000u);
  public static readonly I2CAddress DeviceMaxValue = new((byte)0b_0_1110_111u);

  private readonly byte address;

  private static byte ValidateDeviceAddressBits(uint address, string paramName)
  {
    const uint addressRangeLower = 0b_0_0001_000u;
    const uint addressRangeUpper = 0b_0_1110_000u;

    var actualValue = address;

    address &= 0b_0_1111_000u;

    if (address is not (>= addressRangeLower and <= addressRangeUpper))
      throw new ArgumentOutOfRangeException(paramName, actualValue, $"must be in range between {addressRangeLower}(0x{addressRangeLower:X2}) and {addressRangeUpper}(0x{addressRangeUpper:X2})");

    return (byte)address;
  }

  private static byte ValidateHardwareAddressBits(uint address, string paramName)
  {
    const uint addressRangeLower = 0b_0_0000_000u;
    const uint addressRangeUpper = 0b_0_0000_111u;

    if (address is not (>= addressRangeLower and <= addressRangeUpper))
      throw new ArgumentOutOfRangeException(paramName, address, $"must be in range between {addressRangeLower}(0x{addressRangeLower:X2}) and {addressRangeUpper}(0x{addressRangeUpper:X2})");

    return (byte)(address & 0b0_0000_111u);
  }

  public I2CAddress(int deviceAddressBits, int hardwareAddressBits)
    : this(
      (byte)(
        ValidateDeviceAddressBits((uint)deviceAddressBits, nameof(deviceAddressBits)) |
        ValidateHardwareAddressBits((uint)hardwareAddressBits, nameof(hardwareAddressBits))
      )
    )
  {
  }

  private static byte ValidateAddress(uint address, string paramName)
  {
    if (!(DeviceMinValue.address <= address && address <= DeviceMaxValue.address))
      throw new ArgumentOutOfRangeException(paramName, address, $"must be in range between {DeviceMinValue.address}(0x{DeviceMinValue.address:X2}) and {DeviceMaxValue.address}(0x{DeviceMaxValue.address:X2})");

    return (byte)(address & 0b_0_1111_111u);
  }

  public I2CAddress(int address)
    : this(ValidateAddress((uint)address, nameof(address)))
  {
  }

  private I2CAddress(byte address)
  {
    this.address = address;
  }

  public bool Equals(I2CAddress other) => this.address == other.address;
  public bool Equals(int other) => this.address == other;
  public bool Equals(byte other) => this.address == other;
  public override bool Equals(object obj) => obj switch {
    null => false,
    I2CAddress other => this.Equals(other),
    int other => this.Equals(other),
    byte other => this.Equals(other),
    _ => false,
  };
  public override int GetHashCode() => address.GetHashCode();
  public static bool operator ==(I2CAddress x, I2CAddress y) => x.Equals(y);
  public static bool operator !=(I2CAddress x, I2CAddress y) => !x.Equals(y);

  public int CompareTo(I2CAddress other) => Comparer<byte>.Default.Compare(this.address, other.address);

  public static explicit operator byte(I2CAddress address) => address.address;
  public static explicit operator int(I2CAddress address) => address.address;
  public static implicit operator I2CAddress(byte address) => new(address);

  internal byte GetReadAddress() => (byte)((address << 1) | 0b_0000_0001);
  internal byte GetWriteAddress() => (byte)(address << 1);

  public override string ToString() => address.ToString("X2");
}
