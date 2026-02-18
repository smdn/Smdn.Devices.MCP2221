// SPDX-FileCopyrightText: 2026 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;

using Microsoft.Extensions.DependencyInjection;

namespace Smdn.Devices.UsbHid.DependencyInjection;

public static class LibUsbDotNetServiceCollectionExtensions {
#pragma warning disable IDE0060
  private static void ConfigureNothing<TServiceKey>(UsbHidServiceBuilder<TServiceKey> builder, LibUsbDotNetOptions options)
  {
    // do nothing
  }
#pragma warning restore IDE0060

  [CLSCompliant(false)]
  public static IServiceCollection AddLibUsbDotNetUsbHid(
    this IServiceCollection services
  )
    => AddLibUsbDotNetUsbHid<object?>(
      services: services ?? throw new ArgumentNullException(nameof(services)),
      serviceKey: null,
      selectOptionsNameForServiceKey: static _ => string.Empty /* Options.DefaultName */,
      configure: ConfigureNothing
    );

  [CLSCompliant(false)]
  public static IServiceCollection AddLibUsbDotNetUsbHid(
    this IServiceCollection services,
    Action<UsbHidServiceBuilder<object?>, LibUsbDotNetOptions> configure
  )
    => AddLibUsbDotNetUsbHid(
      services: services ?? throw new ArgumentNullException(nameof(services)),
      serviceKey: null,
      selectOptionsNameForServiceKey: static _ => string.Empty /* Options.DefaultName */,
      configure: configure ?? throw new ArgumentNullException(nameof(configure))
    );

  [CLSCompliant(false)]
  public static IServiceCollection AddLibUsbDotNetUsbHid(
    this IServiceCollection services,
    string serviceKey
  )
    => AddLibUsbDotNetUsbHid(
      services: services ?? throw new ArgumentNullException(nameof(services)),
      serviceKey: serviceKey,
      selectOptionsNameForServiceKey: static key => key,
      configure: ConfigureNothing
    );

  [CLSCompliant(false)]
  public static IServiceCollection AddLibUsbDotNetUsbHid(
    this IServiceCollection services,
    string serviceKey,
    Action<UsbHidServiceBuilder<string>, LibUsbDotNetOptions> configure
  )
    => AddLibUsbDotNetUsbHid(
      services: services ?? throw new ArgumentNullException(nameof(services)),
      serviceKey: serviceKey,
      selectOptionsNameForServiceKey: static key => key,
      configure: configure ?? throw new ArgumentNullException(nameof(configure))
    );

  [CLSCompliant(false)]
  public static IServiceCollection AddLibUsbDotNetUsbHid<TServiceKey>(
    this IServiceCollection services,
    TServiceKey serviceKey,
    Func<TServiceKey, string?> selectOptionsNameForServiceKey
  )
    => AddLibUsbDotNetUsbHid(
      services: services ?? throw new ArgumentNullException(nameof(services)),
      serviceKey: serviceKey,
      selectOptionsNameForServiceKey: selectOptionsNameForServiceKey ?? throw new ArgumentNullException(nameof(selectOptionsNameForServiceKey)),
      configure: ConfigureNothing
    );

  [CLSCompliant(false)]
  public static IServiceCollection AddLibUsbDotNetUsbHid<TServiceKey>(
    this IServiceCollection services,
    TServiceKey serviceKey,
    Func<TServiceKey, string?> selectOptionsNameForServiceKey,
    Action<UsbHidServiceBuilder<TServiceKey>, LibUsbDotNetOptions> configure
  )
  {
    if (services is null)
      throw new ArgumentNullException(nameof(services));
    if (selectOptionsNameForServiceKey is null)
      throw new ArgumentNullException(nameof(selectOptionsNameForServiceKey));
    if (configure is null)
      throw new ArgumentNullException(nameof(configure));

    var builder = new LibUsbDotNetUsbHidServiceBuilder<TServiceKey>(
      services,
      serviceKey,
      selectOptionsNameForServiceKey
    );
    var configuredOptions = new LibUsbDotNetOptions();

    configure(builder, configuredOptions);

    _ = services.Configure<LibUsbDotNetOptions>(
      name: builder.GetOptionsName(),
      configureOptions: options => options.Configure(configuredOptions)
    );

    services.Add(
      ServiceDescriptor.KeyedSingleton/* <LibUsbDotNetServiceBuilder<TServiceKey>> */(
        serviceKey: builder.ServiceKey,
        implementationFactory: (_, _) => builder
      )
    );

    services.Add(
      ServiceDescriptor.KeyedSingleton/* <IUsbService> */(
        serviceKey: builder.ServiceKey,
        static (serviceProvider, serviceKey)
          => serviceProvider
            .GetRequiredKeyedService<LibUsbDotNetUsbHidServiceBuilder<TServiceKey>>(serviceKey)
            .Build(serviceProvider)
      )
    );

    return services;
  }
}
