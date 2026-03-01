// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT

using System;
using System.Collections.Generic;

using Microsoft.Extensions.DependencyInjection;

using Smdn.Devices.MCP2221;
using Smdn.IO.UsbHid.DependencyInjection;

var services = new ServiceCollection();

services.AddHidSharpUsbHid();

using var serviceProvider = services.BuildServiceProvider();

await using var device = await MCP2221.CreateAsync(serviceProvider);

device.I2C.BusSpeed = I2CBusSpeed.Default;

var initialCursorPosition = (left: Console.CursorLeft, top: Console.CursorTop);

var scanBusProgress = new Progress<I2CScanBusProgress>(progress => {
  Console.SetCursorPosition(initialCursorPosition.left, initialCursorPosition.top);
  Console.Write(
    "Scanning 0x{0}: Min=0x{1} {3}{4} Max=0x{2}",
    progress.ScanningAddress,
    progress.AddressRangeMin,
    progress.AddressRangeMax,
    new string('|', progress.ProgressInPercent),
    new string('-', 100 - progress.ProgressInPercent)
  );

  if (progress.ProgressInPercent == 100)
    Console.WriteLine();
});

I2CAddress addressRangeMin = I2CAddress.DeviceMinValue;
I2CAddress addressRangeMax = I2CAddress.DeviceMaxValue;
// I2CAddress addressRangeMin = 0x20;
// I2CAddress addressRangeMax = 0x27;

var (writeAddressSet, readAddressSet) = await device.I2C.ScanBusAsync(addressRangeMin, addressRangeMax, scanBusProgress);

foreach (var writeRead in new[] {
  new {Header = "[I2C write]", AddressSet = writeAddressSet},
  new {Header = "[I2C read]", AddressSet = readAddressSet},
}) {
  Console.WriteLine(writeRead.Header);

  Console.Write("    |");
  for (var hcol = 0x00; hcol <= 0x0F; hcol += 0x01) {
    Console.Write($"{hcol,3:X1} ");
  }
  Console.WriteLine();

  Console.WriteLine(new string('-', 72));

  for (var row = 0x00; row <= 0x70; row += 0x10) {
    Console.Write($"0x{row:X2}|");
    for (var col = 0x00; col <= 0x0F; col += 0x01) {
      var address = row | col;

      if (writeRead.AddressSet.Contains((I2CAddress)address))
        Console.Write($"{address,3:X2} ");
      else
        Console.Write($"{("--"),3} ");
    }
    Console.WriteLine();
  }

  Console.WriteLine();
}

