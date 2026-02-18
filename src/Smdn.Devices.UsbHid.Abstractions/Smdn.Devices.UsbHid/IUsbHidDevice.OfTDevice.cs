// SPDX-FileCopyrightText: 2026 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT

namespace Smdn.Devices.UsbHid;

public interface IUsbHidDevice<TDevice> : IUsbHidDevice where TDevice : notnull {
  /// <summary>
  /// Gets the implementation-dependent device object held
  /// internally by this object.
  /// </summary>
  TDevice DeviceImplementation { get; }
}
