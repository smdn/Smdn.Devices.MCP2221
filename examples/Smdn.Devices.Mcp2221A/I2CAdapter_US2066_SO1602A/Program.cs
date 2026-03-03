// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT

using System;
using System.Threading.Tasks;
using System.Reflection;

using Microsoft.Extensions.DependencyInjection;

using Smdn.Devices.Mcp2221A;
using Smdn.Devices.Mcp2221A.Peripherals.I2c;
using Smdn.Devices.US2066;
using Smdn.IO.UsbHid.DependencyInjection;

var services = new ServiceCollection();

services.AddHidSharpUsbHid();

using var serviceProvider = services.BuildServiceProvider();

await using var device = await Mcp2221A.CreateAsync(serviceProvider);

await device.GP3.ConfigureAsLedI2cAsync();

device.I2c.BusSpeed = I2cBusSpeed.FastMode;

using var display = SO1602A.Create(
  device.I2c.CreateI2cDeviceAdapter(SO1602A.DefaultI2CAddress)
);

// write string and display it
display.WriteLine("Hello, MCP2221A");
display.WriteLine($"{device.GetType().Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion}");
