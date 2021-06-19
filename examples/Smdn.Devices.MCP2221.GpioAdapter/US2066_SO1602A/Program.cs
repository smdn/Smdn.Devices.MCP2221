// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT

using System;
using System.Threading.Tasks;
using System.Reflection;

using Smdn.Devices.MCP2221;
using Smdn.Devices.MCP2221.GpioAdapter;
using Smdn.Devices.US2066;

await using var device = await MCP2221.OpenAsync();

await device.GP3.ConfigureAsLEDI2CAsync();

using var display = SO1602A.Create(
  new MCP2221I2cDevice(device.I2C, SO1602A.DefaultI2CAddress) {
    BusSpeed = I2CBusSpeed.FastMode
  }
);

// write string and display it
display.WriteLine("Hello, MCP2221A");
display.WriteLine($"{device.GetType().Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion}");
