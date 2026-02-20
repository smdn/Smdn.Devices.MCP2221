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
/// Provides a mechanism to abstract and operate USB-HID devices.
/// </summary>
public interface IUsbHidDevice : IDisposable, IAsyncDisposable {
  /// <summary>
  /// Gets the vendor ID of the USB-HID device.
  /// </summary>
  int VendorId { get; }

  /// <summary>
  /// Gets the product ID of the USB-HID device.
  /// </summary>
  int ProductId { get; }

  /// <summary>
  /// Attempts to get the product name of the USB-HID device.
  /// </summary>
  /// <param name="productName">
  /// When this method returns, contains the product name of the USB-HID device,
  /// if the product name can be retrieved; otherwise, <see langword="null"/>.
  /// </param>
  /// <returns>
  /// <see langword="true"/> if the product name was retrieved;
  /// otherwise, <see langword="false"/>.
  /// </returns>
  bool TryGetProductName(
#if NULL_STATE_STATIC_ANALYSIS_ATTRIBUTES
    [NotNullWhen(true)]
#endif
    out string? productName
  );

  /// <summary>
  /// Attempts to get the manufacturer of the USB-HID device.
  /// </summary>
  /// <param name="manufacturer">
  /// When this method returns, contains the manufacturer of the USB-HID device,
  /// if the manufacturer can be retrieved; otherwise, <see langword="null"/>.
  /// </param>
  /// <returns>
  /// <see langword="true"/> if the manufacturer was retrieved;
  /// otherwise, <see langword="false"/>.
  /// </returns>
  bool TryGetManufacturer(
#if NULL_STATE_STATIC_ANALYSIS_ATTRIBUTES
    [NotNullWhen(true)]
#endif
    out string? manufacturer
  );

  /// <summary>
  /// Attempts to get the serial number of the USB-HID device.
  /// </summary>
  /// <param name="serialNumber">
  /// When this method returns, contains the serial number of the USB-HID device,
  /// if the serial number can be retrieved; otherwise, <see langword="null"/>.
  /// </param>
  /// <returns>
  /// <see langword="true"/> if the serial number was retrieved;
  /// otherwise, <see langword="false"/>.
  /// </returns>
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
  /// When this method returns, contains the implementation-dependent identifier string,
  /// if the identifier can be retrieved; otherwise, <see langword="null"/>.
  /// </param>
  /// <returns>
  /// <see langword="true"/> if the implementation-dependent identifier was retrieved;
  /// otherwise, <see langword="false"/>.
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
  /// <param name="openOutEndPoint">
  /// Specifies whether to open the <c>OUT</c> endpoint for writing reports.
  /// </param>
  /// <param name="openInEndPoint">
  /// Specifies whether to open the <c>IN</c> endpoint for reading reports.
  /// </param>
  /// <param name="shouldDisposeDevice">
  /// <see langword="true"/> to dispose this device instance when the returned
  /// <see cref="IUsbHidEndPoint"/> is disposed;
  /// otherwise, <see langword="false"/>.
  /// </param>
  /// <param name="cancellationToken">
  /// The <see cref="CancellationToken"/> to monitor for cancellation requests.
  /// </param>
  /// <returns>
  /// An <see cref="IUsbHidEndPoint"/> that represents the opened endpoints.
  /// </returns>
  /// <exception cref="InvalidOperationException">
  /// Attempted to open an endpoint, but neither <paramref name="openOutEndPoint"/> nor
  /// <paramref name="openInEndPoint"/> was specified.
  /// </exception>
  /// <exception cref="ObjectDisposedException">
  /// This device instance has been disposed.
  /// </exception>
  /// <exception cref="OperationCanceledException">
  /// The <paramref name="cancellationToken"/> has had cancellation requested.
  /// </exception>
  IUsbHidEndPoint OpenEndPoint(
    bool openOutEndPoint,
    bool openInEndPoint,
    bool shouldDisposeDevice,
    CancellationToken cancellationToken
  );

  /// <summary>
  /// Asynchronously opens the endpoint for report input and output.
  /// </summary>
  /// <param name="openOutEndPoint">
  /// Specifies whether to open the <c>OUT</c> endpoint for writing reports.
  /// </param>
  /// <param name="openInEndPoint">
  /// Specifies whether to open the <c>IN</c> endpoint for reading reports.
  /// </param>
  /// <param name="shouldDisposeDevice">
  /// <see langword="true"/> to dispose this device instance when the returned
  /// <see cref="IUsbHidEndPoint"/> is disposed;
  /// otherwise, <see langword="false"/>.
  /// </param>
  /// <param name="cancellationToken">
  /// The <see cref="CancellationToken"/> to monitor for cancellation requests.
  /// </param>
  /// <returns>
  /// A <see cref="ValueTask{TResult}"/> that represents the asynchronous operation.
  /// The result of the task is an <see cref="IUsbHidEndPoint"/> that represents the opened endpoints.
  /// </returns>
  /// <exception cref="InvalidOperationException">
  /// Attempted to open an endpoint, but neither <paramref name="openOutEndPoint"/> nor
  /// <paramref name="openInEndPoint"/> was specified.
  /// </exception>
  /// <exception cref="ObjectDisposedException">
  /// This device instance has been disposed.
  /// </exception>
  /// <exception cref="OperationCanceledException">
  /// The <paramref name="cancellationToken"/> has had cancellation requested.
  /// </exception>
  /// <remarks>
  /// If the underlying implementation does not support asynchronous opening,
  /// this method will perform a synchronous open instead.
  /// </remarks>
  ValueTask<IUsbHidEndPoint> OpenEndPointAsync(
    bool openOutEndPoint,
    bool openInEndPoint,
    bool shouldDisposeDevice,
    CancellationToken cancellationToken
  );
}
