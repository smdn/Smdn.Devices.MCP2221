// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT

using System;
using System.Linq;
using System.Threading;

using Smdn.Devices.MCP2221;

using var device = MCP2221.Open();

// configure GP0-GP3 as GPIO input
device.GP0.ConfigureAsGPIO(GPIODirection.Input);
device.GP1.ConfigureAsGPIO(GPIODirection.Input);
device.GP2.ConfigureAsGPIO(GPIODirection.Input);
device.GP3.ConfigureAsGPIO(GPIODirection.Input);

// read GP0 value
var gp0Val = device.GP0.GetValue();

// read GP1 value as int (0 = LOW, 1 = HIGH)
int gp1Val = (int)device.GP1.GetValue();

// read GP2 value as byte (0 = LOW, 1 = HIGH)
byte gp2Val = (byte)device.GPs[2].GetValue();

// read GP3 value as bool (false = LOW, true = HIGH)
bool gp3Val = (bool)device.GPs[3].GetValue();

// read and display GP0-GP3 pin value every 20 ms
var initialCursorPosition = (left: Console.CursorLeft, top: Console.CursorTop);

while (true) {
  Console.SetCursorPosition(initialCursorPosition.left, initialCursorPosition.top);

  Console.WriteLine(string.Join("\t", device.GPs.Select(gp => gp.PinName)));
  Console.WriteLine(string.Join("\t", device.GPs.Select(gp => gp.GetValue().IsHigh ? "H" : "L")));

  Thread.Sleep(20);
}
