// SPDX-FileCopyrightText: 2026 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Smdn.Devices.UsbHid;

/// <summary>
/// Provides extension methods for <see cref="IUsbHidService"/>.
/// </summary>
public static class IUsbHidServiceExtensions {
  /// <summary>
  /// Gets a list of USB-HID devices that match the specified vendor and product IDs.
  /// </summary>
  /// <param name="usbHidService">
  /// The <see cref="IUsbHidService"/> to use for device enumeration.
  /// </param>
  /// <param name="vendorId">
  /// The vendor ID to match, or <see langword="null"/> to match any vendor ID.
  /// </param>
  /// <param name="productId">
  /// The product ID to match, or <see langword="null"/> to match any product ID.
  /// </param>
  /// <param name="cancellationToken">
  /// The <see cref="CancellationToken"/> to monitor for cancellation requests.
  /// </param>
  /// <returns>
  /// A <see cref="IReadOnlyList{T}"/> of <see cref="IUsbHidDevice"/> that match the specified criteria.
  /// </returns>
  /// <exception cref="ArgumentNullException">
  /// <paramref name="usbHidService"/> is <see langword="null"/>.
  /// </exception>
  /// <seealso cref="IUsbHidService.GetDevices(CancellationToken)"/>
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

    try {
      foreach (var device in devices) {
        if (cancellationToken.IsCancellationRequested)
          break;

        if (vendorId.HasValue && device.VendorId != vendorId.Value)
          continue;

        if (productId.HasValue && device.ProductId != productId.Value)
          continue;

        filteredDevices.Add(device);
      }

      return filteredDevices;
    }
    finally {
      foreach (var device in devices.Except(filteredDevices)) {
        device.Dispose();
      }

      cancellationToken.ThrowIfCancellationRequested();
    }
  }

  /// <summary>
  /// Finds a USB-HID device that matches the specified vendor ID, product ID, and/or predicate.
  /// </summary>
  /// <param name="usbHidService">
  /// The <see cref="IUsbHidService"/> to use for device enumeration.
  /// </param>
  /// <param name="vendorId">
  /// The vendor ID to match, or <see langword="null"/> to match any vendor ID.
  /// </param>
  /// <param name="productId">
  /// The product ID to match, or <see langword="null"/> to match any product ID.
  /// </param>
  /// <param name="predicate">
  /// A <see cref="Predicate{T}"/> to use for filtering the devices.
  /// If <see langword="null"/>, no predicate is applied.
  /// </param>
  /// <param name="cancellationToken">
  /// The <see cref="CancellationToken"/> to monitor for cancellation requests.
  /// </param>
  /// <returns>
  /// The first <see cref="IUsbHidDevice"/> that matches the specified criteria,
  /// or <see langword="null"/> if no matching device is found.
  /// </returns>
  /// <exception cref="ArgumentNullException">
  /// <paramref name="usbHidService"/> is <see langword="null"/>.
  /// </exception>
  /// <seealso cref="IUsbHidService.GetDevices(CancellationToken)"/>
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
    IUsbHidDevice? matchedDevice = null;

    try {
      foreach (var device in devices) {
        if (cancellationToken.IsCancellationRequested)
          break;

        if (vendorId.HasValue && device.VendorId != vendorId.Value)
          continue;

        if (productId.HasValue && device.ProductId != productId.Value)
          continue;

        if (predicate is null || predicate(device)) {
          matchedDevice = device;
          break;
        }
      }

      return matchedDevice;
    }
    finally {
      foreach (var device in devices) {
        if (cancellationToken.IsCancellationRequested || !ReferenceEquals(device, matchedDevice))
          device.Dispose();
      }

      cancellationToken.ThrowIfCancellationRequested();
    }
  }

  /// <summary>
  /// Finds a USB-HID device that matches the specified vendor ID, product ID, and
  /// a predicate on the implementation-specific device object.
  /// </summary>
  /// <param name="usbHidService">
  /// The <see cref="IUsbHidService"/> to use for device enumeration.
  /// </param>
  /// <param name="vendorId">
  /// The vendor ID to match, or <see langword="null"/> to match any vendor ID.
  /// </param>
  /// <param name="productId">
  /// The product ID to match, or <see langword="null"/> to match any product ID.
  /// </param>
  /// <param name="predicate">
  /// A <see cref="Predicate{T}"/> to use for filtering the devices.
  /// </param>
  /// <param name="cancellationToken">
  /// The <see cref="CancellationToken"/> to monitor for cancellation requests.
  /// </param>
  /// <returns>
  /// The first <see cref="IUsbHidDevice"/> that matches the specified criteria,
  /// or <see langword="null"/> if no matching device is found.
  /// </returns>
  /// <exception cref="ArgumentNullException">
  /// <paramref name="usbHidService"/> is <see langword="null"/>.
  /// </exception>
  /// <remarks>
  /// This method applies the <paramref name="predicate"/> to the underlying device object
  /// (<see cref="IUsbHidDevice{T}.DeviceImplementation"/>) from the backend library to filter devices.
  /// This is useful for filtering on properties that are specific to the backend implementation.
  /// </remarks>
  /// <seealso cref="IUsbHidService.GetDevices(CancellationToken)"/>
  /// <seealso cref="IUsbHidDevice{T}.DeviceImplementation"/>
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
    IUsbHidDevice? matchedDevice = null;

    try {
      foreach (var device in devices) {
        if (cancellationToken.IsCancellationRequested)
          break;

        if (vendorId.HasValue && device.VendorId != vendorId.Value)
          continue;

        if (productId.HasValue && device.ProductId != productId.Value)
          continue;

        if (device is IUsbHidDevice<TDevice> d && predicate(d.DeviceImplementation)) {
          matchedDevice = device;
          break;
        }
      }

      return matchedDevice;
    }
    finally {
      foreach (var device in devices) {
        if (cancellationToken.IsCancellationRequested || !ReferenceEquals(device, matchedDevice))
          device.Dispose();
      }

      cancellationToken.ThrowIfCancellationRequested();
    }
  }
}
