// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT

using System;
using System.Collections.Generic;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

using Smdn.Devices.MCP2221;

var services = new ServiceCollection();

services.AddLogging(
  builder => builder
    .AddSimpleConsole(static options => options.SingleLine = true)
    .AddFilter(static level => LogLevel.Trace <= level)
);

// This works only if you use LibUsbDotNet
Smdn.Devices.UsbHid.Log.NativeLibraryLogLevel = LogLevel.Trace;

await using var device = await MCP2221.OpenAsync(services.BuildServiceProvider());

await device.I2C.ScanBusAsync();