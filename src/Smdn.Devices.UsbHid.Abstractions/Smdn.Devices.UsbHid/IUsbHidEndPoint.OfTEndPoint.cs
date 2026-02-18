// SPDX-FileCopyrightText: 2026 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT

namespace Smdn.Devices.UsbHid;

public interface IUsbHidEndPoint<TReadEndPoint, TWriteEndPoint> : IUsbHidEndPoint {
  /// <summary>
  /// Gets the implementation-dependent endpoint object for reading
  /// held internally by this object.
  /// </summary>
  TReadEndPoint? ReadEndPoint { get; }

  /// <summary>
  /// Gets the implementation-dependent endpoint object for writing
  /// held internally by this object.
  /// </summary>
  TWriteEndPoint? WriteEndPoint { get; }
}
