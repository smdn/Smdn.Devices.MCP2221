// Smdn.Devices.MCP2221.GpioAdapter.dll (Smdn.Devices.MCP2221.GpioAdapter)
//   Name: Smdn.Devices.MCP2221.GpioAdapter
//   AssemblyVersion: 0.9.0.0
//   InformationalVersion: 0.9 (netstandard2.1)
//   TargetFramework: .NETStandard,Version=v2.1
//   Configuration: Release

using System;
using System.Device.I2c;
using Smdn.Devices.MCP2221;

namespace Smdn.Devices.MCP2221.GpioAdapter {
  public class MCP2221I2cDevice : I2cDevice {
    public MCP2221I2cDevice(MCP2221.I2CFunctionality i2cBus, I2CAddress i2cDeviceAddress) {}

    public I2CBusSpeed BusSpeed { get; set; }
    public override I2cConnectionSettings ConnectionSettings { get; }

    public override void Read(Span<byte> buffer) {}
    public override byte ReadByte() {}
    public override void Write(ReadOnlySpan<byte> buffer) {}
    public override void WriteByte(byte @value) {}
    public override void WriteRead(ReadOnlySpan<byte> writeBuffer, Span<byte> readBuffer) {}
  }
}

