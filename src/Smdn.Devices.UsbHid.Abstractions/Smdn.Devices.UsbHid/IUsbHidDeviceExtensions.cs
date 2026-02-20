// SPDX-FileCopyrightText: 2026 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Smdn.Devices.UsbHid;

/// <summary>
/// Provides extension methods for <see cref="IUsbHidDevice"/>.
/// </summary>
public static class IUsbHidDeviceExtensions {
  /// <summary>
  /// Opens both the <c>OUT</c> and <c>IN</c> endpoints of the specified
  /// <see cref="IUsbHidDevice"/> with default options.
  /// </summary>
  /// <param name="device">
  /// The <see cref="IUsbHidDevice"/> for which to open the endpoints.
  /// </param>
  /// <param name="cancellationToken">
  /// The <see cref="CancellationToken"/> to monitor for cancellation requests.
  /// </param>
  /// <returns>
  /// An <see cref="IUsbHidEndPoint"/> that represents the opened endpoints.
  /// </returns>
  /// <exception cref="ArgumentNullException">
  /// <paramref name="device"/> is <see langword="null"/>.
  /// </exception>
  /// <seealso cref="IUsbHidDevice.OpenEndPoint(bool, bool, bool, CancellationToken)"/>
  public static IUsbHidEndPoint OpenEndPoint(
    this IUsbHidDevice device,
    CancellationToken cancellationToken = default
  )
    => (device ?? throw new ArgumentNullException(nameof(device)))
      .OpenEndPoint(
        openOutEndPoint: true,
        openInEndPoint: true,
        shouldDisposeDevice: false,
        cancellationToken: cancellationToken
      );

  /// <summary>
  /// Opens both the <c>OUT</c> and <c>IN</c> endpoints of the specified
  /// <see cref="IUsbHidDevice"/> with default options.
  /// </summary>
  /// <param name="device">
  /// The <see cref="IUsbHidDevice"/> for which to open the endpoints.
  /// </param>
  /// <param name="shouldDisposeDevice">
  /// <see langword="true"/> to dispose the <paramref name="device"/> instance when
  /// the returned <see cref="IUsbHidEndPoint"/> is disposed;
  /// otherwise, <see langword="false"/>.
  /// </param>
  /// <returns>
  /// An <see cref="IUsbHidEndPoint"/> that represents the opened endpoints.
  /// </returns>
  /// <exception cref="ArgumentNullException">
  /// <paramref name="device"/> is <see langword="null"/>.
  /// </exception>
  /// <seealso cref="IUsbHidDevice.OpenEndPoint(bool, bool, bool, CancellationToken)"/>
  public static IUsbHidEndPoint OpenEndPoint(
    this IUsbHidDevice device,
    bool shouldDisposeDevice
  )
    => (device ?? throw new ArgumentNullException(nameof(device)))
      .OpenEndPoint(
        openOutEndPoint: true,
        openInEndPoint: true,
        shouldDisposeDevice: shouldDisposeDevice,
        cancellationToken: default
      );

  /// <summary>
  /// Opens both the <c>OUT</c> and <c>IN</c> endpoints of the specified
  /// <see cref="IUsbHidDevice"/> with default options.
  /// </summary>
  /// <param name="device">
  /// The <see cref="IUsbHidDevice"/> for which to open the endpoints.
  /// </param>
  /// <param name="shouldDisposeDevice">
  /// <see langword="true"/> to dispose the <paramref name="device"/> instance when
  /// the returned <see cref="IUsbHidEndPoint"/> is disposed;
  /// otherwise, <see langword="false"/>.
  /// </param>
  /// <param name="cancellationToken">
  /// The <see cref="CancellationToken"/> to monitor for cancellation requests.
  /// </param>
  /// <returns>
  /// An <see cref="IUsbHidEndPoint"/> that represents the opened endpoints.
  /// </returns>
  /// <exception cref="ArgumentNullException">
  /// <paramref name="device"/> is <see langword="null"/>.
  /// </exception>
  /// <seealso cref="IUsbHidDevice.OpenEndPoint(bool, bool, bool, CancellationToken)"/>
  public static IUsbHidEndPoint OpenEndPoint(
    this IUsbHidDevice device,
    bool shouldDisposeDevice,
    CancellationToken cancellationToken = default
  )
    => (device ?? throw new ArgumentNullException(nameof(device)))
      .OpenEndPoint(
        openOutEndPoint: true,
        openInEndPoint: true,
        shouldDisposeDevice: shouldDisposeDevice,
        cancellationToken: cancellationToken
      );

  /// <summary>
  /// Asynchronously opens both the <c>OUT</c> and <c>IN</c> endpoints of the
  /// specified <see cref="IUsbHidDevice"/> with default options.
  /// </summary>
  /// <param name="device">
  /// The <see cref="IUsbHidDevice"/> for which to open the endpoints.
  /// </param>
  /// <param name="cancellationToken">
  /// The <see cref="CancellationToken"/> to monitor for cancellation requests.
  /// </param>
  /// <returns>
  /// A <see cref="ValueTask{TResult}"/> that represents the asynchronous operation.
  /// The result of the task is an <see cref="IUsbHidEndPoint"/> that represents the opened endpoints.
  /// </returns>
  /// <exception cref="ArgumentNullException">
  /// <paramref name="device"/> is <see langword="null"/>.
  /// </exception>
  /// <seealso cref="IUsbHidDevice.OpenEndPointAsync(bool, bool, bool, CancellationToken)"/>
  public static ValueTask<IUsbHidEndPoint> OpenEndPointAsync(
    this IUsbHidDevice device,
    CancellationToken cancellationToken = default
  )
    => (device ?? throw new ArgumentNullException(nameof(device)))
      .OpenEndPointAsync(
        openOutEndPoint: true,
        openInEndPoint: true,
        shouldDisposeDevice: false,
        cancellationToken: cancellationToken
      );

  /// <summary>
  /// Asynchronously opens both the <c>OUT</c> and <c>IN</c> endpoints of the
  /// specified <see cref="IUsbHidDevice"/> with default options.
  /// </summary>
  /// <param name="device">
  /// The <see cref="IUsbHidDevice"/> for which to open the endpoints.
  /// </param>
  /// <param name="shouldDisposeDevice">
  /// <see langword="true"/> to dispose the <paramref name="device"/> instance when
  /// the returned <see cref="IUsbHidEndPoint"/> is disposed;
  /// otherwise, <see langword="false"/>.
  /// </param>
  /// <returns>
  /// A <see cref="ValueTask{TResult}"/> that represents the asynchronous operation.
  /// The result of the task is an <see cref="IUsbHidEndPoint"/> that represents the opened endpoints.
  /// </returns>
  /// <exception cref="ArgumentNullException">
  /// <paramref name="device"/> is <see langword="null"/>.
  /// </exception>
  /// <seealso cref="IUsbHidDevice.OpenEndPointAsync(bool, bool, bool, CancellationToken)"/>
  public static ValueTask<IUsbHidEndPoint> OpenEndPointAsync(
    this IUsbHidDevice device,
    bool shouldDisposeDevice
  )
    => (device ?? throw new ArgumentNullException(nameof(device)))
      .OpenEndPointAsync(
        openOutEndPoint: true,
        openInEndPoint: true,
        shouldDisposeDevice: shouldDisposeDevice,
        cancellationToken: default
      );

  /// <summary>
  /// Asynchronously opens both the <c>OUT</c> and <c>IN</c> endpoints of the
  /// specified <see cref="IUsbHidDevice"/> with default options.
  /// </summary>
  /// <param name="device">
  /// The <see cref="IUsbHidDevice"/> for which to open the endpoints.
  /// </param>
  /// <param name="shouldDisposeDevice">
  /// <see langword="true"/> to dispose the <paramref name="device"/> instance when
  /// the returned <see cref="IUsbHidEndPoint"/> is disposed;
  /// otherwise, <see langword="false"/>.
  /// </param>
  /// <param name="cancellationToken">
  /// The <see cref="CancellationToken"/> to monitor for cancellation requests.
  /// </param>
  /// <returns>
  /// A <see cref="ValueTask{TResult}"/> that represents the asynchronous operation.
  /// The result of the task is an <see cref="IUsbHidEndPoint"/> that represents the opened endpoints.
  /// </returns>
  /// <exception cref="ArgumentNullException">
  /// <paramref name="device"/> is <see langword="null"/>.
  /// </exception>
  /// <seealso cref="IUsbHidDevice.OpenEndPointAsync(bool, bool, bool, CancellationToken)"/>
  public static ValueTask<IUsbHidEndPoint> OpenEndPointAsync(
    this IUsbHidDevice device,
    bool shouldDisposeDevice,
    CancellationToken cancellationToken = default
  )
    => (device ?? throw new ArgumentNullException(nameof(device)))
      .OpenEndPointAsync(
        openOutEndPoint: true,
        openInEndPoint: true,
        shouldDisposeDevice: shouldDisposeDevice,
        cancellationToken: cancellationToken
      );

  /// <summary>
  /// Gets an implementation-dependent string that identifies the device.
  /// </summary>
  /// <param name="device">
  /// The <see cref="IUsbHidDevice"/> to get the identifier for.
  /// </param>
  /// <returns>
  /// An implementation-dependent string that can be used to identify the device.
  /// </returns>
  /// <remarks>
  /// <para>
  /// This method tries to return a string that can be used to uniquely identify the device.
  /// The format of the returned string is implementation-dependent.
  /// </para>
  /// <para>
  /// It may return a path to a device file, a string representation of the bus and port number,
  /// a composite string of the vendor ID, product ID, and serial number, etc.
  /// The format is not guaranteed to be consistent across different platforms or backend libraries.
  /// </para>
  /// </remarks>
  /// <seealso cref="IUsbHidDevice.TryGetDeviceIdentifier(out string?)"/>
  /// <seealso cref="IUsbHidDevice.TryGetSerialNumber(out string?)"/>
  public static string ToIdentificationString(this IUsbHidDevice device)
  {
    if (device is null)
      throw new ArgumentNullException(nameof(device));

    if (
      device.TryGetDeviceIdentifier(out var deviceIdentifier) &&
#if NULL_STATE_STATIC_ANALYSIS_ATTRIBUTES
      0 < deviceIdentifier.Length
#else
      0 < deviceIdentifier!.Length
#endif
    ) {
      return deviceIdentifier;
    }

    if (
      device.TryGetSerialNumber(out var serialNumber) &&
#if NULL_STATE_STATIC_ANALYSIS_ATTRIBUTES
      0 < serialNumber.Length
#else
      0 < serialNumber!.Length
#endif
    ) {
      return $"{{{device.VendorId:X4}:{device.ProductId:X4};{serialNumber}}}";
    }

    return device.ToString() ?? $"{{{device.VendorId:X4}:{device.ProductId:X4}}}";
  }
}
