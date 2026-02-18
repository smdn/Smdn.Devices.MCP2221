// SPDX-FileCopyrightText: 2026 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT

using System;
using System.Collections.Generic;
using System.Threading;

namespace Smdn.Devices.UsbHid;

public static class IUsbHidServiceExtensions {
  public static IReadOnlyList<IUsbHidDevice> GetDevices(
    this IUsbHidService usbHidService,
    int? vendorId = null,
    int? productId = null,
    CancellationToken cancellationToken = default
  )
  {
    if (usbHidService is null)
      throw new ArgumentNullException(nameof(usbHidService));

    var devices = usbHidService.GetDevices(cancellationToken);
    var filteredDevices = new List<IUsbHidDevice>(capacity: devices.Count);

    foreach (var device in devices) {
      if (vendorId.HasValue && device.VendorId != vendorId.Value) {
        device.Dispose();
        continue;
      }

      if (productId.HasValue && device.ProductId != productId.Value) {
        device.Dispose();
        continue;
      }

      filteredDevices.Add(device);
    }

    cancellationToken.ThrowIfCancellationRequested();

    return filteredDevices;
  }

  public static IUsbHidDevice? FindDevice(
    this IUsbHidService usbHidService,
    int? vendorId,
    int? productId,
    Predicate<IUsbHidDevice>? predicate = null,
    CancellationToken cancellationToken = default
  )
  {
    if (usbHidService is null)
      throw new ArgumentNullException(nameof(usbHidService));

    var devices = usbHidService.GetDevices(cancellationToken);

    foreach (var device in devices) {
      if (vendorId.HasValue && device.VendorId != vendorId.Value) {
        device.Dispose();
        continue;
      }

      if (productId.HasValue && device.ProductId != productId.Value) {
        device.Dispose();
        continue;
      }

      if (predicate is null || predicate(device)) {
        cancellationToken.ThrowIfCancellationRequested();

        return device;
      }

      device.Dispose();
    }

    return null;
  }

  public static IUsbHidDevice? FindDevice<TDevice>(
    this IUsbHidService usbHidService,
    int? vendorId,
    int? productId,
    Predicate<TDevice> predicate,
    CancellationToken cancellationToken = default
  ) where TDevice : notnull
  {
    if (usbHidService is null)
      throw new ArgumentNullException(nameof(usbHidService));
    if (predicate is null)
      throw new ArgumentNullException(nameof(predicate));

    var devices = usbHidService.GetDevices(cancellationToken);

    foreach (var device in devices) {
      if (vendorId.HasValue && device.VendorId != vendorId.Value) {
        device.Dispose();
        continue;
      }

      if (productId.HasValue && device.ProductId != productId.Value) {
        device.Dispose();
        continue;
      }

      if (device is IUsbHidDevice<TDevice> d && predicate(d.DeviceImplementation)) {
        cancellationToken.ThrowIfCancellationRequested();

        return device;
      }

      device.Dispose();
    }

    return null;
  }
}
