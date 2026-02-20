// SPDX-FileCopyrightText: 2026 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT

using System;
using System.Collections.Generic;
using System.Threading;

namespace Smdn.Devices.UsbHid;

/// <summary>
/// Provides services for discovering and managing USB-HID devices.
/// </summary>
public interface IUsbHidService : IDisposable, IAsyncDisposable {
  /// <summary>
  /// Gets a read-only list of all USB-HID devices currently available on the system.
  /// </summary>
  /// <param name="cancellationToken">
  /// The <see cref="CancellationToken"/> to monitor for cancellation requests.
  /// </param>
  /// <returns>
  /// A <see cref="IReadOnlyList{T}"/> of <see cref="IUsbHidDevice"/> representing the
  /// devices available on the system.
  /// </returns>
  /// <exception cref="OperationCanceledException">
  /// The <paramref name="cancellationToken"/> has had cancellation requested.
  /// </exception>
  /// <exception cref="ObjectDisposedException">
  /// The service has been disposed.
  /// </exception>
  IReadOnlyList<IUsbHidDevice> GetDevices(CancellationToken cancellationToken);
}
