// SPDX-FileCopyrightText: 2026 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

using Smdn.IO.UsbHid;

namespace Smdn.Devices.Mcp2221A;

#pragma warning disable IDE0040, CA1724
partial class Mcp2221A {
#pragma warning restore IDE0040, CA1724
  public static ValueTask<Mcp2221A> CreateAsync(
    IServiceProvider serviceProvider,
    CancellationToken cancellationToken = default
  )
    => CreateWithServiceProviderAsyncCore(
      serviceProvider: serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider)),
      serviceKey: (object?)null,
      predicate: null,
      cancellationToken: cancellationToken
    );

  public static Mcp2221A Create(
    IServiceProvider serviceProvider,
    CancellationToken cancellationToken = default
  )
    => CreateWithServiceProviderCore(
      serviceProvider: serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider)),
      serviceKey: (object?)null,
      predicate: null,
      cancellationToken: cancellationToken
    );

  public static ValueTask<Mcp2221A> CreateAsync<TServiceKey>(
    IServiceProvider serviceProvider,
    TServiceKey serviceKey,
    CancellationToken cancellationToken = default
  )
    => CreateWithServiceProviderAsyncCore(
      serviceProvider: serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider)),
      serviceKey: serviceKey,
      predicate: null,
      cancellationToken: cancellationToken
    );

  public static Mcp2221A Create<TServiceKey>(
    IServiceProvider serviceProvider,
    TServiceKey serviceKey,
    CancellationToken cancellationToken = default
  )
    => CreateWithServiceProviderCore(
      serviceProvider: serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider)),
      serviceKey: serviceKey,
      predicate: null,
      cancellationToken: cancellationToken
    );

  private static IMcp2221AUsbHidDeviceFactory GetUsbHidDeviceFactoryFrom<TServiceKey>(
    IServiceProvider? serviceProvider,
    TServiceKey serviceKey
  )
    =>
      serviceProvider?.GetKeyedService<IMcp2221AUsbHidDeviceFactory>(serviceKey) ??
      Mcp2221ADefaultUsbHidDeviceFactory.Instance; // fallback to default factory

  private static ValueTask<Mcp2221A> CreateWithServiceProviderAsyncCore<TServiceKey>(
    IServiceProvider? serviceProvider,
    TServiceKey? serviceKey,
    Predicate<IUsbHidDevice>? predicate,
    CancellationToken cancellationToken = default
  )
    => CreateWithDeviceFactoryAsyncCore(
      usbHidDeviceFactory: GetUsbHidDeviceFactoryFrom(serviceProvider, serviceKey),
      serviceProvider: serviceProvider,
      serviceKey: serviceKey,
      predicate: predicate,
      cancellationToken: cancellationToken
    );

  private static Mcp2221A CreateWithServiceProviderCore<TServiceKey>(
    IServiceProvider? serviceProvider,
    TServiceKey? serviceKey,
    Predicate<IUsbHidDevice>? predicate,
    CancellationToken cancellationToken = default
  )
    => CreateWithDeviceFactoryCore(
      usbHidDeviceFactory: GetUsbHidDeviceFactoryFrom(serviceProvider, serviceKey),
      serviceProvider: serviceProvider,
      serviceKey: serviceKey,
      predicate: predicate,
      cancellationToken: cancellationToken
    );
}
