// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT

#if USBHIDDRIVER_LIBUSBDOTNET

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using LibUsbDotNet;
using LibUsbDotNet.Info;
using LibUsbDotNet.Main;
using LibUsbDotNet.LibUsb;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Smdn.Devices.UsbHid;

namespace Smdn.Devices.UsbHid.LibUsbDotNet;

internal class Device : IUsbHidDevice {
  /*
   * static members
   */
  public static Device Find(Predicate<Device> predicate, IServiceProvider serviceProvider = null)
  {
    if (predicate is null)
      throw new ArgumentNullException(nameof(predicate));

    var logger = serviceProvider?.GetService<ILoggerFactory>()?.CreateLogger<Device>();

    using (var context = new UsbContext()) {
      context.SetDebugLevel(Log.LibUsbDotNetLogLevel);

      foreach (var usbDevice in context.List().OfType<UsbDevice>()) {
        if (!usbDevice.Configs.SelectMany(c => c.Interfaces).Any(i => i.Class == ClassCode.Hid))
          continue; // has no USB-HID interface

        var d = new Device(usbDevice, logger);

        logger?.LogDebug(EventIds.UsbHidListDevice, $"found: VID={d.VendorID:x4} PID={d.ProductID:x4} ({d.Manufacturer}, {d.ProductName}, {d.SerialNumber ?? "[no serial number]"}) {d.FileSystemName} {d.DevicePath}");

        if (predicate(d)) {
          logger?.LogInformation(EventIds.UsbHidSelectDevice, $"selected VID={d.VendorID:x4} PID={d.ProductID:x4} ({d.Manufacturer}, {d.ProductName}, {d.SerialNumber ?? "[no serial number]"}) {d.FileSystemName} {d.DevicePath}");

          return d;
        }

        d.Dispose();
      }

      return null;
    }
  }

  /*
   * instance members
   */
  private UsbDevice _usbDevice;
  private UsbDevice UsbDevice => _usbDevice ?? throw new ObjectDisposedException(GetType().Name);

  private readonly ILogger logger;

  public string ProductName => UsbDevice.Descriptor.Product;
  public string Manufacturer => UsbDevice.Descriptor.Manufacturer;
  public int VendorID => (int)UsbDevice.VendorId;
  public int ProductID => (int)UsbDevice.ProductId;
  public string SerialNumber => UsbDevice.Descriptor.SerialNumber;
  public System.Version ReleaseNumber => null;
  public string DevicePath => null;
  public string FileSystemName => null;

  internal Device(UsbDevice usbDevice, ILogger logger)
  {
    this._usbDevice = usbDevice;
    this.logger = logger;
  }

  private Stream OpenEndpointStream()
  {
    UsbConfigInfo config = null;
    UsbInterfaceInfo hidInterface = null;
    UsbEndpointInfo outEndpoint = null;
    UsbEndpointInfo inEndpoint = null;

    const byte endpointAddressInOutBitMask  = 0b_1000_0000;
    const byte endpointAddressIn            = 0b_1000_0000;
    const byte endpointAddressOut           = 0b_0000_0000;
    //const byte endpointAddressNumberMask    = 0b_0000_0111;
    const byte attributesTransferTypeMask   = 0b_0000_0011;

    foreach (var cfg in UsbDevice.Configs) {
      foreach (var iface in cfg.Interfaces) {
        if (iface.Class == ClassCode.Hid) {
          config = cfg;
          hidInterface = iface;
          outEndpoint = iface.Endpoints.FirstOrDefault(ep => (ep.EndpointAddress & endpointAddressInOutBitMask) == endpointAddressOut);
          inEndpoint = iface.Endpoints.FirstOrDefault(ep => (ep.EndpointAddress & endpointAddressInOutBitMask) == endpointAddressIn);

          break;
        }
      }

      if (hidInterface is not null)
        break;
    }

    if (config is null)
      throw new UsbHidException("USB configuration not found");
    if (hidInterface is null)
      throw new UsbHidException("HID interface not found");
    if (outEndpoint is null)
      throw new UsbHidException("HID OUT endpoint not found");
    if (inEndpoint is null)
      throw new UsbHidException("HID IN endpoint not found");

    logger?.LogDebug(EventIds.UsbHidOpenEndPoint, $"HID interface: {hidInterface}");

    if (!UsbDevice.IsOpen)
      UsbDevice.Open();

    // try set configuration
    foreach (var cfg in new[] {config.ConfigurationValue, 0 /* fallback */}) {
      try {
        UsbDevice.SetConfiguration(cfg);
      }
      catch (UsbException ex) when (ex.ErrorCode == Error.Busy) {
        // [LibUsbDotNet 3.0.87-alpha] SetConfiguration always throw UsbException with Error.Busy
        logger?.LogWarning(EventIds.UsbHidOpenEndPoint, ex, $"configuration #{config.ConfigurationValue} resource busy");
        continue; // expected, continue
      }
      catch (Exception ex) {
        logger?.LogCritical(EventIds.UsbHidOpenEndPoint, ex, $"set configuration #{config.ConfigurationValue} failed");
        throw; // unexpected
      }
    }

    // try claim HID interface
    try {
      UsbDevice.ClaimInterface(hidInterface.Number);
    }
    catch (Exception ex) {
      logger?.LogCritical(EventIds.UsbHidOpenEndPoint, ex, $"claim interface #{hidInterface.Number} failed");
      throw;
    }

    // open HID endpoint
    return new Stream(
      maxOutPacketSize: outEndpoint.MaxPacketSize,
      maxInPacketSize: inEndpoint.MaxPacketSize,
      writer: UsbDevice.OpenEndpointWriter(
        writeEndpointID: (WriteEndpointID)outEndpoint.EndpointAddress,
        endpointType: (EndpointType)(outEndpoint.Attributes & attributesTransferTypeMask)
      ),
      reader: UsbDevice.OpenEndpointReader(
        readBufferSize: Stream.DefaultReadBufferSize,
        readEndpointID: (ReadEndpointID)inEndpoint.EndpointAddress,
        endpointType: (EndpointType)(inEndpoint.Attributes & attributesTransferTypeMask)
      )
    );
  }

  public ValueTask<IUsbHidStream> OpenStreamAsync()
  {
    return
#if NET5_0_OR_GREATER
    ValueTask.FromResult<IUsbHidStream>
#else
    new ValueTask<IUsbHidStream>
#endif
    (OpenEndpointStream());
  }

  public IUsbHidStream OpenStream()
    => OpenEndpointStream();

  public void Dispose()
  {
    _usbDevice?.Dispose();
    _usbDevice = null;
  }

  public ValueTask DisposeAsync()
  {
    _usbDevice?.Dispose();
    _usbDevice = null;

#if NET5_0_OR_GREATER
    return ValueTask.CompletedTask;
#else
    return new ValueTask(Task.CompletedTask);
#endif
  }
}

#endif // USBHIDDRIVER_LIBUSBDOTNET
