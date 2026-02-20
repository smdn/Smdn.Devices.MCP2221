// SPDX-FileCopyrightText: 2026 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT

namespace Smdn.Devices.UsbHid;

/// <summary>
/// Provides a mechanism to abstract reading from and writing to USB-HID report endpoints,
/// and provides properties for accessing the underlying endpoint objects used by
/// the backend library.
/// </summary>
public interface IUsbHidEndPoint<TReadEndPoint, TWriteEndPoint> : IUsbHidEndPoint {
  /// <summary>
  /// Gets the implementation-dependent underlying endpoint object for reading.
  /// </summary>
  TReadEndPoint? ReadEndPoint { get; }

  /// <summary>
  /// Gets the implementation-dependent underlying endpoint object for writing.
  /// </summary>
  TWriteEndPoint? WriteEndPoint { get; }
}
