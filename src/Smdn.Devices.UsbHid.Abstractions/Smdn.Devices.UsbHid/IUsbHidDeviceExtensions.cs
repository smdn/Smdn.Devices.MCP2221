// SPDX-FileCopyrightText: 2026 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Smdn.Devices.UsbHid;

public static class IUsbHidDeviceExtensions {
  public static IUsbHidEndPoint OpenEndPoint(this IUsbHidDevice device, CancellationToken cancellationToken = default)
    => (device ?? throw new ArgumentNullException(nameof(device)))
      .OpenEndPoint(shouldDisposeDevice: false, cancellationToken);

  public static IUsbHidEndPoint OpenEndPoint(this IUsbHidDevice device, bool shouldDisposeDevice)
    => (device ?? throw new ArgumentNullException(nameof(device)))
      .OpenEndPoint(shouldDisposeDevice: shouldDisposeDevice, cancellationToken: default);

  public static ValueTask<IUsbHidEndPoint> OpenEndPointAsync(this IUsbHidDevice device, CancellationToken cancellationToken = default)
    => (device ?? throw new ArgumentNullException(nameof(device)))
      .OpenEndPointAsync(shouldDisposeDevice: false, cancellationToken);

  public static ValueTask<IUsbHidEndPoint> OpenEndPointAsync(this IUsbHidDevice device, bool shouldDisposeDevice)
    => (device ?? throw new ArgumentNullException(nameof(device)))
      .OpenEndPointAsync(shouldDisposeDevice: shouldDisposeDevice, cancellationToken: default);

  public static string ToIdentificationString(this IUsbHidDevice device)
  {
    if (device is null)
      throw new ArgumentNullException(nameof(device));

    if (device.TryGetDeviceIdentifier(out var deviceIdentifier)) {
      return
#if NULL_STATE_STATIC_ANALYSIS_ATTRIBUTES
        deviceIdentifier;
#else
        deviceIdentifier!;
#endif
    }

    if (device.TryGetSerialNumber(out var serialNumber))
      return $"{{{device.VendorId:X4}:{device.ProductId:X4};{serialNumber}}}";

    return device.ToString() ?? $"{{{device.VendorId:X4}:{device.ProductId:X4}}}";
  }
}
