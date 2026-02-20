// SPDX-FileCopyrightText: 2026 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT

namespace Smdn.Devices.UsbHid;

/// <summary>
/// Provides a mechanism to abstract and operate USB-HID devices,
/// and provides a property for accessing the underlying device object used by the backend library.
/// </summary>
public interface IUsbHidDevice<TDevice> : IUsbHidDevice where TDevice : notnull {
  /// <summary>
  /// Gets the implementation-dependent underlying device object.
  /// </summary>
  TDevice DeviceImplementation { get; }
}
