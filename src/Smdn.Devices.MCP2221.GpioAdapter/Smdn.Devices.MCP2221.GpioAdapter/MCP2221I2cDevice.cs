// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT

using System;
using System.Device.I2c;

using Smdn.Devices.MCP2221;

namespace Smdn.Devices.MCP2221.GpioAdapter;

[CLSCompliant(false)]
public class MCP2221I2cDevice : I2cDevice {
  private readonly MCP2221.I2CFunctionality bus;
  private readonly I2CAddress address;
  public override I2cConnectionSettings ConnectionSettings { get; }

  public I2CBusSpeed BusSpeed {
    get => bus.BusSpeed;
    set => bus.BusSpeed = value;
  }

  public MCP2221I2cDevice(MCP2221.I2CFunctionality i2cBus, I2CAddress i2cDeviceAddress)
  {
    this.bus = i2cBus ?? throw new ArgumentNullException(nameof(i2cBus));
    this.address = i2cDeviceAddress;
    this.ConnectionSettings = new I2cConnectionSettings(busId: 0, deviceAddress: (int)address);
  }

  public unsafe override byte ReadByte()
  {
    Span<byte> buffer = stackalloc byte[1];

    Read(buffer);

    return buffer[0];
  }

  public override void Read(Span<byte> buffer)
  {
    bus.Read(address, buffer);
  }

  public unsafe override void WriteByte(byte value)
  {
    Span<byte> buffer = stackalloc byte[1] {value};

    Write(buffer);
  }

  public override void Write(ReadOnlySpan<byte> buffer)
  {
    bus.Write(address, buffer);
  }

  public override void WriteRead(ReadOnlySpan<byte> writeBuffer, Span<byte> readBuffer)
  {
    Write(writeBuffer);
    Read(readBuffer);
  }
}
