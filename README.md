[![GitHub license](https://img.shields.io/github/license/smdn/Smdn.Devices.MCP2221)](https://github.com/smdn/Smdn.Devices.MCP2221/blob/main/LICENSE.txt)
[![tests/main](https://img.shields.io/github/actions/workflow/status/smdn/Smdn.Devices.MCP2221/test.yml?branch=main&label=tests%2Fmain)](https://github.com/smdn/Smdn.Devices.MCP2221/actions/workflows/test.yml)
[![CodeQL](https://github.com/smdn/Smdn.Devices.MCP2221/actions/workflows/codeql-analysis.yml/badge.svg?branch=main)](https://github.com/smdn/Smdn.Devices.MCP2221/actions/workflows/codeql-analysis.yml)

# Smdn.Devices.MCP2221
[![NuGet Smdn.Devices.MCP2221](https://img.shields.io/nuget/v/Smdn.Devices.MCP2221.svg)](https://www.nuget.org/packages/Smdn.Devices.MCP2221/)

[Smdn.Devices.MCP2221](src/Smdn.Devices.MCP2221/) is a .NET library for controlling [MCP2221](https://www.microchip.com/wwwproducts/en/MCP2221)/[MCP2221A USB2.0 to I<sup>2</sup>C/UART Protocol Converter with GPIO](https://www.microchip.com/wwwproducts/en/MCP2221A). This library enables you to control MCP2221/MCP2221A's GPIO, I<sup>2</sup>C interface, and other functionalities via USB-HID interface.

With this library, you can control I<sup>2</sup>C devices and others devices on any PCs with USB ports. It is not needed the board like the Raspberry Pi or Arduino. Also it is not required to install native device drivers on your system.

See [Smdn.Devices.MCP2221 examples](examples/Smdn.Devices.MCP2221/).

## Smdn.Devices.MCP2221.GpioAdapter
[![NuGet Smdn.Devices.MCP2221.GpioAdapter](https://img.shields.io/nuget/v/Smdn.Devices.MCP2221.GpioAdapter.svg)](https://www.nuget.org/packages/Smdn.Devices.MCP2221.GpioAdapter/)

[Smdn.Devices.MCP2221.GpioAdapter](src/Smdn.Devices.MCP2221.GpioAdapter/) provides the MCP2221/MCP2221A adapter interface for [System.Device.Gpio](https://www.nuget.org/packages/System.Device.Gpio/). This library enables you to use the many device bindings provided by [Iot.Device.Bindings](https://www.nuget.org/packages/Iot.Device.Bindings/).

See [Smdn.Devices.MCP2221.GpioAdapter examples](examples/Smdn.Devices.MCP2221.GpioAdapter/).

# Supported MCP2221/MCP2221A features
- [x] GP functionalities
  - [x] GPIO
    - [x] GPIO read/write value
    - [x] GPIO get/set direction (some methods throw NotImplementedException)
  - [ ] ADC inputs
  - [ ] DAC outputs
  - [ ] Clock output
  - [ ] Interrupt detection
  - [x] Other functionalities
    - [x] Configure GP0 as SSPND
    - [x] Configure GP0 as LED_URx
    - [x] Configure GP1 as LED_UTx
    - [x] Configure GP2 as USBCFG
    - [x] Configure GP3 as LED_I2C
    - [ ] Set initial value (Negating logic level)
- [x] I2C functionalities
  - [x] I2C read/write (7-bit address) (Transferring larger than 60 bytes have not been tested with actual device)
  - [ ] I2C read/write (10-bit address)
  - [x] configure communication speed
    - [x] Standard mode (100kbps)
    - [x] Fast mode (400kbps)
    - [x] Low speed mode (10kbps)
    - [ ] Non-standard custom speed
- [ ] Flash/SRAM functionalities
  - [ ] SRAM read/write
    - [x] GP settings
    - [ ] Chip settings
  - [ ] Flash read/write
    - [ ] GP Settings
    - [ ] Chip Settings
    - [x] USB Manufacturer Descriptor String (read only)
    - [x] USB Product Descriptor String (read only)
    - [x] USB Serial Number Descriptor String (read only)
    - [x] Chip Factory Serial Number (read only, [always returns `01234567` (issue #8)](../../issues/8))
    - [x] Hardware/Firmware revision (read only)
    - [ ] Passwords and chip setting protection
- [ ] Reset

Haven't tested with the actual MCP2221, but it is expected that works as same as MCP2221A.

# Library API features
- Frameworks/Platforms
  - .NET Standard 2.1/.NET 5
  - Windows/Linux/MacOS and any other platforms which USB-HID driver supports
- `Smdn.Devices.MCP2221`
  - `Read`/`Write` and other command methods
    - Supports `Span<byte>`/`Memory<byte>`
    - Supports `async`, `CancellationToken`
    - Supports logging with `ILogger`, [Microsoft.Extensions.Logging](https://www.nuget.org/packages/Microsoft.Extensions.Logging/) ([example](examples/Smdn.Devices.MCP2221/logging/))
  - Can handle multiple MCP2221/MCP2221A by finding target with `Predicate<IUsbHidDevice>`
  - I2C bus scanning ([example](examples/Smdn.Devices.MCP2221/i2cscanbus/))
  - Using [HIDSharp](https://www.zer7.com/software/hidsharp) as default USB-HID driver, [LibUsbDotNet](https://www.nuget.org/packages/LibUsbDotNet/) also supported.
- `Smdn.Devices.MCP2221.GpioAdapter`
  - Provides an adapter for [System.Device.Gpio](https://www.nuget.org/packages/System.Device.Gpio/)
  - Can handle I2C devices using with [Iot.Device.Bindings](https://www.nuget.org/packages/Iot.Device.Bindings/) ([examples](examples/Smdn.Devices.MCP2221.GpioAdapter/))

# Getting started and usage examples

## Linux setup
To use the MCP2221 with this library, two configuration steps may be required depending on your Linux distribution.

### Device permissions (udev) <a name="mcp2221_udev_device_permissions">§</a>
To access the MCP2221 via this library, some system configuration may be required. Generally, a **udev rule** is necessary on most distributions to grant non-root users access to the device (see [udev rule files](misc/udev/) for setup instructions).

### Driver conflict (Ubuntu 24.04 / Kernel 6.8+) <a name="mcp2221_modprobe_blacklist">§</a>
On Ubuntu 24.04 (Kernel 6.8+) and newer systems, you may also encounter a driver conflict where the native `hid_mcp2221` driver claims the device, preventing the `/dev/hidraw*` node from being created. In this case, you must **blacklist** the dedicated driver to force the system to use the generic `usbhid` driver. Detailed steps for this process can be found in [modprobe blacklist file](misc/modprobe/blacklist-MCP2221.conf).

## Write code
Firstly, add package [Smdn.Devices.MCP2221](https://www.nuget.org/packages/Smdn.Devices.MCP2221/) to your project.

```
dotnet add package Smdn.Devices.MCP2221
```

Nextly, write your codes. The simplest code, blinking the LEDs connected to the GP pins is like below.

```cs
using System;
using System.Threading;
using Smdn.Devices.MCP2221;

using var device = MCP2221.Open();

// configure GP0-GP3 as GPIO output
device.GP0.ConfigureAsGPIO(PinMode.Output);
device.GP1.ConfigureAsGPIO(PinMode.Output);
device.GP2.ConfigureAsGPIO(PinMode.Output);
device.GP3.ConfigureAsGPIO(PinMode.Output);

// blink GP0-GP3
foreach (var gp in device.GPs) {
  Console.WriteLine($"blink {gp.PinDesignation}");

  for (var n = 0; n < 10; n++) {
    gp.SetValue(false); Thread.Sleep(100);
    gp.SetValue(true); Thread.Sleep(100);
  }
}
```

For detailed instructions, including wiring of the devices and parts, see [blink example](examples/Smdn.Devices.MCP2221/blink-csharp) page.

More examples can be found in following examples directory.

- [Smdn.Devices.MCP2221 examples](examples/Smdn.Devices.MCP2221/): Small examples using MCP2221/MCP2221A functionalities.
- [Smdn.Devices.MCP2221.GpioAdapter examples](examples/Smdn.Devices.MCP2221.GpioAdapter/): Small examples using [Iot.Device.Bindings](https://www.nuget.org/packages/Iot.Device.Bindings/).

# Troubleshooting
## Linux
### MCP2221 Detection and Access Permissions
If your application cannot detect the MCP2221, follow these steps to diagnose driver conflicts or permission issues.

First, check if the system has created the character device file for HIDRAW:

```bash
ls -l /dev/hidraw*
```

If no devices are listed, or if a device corresponding to the target MCP2221 is not listed, the kernel driver conflict is likely the cause. If the file exists but you cannot open it due to unprivileged access, [check the permissions](#mcp2221_udev_device_permissions).

Next, inspect the kernel logs. Monitor the kernel logs while reconnecting the device to identify which driver is claiming it. Use `dmesg -w` or `journalctl -k -f`.

**Expected Output**: You should see `hid-generic` or `usbhid` associated with the device, followed by a `hidraw` assignment.

```
kernel: usb 3-1.1: New USB device found, idVendor=04d8, idProduct=00dd, bcdDevice= 1.00
kernel: usb 3-1.1: New USB device strings: Mfr=1, Product=2, SerialNumber=0
kernel: usb 3-1.1: Product: MCP2221 USB-I2C/UART Combo
kernel: usb 3-1.1: Manufacturer: Microchip Technology Inc.
kernel: hid-generic 0003:04D8:00DD.0008: hiddev0,hidraw0: USB HID v1.11 Device [Microchip Technology Inc. MCP2221 USB-I2C/UART Combo] on usb-0000:00:14.0-1.1/input2
```

*In this example, `/dev/hidraw0` is successfully assigned.*

**Conflict Indicator**: If you see `mcp2221` mentioned instead of `hid-generic`, or logs indicating the I2C/GPIO features are being initialized, the dedicated `mcp2221` kernel driver is active and suppressing the HIDRAW interface.

```
kernel: mcp2221 0003:04D8:00DD.000A: USB HID v1.11 Device [Microchip Technology Inc. MCP2221 USB-I2C/UART Combo] on usb-0000:00:14.0-1.1/input2
```

If this occurs, you must [blacklist the mcp2221 driver](#mcp2221_modprobe_blacklist).

To verify which driver is currently controlling the interface, run:

```bash
lsusb -t
```

Look for the MCP2221 interface (typically `Class=Human Interface Device`).

If the target interface is displayed as `Driver=usbhid`, it can be controlled as HIDRAW. However, if it is displayed as `Driver=mcp2221`, it is controlled by the kernel driver, and the HIDRAW node is suppressed.

If the `/dev/hidrawX` file exists but remains inaccessible, verify the ACL (Access Control List) permissions using the `getfacl` command and check that your user is part of the `plugdev` group or that the `uaccess` tag is correctly applied:

```bash
getfacl /dev/hidrawX  # Replace X with your device number
```

```
# file: dev/hidrawX
# owner: root
# group: plugdev
user::rw-
user:[your-username]:rw-  # This line may be added by 'uaccess'
group::rw-
mask::rw-
other::---
```

If you do not have sufficient permissions, check if the `udev` properties were applied correctly:

```bash
udevadm info -n /dev/hidrawX
```

Confirm that `E: GROUP=plugdev` and/or `E: TAGS=:uaccess:` are present in the output. If these properties are missing, ensure your `udev` rules are correctly installed and reloaded.

If `uaccess` does not apply as expected, manually add your user to the `plugdev` group:

```bash
sudo usermod -aG plugdev $USER   # A re-login is required for group changes to take effect
```


### DllImport resolving
LibUsbDotNet do DllImport-ing a shared library with the filename `libusb-1.0.so.0`.

If the libusb's .so filename installed on your system is different from that, use the [NativeLibrary.SetDllImportResolver()](https://docs.microsoft.com/ja-jp/dotnet/api/system.runtime.interopservices.nativelibrary.setdllimportresolver) to load installed .so file like below.

```sh
$ find /lib/ -name "libusb-*.so*"
/lib/x86_64-linux-gnu/libusb-1.0.so.x.y.z
/lib/i386-linux-gnu/libusb-1.0.so.x.y.z
```

```cs
using System.Runtime.InteropServices;

static void Main() {
  // libusb.so filename which is installed on your system
  const string fileNameLibUsb = "libusb-1.0.so.x.y.z";

  NativeLibrary.SetDllImportResolver(
    typeof(global::LibUsbDotNet.LibUsb.UsbDevice).Assembly,
    (libraryName, assembly, searchPath) => {
      if (string.Equals(libraryName, "libusb-1.0.so.0", StringComparison.OrdinalIgnoreCase)) {
        if (NativeLibrary.TryLoad(fileNameLibUsb, out var handleOfLibUsb))
          return handleOfLibUsb;
      }

      return IntPtr.Zero;
    }
  );

  // your codes here
    ︙
    ︙
}
```

### Unbinding usbhid driver
When using LibUsbDotNet, you need to unbind the devices that are bound to the usbhid driver.

If you got the exception like below, configure the MCP2221/MCP2221A to unbind, and reconnect it again. See [udev rule file](misc/udev/90-MCP2221-LibUsbDotNet.rules) for detail.

```
Unhandled exception. Smdn.Devices.MCP2221.DeviceUnavailableException: MCP2221/MCP2221A is not available, not privileged or disconnected.
  ---> LibUsbDotNet.LibUsb.UsbException: Resource busy
    at LibUsbDotNet.LibUsb.ErrorExtensions.ThrowOnError(Error error)
    at LibUsbDotNet.LibUsb.UsbDevice.ClaimInterface(Int32 interfaceID)
```

### Permitting device access
If you got the exception like below, you need to run as the root user, the command like `sudo dotnet run`.

LibUsbDotNet:

```
Unhandled exception. Smdn.Devices.MCP2221.DeviceUnavailableException: MCP2221/MCP2221A is not available, not privileged or disconnected.
 ---> LibUsbDotNet.LibUsb.UsbException: Access denied (insufficient permissions)
   at LibUsbDotNet.LibUsb.ErrorExtensions.ThrowOnError(Error error)
   at LibUsbDotNet.LibUsb.UsbDevice.Open()
```

HIDSharp:

```
Unhandled exception. Smdn.Devices.MCP2221.DeviceUnavailableException: MCP2221/MCP2221A is not available, not privileged or disconnected.
 ---> HidSharp.Exceptions.DeviceIOException: Unable to open HID class device (OK).
   at HidSharp.Platform.Linux.LinuxHidStream.DeviceHandleFromPath(String path, HidDevice hidDevice, oflag oflag)
   at HidSharp.Platform.Linux.LinuxHidDevice.TryParseReportDescriptor(ReportDescriptor& parser, Byte[]& reportDescriptor)
   at HidSharp.Platform.Linux.LinuxHidDevice.RequiresGetInfo()
   at HidSharp.Platform.Linux.LinuxHidDevice.OpenDeviceDirectly(OpenConfiguration openConfig)
   at HidSharp.Device.OpenDeviceAndRestrictAccess(OpenConfiguration openConfig)
   at HidSharp.Device.Open(OpenConfiguration openConfig)
   at HidSharp.Device.Open()
   at HidSharp.HidDevice.Open()
```

If you want to give access privileges to a non-root user instead, you can use udev rule file. See [udev rule files](misc/udev/).


