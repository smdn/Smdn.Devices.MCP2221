// SPDX-FileCopyrightText: 2026 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.Threading;
using System.Threading.Tasks;

using Smdn.IO.UsbHid;

namespace Smdn.Devices.Mcp2221A;

#pragma warning disable IDE0040, CA1724
partial class Mcp2221A {
#pragma warning restore IDE0040, CA1724
  public static ValueTask<Mcp2221A> CreateAsync(
    IMcp2221AUsbHidDeviceFactory usbHidDeviceFactory,
    IServiceProvider? serviceProvider = null,
    CancellationToken cancellationToken = default
  )
    => CreateWithDeviceFactoryAsyncCore(
      usbHidDeviceFactory: usbHidDeviceFactory ?? throw new ArgumentNullException(nameof(usbHidDeviceFactory)),
      serviceProvider: serviceProvider,
      serviceKey: (object?)null,
      predicate: null,
      cancellationToken: cancellationToken
    );

  public static Mcp2221A Create(
    IMcp2221AUsbHidDeviceFactory usbHidDeviceFactory,
    IServiceProvider? serviceProvider = null,
    CancellationToken cancellationToken = default
  )
    => CreateWithDeviceFactoryCore(
      usbHidDeviceFactory: usbHidDeviceFactory ?? throw new ArgumentNullException(nameof(usbHidDeviceFactory)),
      serviceProvider: serviceProvider,
      serviceKey: (object?)null,
      predicate: null,
      cancellationToken: cancellationToken
    );

  public static ValueTask<Mcp2221A> CreateAsync<TServiceKey>(
    IMcp2221AUsbHidDeviceFactory usbHidDeviceFactory,
    IServiceProvider serviceProvider,
    TServiceKey serviceKey,
    CancellationToken cancellationToken = default
  )
    => CreateWithDeviceFactoryAsyncCore(
      usbHidDeviceFactory: usbHidDeviceFactory ?? throw new ArgumentNullException(nameof(usbHidDeviceFactory)),
      serviceProvider: serviceProvider,
      serviceKey: serviceKey,
      predicate: null,
      cancellationToken: cancellationToken
    );

  public static Mcp2221A Create<TServiceKey>(
    IMcp2221AUsbHidDeviceFactory usbHidDeviceFactory,
    IServiceProvider serviceProvider,
    TServiceKey serviceKey,
    CancellationToken cancellationToken = default
  )
    => CreateWithDeviceFactoryCore(
      usbHidDeviceFactory: usbHidDeviceFactory ?? throw new ArgumentNullException(nameof(usbHidDeviceFactory)),
      serviceProvider: serviceProvider,
      serviceKey: serviceKey,
      predicate: null,
      cancellationToken: cancellationToken
    );

  private static async ValueTask<Mcp2221A> CreateWithDeviceFactoryAsyncCore<TServiceKey>(
    IMcp2221AUsbHidDeviceFactory usbHidDeviceFactory,
    IServiceProvider? serviceProvider,
    TServiceKey? serviceKey,
    Predicate<IUsbHidDevice>? predicate,
    CancellationToken cancellationToken = default
  )
    => await CreateFromUsbHidDeviceAsyncCore(
      usbHidDevice: await usbHidDeviceFactory.CreateAsync(
        serviceProvider,
        serviceKey,
        predicate,
        cancellationToken
      ).ConfigureAwait(false),
      shouldDisposeUsbHidDevice: true,
      serviceProvider: serviceProvider,
      serviceKey: serviceKey,
      cancellationToken: cancellationToken
    ).ConfigureAwait(false);

  private static Mcp2221A CreateWithDeviceFactoryCore<TServiceKey>(
    IMcp2221AUsbHidDeviceFactory usbHidDeviceFactory,
    IServiceProvider? serviceProvider,
    TServiceKey? serviceKey,
    Predicate<IUsbHidDevice>? predicate,
    CancellationToken cancellationToken = default
  )
    => CreateFromUsbHidDeviceCore(
      usbHidDevice: usbHidDeviceFactory.Create(
        serviceProvider,
        serviceKey,
        predicate,
        cancellationToken
      ),
      shouldDisposeUsbHidDevice: true,
      serviceProvider: serviceProvider,
      serviceKey: serviceKey,
      cancellationToken: cancellationToken
    );
}
