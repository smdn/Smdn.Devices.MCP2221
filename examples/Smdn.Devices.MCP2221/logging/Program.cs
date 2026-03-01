// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT

using System;
using System.Collections.Generic;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

using Smdn.Devices.MCP2221;
using Smdn.IO.UsbHid.DependencyInjection;

var services = new ServiceCollection();

services.AddHidSharpUsbHid();

#if false
// When using LibUsbDotNet as the backend, you can control the level of logs
// output by libusb to stdout and stderr using DebugLevel.
services.AddLibUsbDotNetUsbHid(
  configure: static (builder, options) => {
    options.DebugLevel = LogLevel.Information;
  }
);

services.AddLibUsbDotNetV3UsbHid(
  configure: static (builder, options) => {
    options.DebugLevel = LogLevel.Information;
  }
);
#endif

services.AddLogging(
  builder => builder
    .AddSimpleConsole(static options => options.SingleLine = true)
    .AddFilter(static level => LogLevel.Trace <= level)
);

using var serviceProvider = services.BuildServiceProvider();

await using var device = await MCP2221.CreateAsync(serviceProvider);

await device.I2C.ScanBusAsync();
