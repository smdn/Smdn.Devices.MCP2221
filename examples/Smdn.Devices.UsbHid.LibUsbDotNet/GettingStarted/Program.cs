using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Smdn.Devices.UsbHid;
using Smdn.Devices.UsbHid.DependencyInjection;

var services = new ServiceCollection();

services.AddLibUsbDotNetUsbHid( // add singleton
  serviceKey: "service 1",
  configure: static (builder, options) => {
    options.DebugLevel = LogLevel.Information;
  }
);
services.AddLogging(
  builder => builder
    .AddSimpleConsole(static options => options.SingleLine = true)
    .AddFilter(static level => LogLevel.Trace <= level)
);

var serviceProvider = services.BuildServiceProvider();

var usbHidService = serviceProvider.GetRequiredKeyedService<IUsbHidService>(serviceKey: "service 1");

const int DeviceVendorID = 0x04d8;
const int DeviceProductID = 0x00dd;

foreach (var device in usbHidService.GetDevices(DeviceVendorID, DeviceProductID)) {
  Console.WriteLine($"{device.VendorId:X4}:{device.ProductId:X4}:");

  if (device.TryGetProductName(out var productName))
    Console.WriteLine($"  {productName}");

  if (device.TryGetManufacturer(out var manufacturer))
    Console.WriteLine($"  {manufacturer}");

  if (device.TryGetSerialNumber(out var serialNumber))
    Console.WriteLine($"  {serialNumber}");

  if (device.TryGetDeviceIdentifier(out var deviceIdentifier))
    Console.WriteLine($"  {deviceIdentifier}");

  using var endPoint = device.OpenEndPoint(shouldDisposeDevice: true);

  endPoint.Dispose();
}
