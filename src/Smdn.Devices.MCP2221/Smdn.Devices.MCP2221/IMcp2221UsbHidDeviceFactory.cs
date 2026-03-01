// SPDX-FileCopyrightText: 2026 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

using Smdn.IO.UsbHid;

namespace Smdn.Devices.MCP2221;

/// <summary>
/// Provides a mechanism to select the MCP2221/MCP2221A available on the system and
/// create the corresponding <see cref="IUsbHidDevice"/>.
/// </summary>
public interface IMcp2221UsbHidDeviceFactory {
  /// <summary>
  /// Asynchronously creates an <see cref="IUsbHidDevice"/> corresponding to the MCP2221/MCP2221A
  /// available on the system.
  /// </summary>
  /// <typeparam name="TServiceKey">
  /// Specifies the type of the <paramref name="serviceKey"/>.
  /// </typeparam>
  /// <param name="serviceProvider">
  /// Specifies the search for <see cref="IUsbHidDevice"/> or the <see cref="IServiceProvider"/>
  /// passed to the created <see cref="IUsbHidDevice"/>.
  /// </param>
  /// <param name="serviceKey">
  /// The <see cref="ServiceDescriptor.ServiceKey"/> specified when getting services
  /// from the <see paramref="serviceProvider"/>.
  /// Specify <see langword="null"/> when getting services without using a service key.
  /// This value is also passed to the created <see cref="IUsbHidDevice"/> along with
  /// the value of <paramref name="serviceProvider"/>.
  /// </param>
  /// <param name="predicate">
  /// Specifies additional criteria for selecting an <see cref="IUsbHidDevice"/> corresponding
  /// to the MCP2221/MCP2221A.
  /// If <see langword="null"/> is specified, the first enumerated <see cref="IUsbHidDevice"/>
  /// is selected.
  /// </param>
  /// <param name="cancellationToken">
  /// The <see cref="CancellationToken"/> to monitor for cancellation requests.
  /// </param>
  /// <returns>
  /// The <see cref="ValueTask{T}"/> that represents the asynchronous operation.
  /// The value of its <see cref="ValueTask{T}.Result"/> property contains
  /// the <see cref="IUsbHidDevice"/> corresponding to the MCP2221/MCP2221A that
  /// matches the specified <paramref name="predicate"/>.
  /// </returns>
  /// <exception cref="InvalidOperationException">
  /// This implementation requires an <see cref="IServiceProvider"/> to create an
  /// <see cref="IUsbHidDevice"/> corresponding to the MCP2221/MCP2221A,
  /// but <paramref name="serviceProvider"/> is <see langword="null"/>.
  /// </exception>
  /// <exception cref="DeviceNotFoundException">
  /// No MCP2221/MCP2221A <see cref="IUsbHidDevice"/> matching the specified
  /// <paramref name="predicate"/> was found.
  /// Or, the available MCP2221/MCP2221A was not found on the current system.
  /// </exception>
  /// <remarks>
  /// If the implementation does not support asynchronous selection or creation,
  /// this method will perform a synchronous creation instead.
  /// </remarks>
  ValueTask<IUsbHidDevice> CreateAsync<TServiceKey>(
    IServiceProvider? serviceProvider,
    TServiceKey? serviceKey,
    Predicate<IUsbHidDevice>? predicate,
    CancellationToken cancellationToken
  );

  /// <summary>
  /// Creates an <see cref="IUsbHidDevice"/> corresponding to the MCP2221/MCP2221A
  /// available on the system.
  /// </summary>
  /// <typeparam name="TServiceKey">
  /// Specifies the type of the <paramref name="serviceKey"/>.
  /// </typeparam>
  /// <param name="serviceProvider">
  /// Specifies the search for <see cref="IUsbHidDevice"/> or the <see cref="IServiceProvider"/>
  /// passed to the created <see cref="IUsbHidDevice"/>.
  /// </param>
  /// <param name="serviceKey">
  /// The <see cref="ServiceDescriptor.ServiceKey"/> specified when getting services
  /// from the <see paramref="serviceProvider"/>.
  /// Specify <see langword="null"/> when getting services without using a service key.
  /// This value is also passed to the created <see cref="IUsbHidDevice"/> along with
  /// the value of <paramref name="serviceProvider"/>.
  /// </param>
  /// <param name="predicate">
  /// Specifies additional criteria for selecting an <see cref="IUsbHidDevice"/> corresponding
  /// to the MCP2221/MCP2221A.
  /// If <see langword="null"/> is specified, the first enumerated <see cref="IUsbHidDevice"/>
  /// is selected.
  /// </param>
  /// <param name="cancellationToken">
  /// The <see cref="CancellationToken"/> to monitor for cancellation requests.
  /// </param>
  /// <returns>
  /// Returns the MCP2221/MCP2221A <see cref="IUsbHidDevice"/> that matches
  /// the specified <paramref name="predicate"/>.
  /// </returns>
  /// <exception cref="InvalidOperationException">
  /// This implementation requires an <see cref="IServiceProvider"/> to create an
  /// <see cref="IUsbHidDevice"/> corresponding to the MCP2221/MCP2221A,
  /// but <paramref name="serviceProvider"/> is <see langword="null"/>.
  /// </exception>
  /// <exception cref="DeviceNotFoundException">
  /// No MCP2221/MCP2221A <see cref="IUsbHidDevice"/> matching the specified
  /// <paramref name="predicate"/> was found.
  /// Or, the available MCP2221/MCP2221A was not found on the current system.
  /// </exception>
  IUsbHidDevice Create<TServiceKey>(
    IServiceProvider? serviceProvider,
    TServiceKey? serviceKey,
    Predicate<IUsbHidDevice>? predicate,
    CancellationToken cancellationToken
  );
}
