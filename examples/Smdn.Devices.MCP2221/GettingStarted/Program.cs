using System.Device.Gpio;

using Smdn.Devices.MCP2221;

// Find and open the first MCP2221 device connected to the USB port.
using var device = MCP2221.Open();

// Configure the GP pins (GP0-GP3) as GPIO output.
device.GP0.ConfigureAsGPIO(PinMode.Output);
device.GP1.ConfigureAsGPIO(PinMode.Output);
device.GP2.ConfigureAsGPIO(PinMode.Output);
device.GP3.ConfigureAsGPIO(PinMode.Output);

// Blink the configured GPIO pins.
//
// This example assumes an LED is connected to each pin.
// See this code in action in the YouTube video:
// https://www.youtube.com/watch?v=MnIunESm71E
foreach (var gp in device.GPs) {
  Console.WriteLine($"Blinking {gp.PinDesignation}");

  for (var n = 0; n < 10; n++) {
    // Set the pin output to Low (logic 0)
    gp.SetValue(false);
    Thread.Sleep(100);

    // Set the pin output to High (logic 0)
    gp.SetValue(true);
    Thread.Sleep(100);
  }
}
