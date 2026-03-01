// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT

using System;
using System.Threading.Tasks;
using System.Reflection;

using Microsoft.Extensions.DependencyInjection;

using Smdn.Devices.MCP2221;
using Smdn.Devices.MCP2221.GpioAdapter;
using Smdn.IO.UsbHid.DependencyInjection;

using Iot.Device.Display;

var services = new ServiceCollection();

services.AddHidSharpUsbHid();

using var serviceProvider = services.BuildServiceProvider();

await using var device = await MCP2221.CreateAsync(serviceProvider);

await device.GP3.ConfigureAsLEDI2CAsync();

MCP2221I2cDevice[] i2cDevices = {
  new(device.I2C, Ht16k33.DefaultI2cAddress | 0b_000),
  new(device.I2C, Ht16k33.DefaultI2cAddress | 0b_001),
};

i2cDevices[0].BusSpeed = I2CBusSpeed.Default;
i2cDevices[1].BusSpeed = I2CBusSpeed.Default;

// If an I2CCommandException is thrown when using the
// HidSharp backend, try the following configuration:
// i2cDevices[0].BusSpeed = I2CBusSpeed.FastMode;
// i2cDevices[1].BusSpeed = I2CBusSpeed.FastMode;

FourDigitFourteenSegmentDisplay[] displays = {
  new(i2cDevices[0]) { Brightness = Ht16k33.MaxBrightness, BufferingEnabled = true },
  new(i2cDevices[1]) { Brightness = Ht16k33.MaxBrightness, BufferingEnabled = true },
};

// clear display
displays[0].Clear();
displays[1].Clear();

displays[0].Flush();
displays[1].Flush();

// write string and display it
const int numOfDigits = FourDigitFourteenSegmentDisplay.NumberOfDigits;
var str = $"    Hello, MCP2221/MCP2221A. {device.GetType().Assembly.GetName().Name} {device.GetType().Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion}    ";

Console.WriteLine(str);

for (var i = 0; i < str.Length; i++) {
  displays[0].Clear();
  displays[1].Clear();

  if (i < str.Length)
    displays[0].Write(str.AsSpan(i));

  if (i + numOfDigits < str.Length)
    displays[1].Write(str.AsSpan(i + numOfDigits));

  displays[0].Flush();
  displays[1].Flush();

  await Task.Delay(200);
}
