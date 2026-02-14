// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
#pragma warning disable CA1848

using System;
#if NULL_STATE_STATIC_ANALYSIS_ATTRIBUTES
using System.Diagnostics.CodeAnalysis;
#endif
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using LibUsbDotNet;
using LibUsbDotNet.Info;
using LibUsbDotNet.LibUsb;
using LibUsbDotNet.Main;

using Microsoft.Extensions.Logging;

using Smdn.Devices.UsbHid.Logging;

namespace Smdn.Devices.UsbHid;

internal sealed class UsbHidDevice(
  UsbHidService service,
  UsbDevice device,
  ILogger? logger
) : IUsbHidDevice<UsbDevice> {
  private UsbDevice? deviceImplementation = device ?? throw new ArgumentNullException(nameof(device));

  public UsbDevice DeviceImplementation => deviceImplementation ?? throw new ObjectDisposedException(GetType().FullName);

  public int VendorId => DeviceImplementation.VendorId;
  public int ProductId => DeviceImplementation.ProductId;

  private void ThrowIfDisposed()
  {
    if (deviceImplementation is null)
      throw new ObjectDisposedException(GetType().FullName);
  }

  public bool TryGetProductName(
#if NULL_STATE_STATIC_ANALYSIS_ATTRIBUTES
    [NotNullWhen(true)]
#endif
    out string? productName
  )
  {
    productName = default;

    if (!DeviceImplementation.IsOpen && !DeviceImplementation.TryOpen())
      return false;

    return (productName = DeviceImplementation.Descriptor.Product) is not null;
  }

  public bool TryGetManufacturer(
#if NULL_STATE_STATIC_ANALYSIS_ATTRIBUTES
    [NotNullWhen(true)]
#endif
    out string? manufacturer
  )
  {
    manufacturer = default;

    if (!DeviceImplementation.IsOpen && !DeviceImplementation.TryOpen())
      return false;

    return (manufacturer = DeviceImplementation.Descriptor.Manufacturer) is not null;
  }

  public bool TryGetSerialNumber(
#if NULL_STATE_STATIC_ANALYSIS_ATTRIBUTES
    [NotNullWhen(true)]
#endif
    out string? serialNumber
  )
  {
    serialNumber = default;

    if (!DeviceImplementation.IsOpen && !DeviceImplementation.TryOpen())
      return false;

    return (serialNumber = DeviceImplementation.Descriptor.SerialNumber) is not null;
  }

  public bool TryGetDeviceIdentifier(
#if NULL_STATE_STATIC_ANALYSIS_ATTRIBUTES
    [NotNullWhen(true)]
#endif
    out string? deviceIdentifier
  )
    => (deviceIdentifier = DeviceImplementation.LocationId.ToString()) is not null;

  public void Dispose()
  {
    deviceImplementation?.Dispose();
    deviceImplementation = null;
  }

  public ValueTask DisposeAsync()
  {
    // UsbDevice does not implement IAsyncDisposable
    deviceImplementation?.Dispose();
    deviceImplementation = null;

    return default;
  }

  private UsbHidEndPoint OpenEndPointCore(
    bool shouldDisposeDevice,
    CancellationToken cancellationToken
  )
  {
    UsbConfigInfo? config = null;
    UsbInterfaceInfo? hidInterface = null;
    UsbEndpointInfo? outEndpoint = null;
    UsbEndpointInfo? inEndpoint = null;

    const byte EndpointAddressInOutBitMask  = 0b_1000_0000;
    const byte EndpointAddressIn            = 0b_1000_0000;
    const byte EndpointAddressOut           = 0b_0000_0000;
    // const byte EndpointAddressNumberMask    = 0b_0000_0111;
    const byte AttributesTransferTypeMask   = 0b_0000_0011;

    foreach (var cfg in DeviceImplementation.Configs) {
      foreach (var iface in cfg.Interfaces) {
        if (iface.Class == ClassCode.Hid) {
          config = cfg;
          hidInterface = iface;
          outEndpoint = iface.Endpoints.FirstOrDefault(ep => (ep.EndpointAddress & EndpointAddressInOutBitMask) == EndpointAddressOut);
          inEndpoint = iface.Endpoints.FirstOrDefault(ep => (ep.EndpointAddress & EndpointAddressInOutBitMask) == EndpointAddressIn);

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

    logger?.LogDebug(
      EventIds.UsbHidOpenEndPoint,
      "HID interface: #{Number}, {Interface}",
      hidInterface.Number,
      hidInterface.Interface
    );

    cancellationToken.ThrowIfCancellationRequested();

    if (!DeviceImplementation.IsOpen)
      DeviceImplementation.Open();

    // try set configuration
    foreach (var cfg in new[] { config.ConfigurationValue, 0 /* fallback */ }) {
      try {
        DeviceImplementation.SetConfiguration(cfg);
      }
      catch (UsbException ex) when (ex.ErrorCode == Error.Busy) {
        // [LibUsbDotNet 3.0.87-alpha] SetConfiguration always throw UsbException with Error.Busy
        logger?.LogWarning(
          EventIds.UsbHidOpenEndPoint,
          ex,
          "configuration #{Configuration} resource busy",
          config.ConfigurationValue
        );
        continue; // expected, continue
      }
      catch (Exception ex) {
        logger?.LogCritical(
          EventIds.UsbHidOpenEndPoint,
          ex,
          "set configuration #{Configuration} failed",
          config.ConfigurationValue
        );
        throw; // unexpected
      }
    }

    // try claim HID interface
    try {
      DeviceImplementation.ClaimInterface(hidInterface.Number);
    }
    catch (Exception ex) {
      logger?.LogCritical(
        EventIds.UsbHidOpenEndPoint,
        ex,
        "claim interface #{Number} failed",
        hidInterface.Number
      );
      throw;
    }

    // open HID endpoint
    return new(
      openedDevice: this,
      endPointReader: DeviceImplementation.OpenEndpointReader(
        readBufferSize: service.Options.ReadEndPointBufferSize,
        readEndpointID: (ReadEndpointID)inEndpoint.EndpointAddress,
        endpointType: (EndpointType)(inEndpoint.Attributes & AttributesTransferTypeMask)
      ),
      maxInEndPointPacketSize: inEndpoint.MaxPacketSize,
      readEndPointTimeout: service.Options.ReadEndPointTimeout,
      endPointWriter: DeviceImplementation.OpenEndpointWriter(
        writeEndpointID: (WriteEndpointID)outEndpoint.EndpointAddress,
        endpointType: (EndpointType)(outEndpoint.Attributes & AttributesTransferTypeMask)
      ),
      maxOutEndPointPacketSize: outEndpoint.MaxPacketSize,
      writeEndPointTimeout: service.Options.WriteEndPointTimeout,
      shouldDisposeDevice: shouldDisposeDevice
    );
  }

  public IUsbHidEndPoint OpenEndPoint(
    bool shouldDisposeDevice,
    CancellationToken cancellationToken
  )
  {
    ThrowIfDisposed();

    cancellationToken.ThrowIfCancellationRequested();

    return OpenEndPointCore(
      shouldDisposeDevice: shouldDisposeDevice,
      cancellationToken: cancellationToken
    );
  }

  public ValueTask<IUsbHidEndPoint> OpenEndPointAsync(
    bool shouldDisposeDevice,
    CancellationToken cancellationToken
  )
  {
    ThrowIfDisposed();

    cancellationToken.ThrowIfCancellationRequested();

    return
#pragma warning disable SA1114
#if SYSTEM_THREADING_TASKS_VALUETASK_FROMRESULT
      ValueTask.FromResult<IUsbHidEndPoint>(
#else
      new(
#endif
#pragma warning disable CA2000
        result: OpenEndPointCore(
          shouldDisposeDevice: shouldDisposeDevice,
          cancellationToken: cancellationToken
        )
#pragma warning restore CA2000
      );
#pragma warning restore SA1114
  }

  public override string? ToString()
    => $"{GetType().FullName} (DeviceImplementation='{DeviceImplementation}')";
}
