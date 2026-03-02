// SPDX-FileCopyrightText: 2026 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

using Smdn.IO.UsbHid;

namespace Smdn.Devices.MCP2221;

#pragma warning disable IDE0040, CA1724
partial class Mcp2221 {
#pragma warning restore IDE0040, CA1724
  public static ValueTask<Mcp2221> CreateAsync(
    IServiceProvider serviceProvider,
    CancellationToken cancellationToken = default
  )
    => CreateWithServiceProviderAsyncCore(
      serviceProvider: serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider)),
      serviceKey: (object?)null,
      predicate: null,
      cancellationToken: cancellationToken
    );

  public static Mcp2221 Create(
    IServiceProvider serviceProvider,
    CancellationToken cancellationToken = default
  )
    => CreateWithServiceProviderCore(
      serviceProvider: serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider)),
      serviceKey: (object?)null,
      predicate: null,
      cancellationToken: cancellationToken
    );

  public static ValueTask<Mcp2221> CreateAsync<TServiceKey>(
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

  public static Mcp2221 Create<TServiceKey>(
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

  private static IMcp2221UsbHidDeviceFactory GetUsbHidDeviceFactoryFrom<TServiceKey>(
    IServiceProvider? serviceProvider,
    TServiceKey serviceKey
  )
    =>
      serviceProvider?.GetKeyedService<IMcp2221UsbHidDeviceFactory>(serviceKey) ??
      Mcp2221DefaultUsbHidDeviceFactory.Instance; // fallback to default factory

  private static ValueTask<Mcp2221> CreateWithServiceProviderAsyncCore<TServiceKey>(
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

  private static Mcp2221 CreateWithServiceProviderCore<TServiceKey>(
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
