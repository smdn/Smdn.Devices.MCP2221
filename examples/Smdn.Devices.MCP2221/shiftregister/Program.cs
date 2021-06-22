// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT

using System;
using System.Device.Gpio;
using System.Threading.Tasks;

using Smdn.Devices.MCP2221;

using var device = MCP2221.Open();

// configure GP0-GP3 as GPIO output
device.GP0.ConfigureAsGPIO(PinMode.Output);
device.GP1.ConfigureAsGPIO(PinMode.Output);
device.GP2.ConfigureAsGPIO(PinMode.Output);

// construct shift register
var shiftRegister = new ShiftRegister(
  gpioLatch: device.GP0,
  gpioClock: device.GP1,
  gpioData: device.GP2
);

const int maxBits = 16;

for (;;) {
  for (var shift = 0; shift < maxBits; shift++) {
    var data = 0b1u << shift;

    Console.WriteLine($"0b_{Convert.ToString(data, 2)}");

    await shiftRegister.WriteAsync(data, Endianness.BigEndian, BitOrder.HSBFirst);

    await Task.Delay(100);
  }
}
