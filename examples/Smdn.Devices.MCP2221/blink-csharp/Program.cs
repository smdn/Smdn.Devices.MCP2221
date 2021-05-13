// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT

using System;
using System.Threading;

using Smdn.Devices.MCP2221;

using var device = MCP2221.Open();

Console.WriteLine("[MCP2221 Device informations]");
Console.WriteLine($"Release number: {device.HidDevice.ReleaseNumber}");
Console.WriteLine($"Serial number: {device.HidDevice.SerialNumber ?? "(no serial number)"}");
Console.WriteLine($"Device path: {device.HidDevice.DevicePath}");
Console.WriteLine($"File system name: {device.HidDevice.FileSystemName}");
Console.WriteLine($"Chip factory serial number: {device.ChipFactorySerialNumber}");
Console.WriteLine($"Hardware revision: {device.HardwareRevision}");
Console.WriteLine($"Firmware revision: {device.FirmwareRevision}");
Console.WriteLine();

// configure GP0-GP3 as GPIO output
device.GP0.ConfigureAsGPIO(GPIODirection.Output);
device.GP1.ConfigureAsGPIO(GPIODirection.Output);
device.GP2.ConfigureAsGPIO(GPIODirection.Output);
device.GP3.ConfigureAsGPIO(GPIODirection.Output, initialValue: GPIOValue.Low); // initial value also can be specified

// set GPIO pin values
Console.WriteLine("set all GPs HIGH");

device.GPs[0].SetValue(1); // set GP0 to HIGH with integer value (0 = LOW, any other value = HIGH)

device.GPs[1].SetValue(true); // set GP1 to HIGH with boolean value

device.GP2.SetValue(GPIOLevel.High); // set GP2 to HIGH with GPIOLevel enum value

GPIOValue gp3Value = (GPIOValue)1;

device.GP3.SetValue(gp3Value); // set GP3 to HIGH with struct GPIOValue

Thread.Sleep(1000);

Console.WriteLine("set all GPs LOW");

// GP0-GP3 also can be accessed via `GPs` read-only collection property
foreach (var gp in device.GPs) {
  gp.SetValue(GPIOValue.Low);
}

Thread.Sleep(1000);

// blink GP0-GP3
foreach (var gp in device.GPs) {
  Console.WriteLine($"blink {gp.PinDesignation}");

  for (var n = 0; n < 10; n++) {
    gp.SetValue(false);
    Thread.Sleep(100);

    gp.SetValue(true);
    Thread.Sleep(100);
  }
}