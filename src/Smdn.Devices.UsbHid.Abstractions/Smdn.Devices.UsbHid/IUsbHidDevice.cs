// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
#if NULL_STATE_STATIC_ANALYSIS_ATTRIBUTES
using System.Diagnostics.CodeAnalysis;
#endif

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Smdn.Devices.UsbHid;

/// <summary>
/// Defines an interface for accessing information about USB-HID devices.
/// Typically, it retrieves information obtained from the device descriptor.
/// </summary>
public interface IUsbHidDevice : IDisposable, IAsyncDisposable {
  int VendorId { get; }
  int ProductId { get; }

  bool TryGetProductName(
#if NULL_STATE_STATIC_ANALYSIS_ATTRIBUTES
    [NotNullWhen(true)]
#endif
    out string? productName
  );

  bool TryGetManufacturer(
#if NULL_STATE_STATIC_ANALYSIS_ATTRIBUTES
    [NotNullWhen(true)]
#endif
    out string? manufacturer
  );

  bool TryGetSerialNumber(
#if NULL_STATE_STATIC_ANALYSIS_ATTRIBUTES
    [NotNullWhen(true)]
#endif
    out string? serialNumber
  );

  /// <summary>
  /// Attempts to get the implementation-dependent <see cref="string"/> for identifying the device.
  /// </summary>
  /// <param name="deviceIdentifier">
  /// The <see cref="string"/> that identifies a specific device.
  /// </param>
  /// <returns>
  /// <see langword="true"/> if the implementation-dependent identifier was retrieved,
  /// otherwise <see langword="false"/>.
  /// </returns>
  /// <remarks>
  /// This method retrieves an implementation-dependent string.
  /// Typically, it returns a path to a device file or a string consisting of a bus and port number,
  /// but its format is not standardized.
  /// </remarks>
  bool TryGetDeviceIdentifier(
#if NULL_STATE_STATIC_ANALYSIS_ATTRIBUTES
    [NotNullWhen(true)]
#endif
    out string? deviceIdentifier
  );

  /// <summary>
  /// Opens the endpoint for report input and output.
  /// </summary>
  /// <param name="shouldDisposeDevice">
  /// Specifies whether to also dispose the source <see cref="IUsbHidDevice"/> when
  /// disposing the opened <see cref="IUsbHidEndPoint"/>.
  /// </param>
  /// <param name="cancellationToken">
  /// The <see cref="CancellationToken"/> to monitor for cancellation requests.
  /// </param>
  /// <returns>
  /// The <see cref="IUsbHidEndPoint"/> that represents the opened endpoint.
  /// </returns>
  /// <exception cref="ObjectDisposedException">
  /// Attempted to open endpoint after the instance was disposed.
  /// </exception>
  IUsbHidEndPoint OpenEndPoint(bool shouldDisposeDevice, CancellationToken cancellationToken);

  /// <summary>
  /// Opens the endpoint for report input and output.
  /// </summary>
  /// <param name="shouldDisposeDevice">
  /// Specifies whether to also dispose the source <see cref="IUsbHidDevice"/> when
  /// disposing the opened <see cref="IUsbHidEndPoint"/>.
  /// </param>
  /// <param name="cancellationToken">
  /// The <see cref="CancellationToken"/> to monitor for cancellation requests.
  /// </param>
  /// <returns>
  /// The <see cref="ValueTask{T}"/> that represents the asynchronous operation.
  /// The value of its <see cref="ValueTask{T}.Result"/> property contains
  /// <see cref="IUsbHidEndPoint"/> that represents the opened endpoint.
  /// </returns>
  /// <exception cref="ObjectDisposedException">
  /// Attempted to open endpoint after the instance was disposed.
  /// </exception>
  /// <remarks>
  /// If the implementation does not support asynchronous opening,
  /// this method will perform a synchronous open instead.
  /// </remarks>
  ValueTask<IUsbHidEndPoint> OpenEndPointAsync(bool shouldDisposeDevice, CancellationToken cancellationToken);
}
