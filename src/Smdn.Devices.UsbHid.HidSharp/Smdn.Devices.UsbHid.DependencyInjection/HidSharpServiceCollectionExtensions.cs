// SPDX-FileCopyrightText: 2026 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;

using Microsoft.Extensions.DependencyInjection;

namespace Smdn.Devices.UsbHid.DependencyInjection;

public static class HidSharpServiceCollectionExtensions {
#pragma warning disable IDE0060
  private static void ConfigureNothing<TServiceKey>(UsbHidServiceBuilder<TServiceKey> builder)
  {
    // do nothing
  }
#pragma warning restore IDE0060

  [CLSCompliant(false)]
  public static IServiceCollection AddHidSharpUsbHid(
    this IServiceCollection services
  )
    => AddHidSharpUsbHid<object?>(
      services: services ?? throw new ArgumentNullException(nameof(services)),
      serviceKey: null,
      selectOptionsNameForServiceKey: static _ => string.Empty, /* Options.DefaultName */
      configure: ConfigureNothing
    );

  [CLSCompliant(false)]
  public static IServiceCollection AddHidSharpUsbHid(
    this IServiceCollection services,
    Action<UsbHidServiceBuilder<object?>> configure
  )
    => AddHidSharpUsbHid<object?>(
      services: services ?? throw new ArgumentNullException(nameof(services)),
      serviceKey: null,
      selectOptionsNameForServiceKey: static _ => string.Empty, /* Options.DefaultName */
      configure: configure ?? throw new ArgumentNullException(nameof(configure))
    );

  [CLSCompliant(false)]
  public static IServiceCollection AddHidSharpUsbHid(
    this IServiceCollection services,
    string serviceKey
  )
    => AddHidSharpUsbHid(
      services: services ?? throw new ArgumentNullException(nameof(services)),
      serviceKey: serviceKey,
      selectOptionsNameForServiceKey: static key => key,
      configure: ConfigureNothing
    );

  [CLSCompliant(false)]
  public static IServiceCollection AddHidSharpUsbHid(
    this IServiceCollection services,
    string serviceKey,
    Action<UsbHidServiceBuilder<string>> configure
  )
    => AddHidSharpUsbHid(
      services: services ?? throw new ArgumentNullException(nameof(services)),
      serviceKey: serviceKey,
      selectOptionsNameForServiceKey: static key => key,
      configure: configure ?? throw new ArgumentNullException(nameof(configure))
    );

  [CLSCompliant(false)]
  public static IServiceCollection AddHidSharpUsbHid<TServiceKey>(
    this IServiceCollection services,
    TServiceKey serviceKey,
    Func<TServiceKey, string?> selectOptionsNameForServiceKey
  )
    => AddHidSharpUsbHid(
      services: services ?? throw new ArgumentNullException(nameof(services)),
      serviceKey: serviceKey,
      selectOptionsNameForServiceKey: selectOptionsNameForServiceKey ?? throw new ArgumentNullException(nameof(selectOptionsNameForServiceKey)),
      configure: ConfigureNothing
    );

  [CLSCompliant(false)]
  public static IServiceCollection AddHidSharpUsbHid<TServiceKey>(
    this IServiceCollection services,
    TServiceKey serviceKey,
    Func<TServiceKey, string?> selectOptionsNameForServiceKey,
    Action<UsbHidServiceBuilder<TServiceKey>> configure
  )
  {
    if (services is null)
      throw new ArgumentNullException(nameof(services));
    if (selectOptionsNameForServiceKey is null)
      throw new ArgumentNullException(nameof(selectOptionsNameForServiceKey));
    if (configure is null)
      throw new ArgumentNullException(nameof(configure));

    var builder = new HidSharpUsbHidServiceBuilder<TServiceKey>(
      services,
      serviceKey,
      selectOptionsNameForServiceKey
    );
#if false // for future extension
    var configuredOptions = new HidSharpOptions();

    configure(builder, configuredOptions);
#endif

    configure(builder);

#if false // for future extension
    _ = services.Configure<HidSharpOptions>(
      name: builder.GetOptionsName(),
      configureOptions: options => options.Configure(configuredOptions)
    );
#endif

    services.Add(
      ServiceDescriptor.KeyedSingleton/* <HidSharpServiceBuilder<TServiceKey>> */(
        serviceKey: builder.ServiceKey,
        implementationFactory: (_, _) => builder
      )
    );

    services.Add(
      ServiceDescriptor.KeyedSingleton/* <IUsbService> */(
        serviceKey: builder.ServiceKey,
        static (serviceProvider, serviceKey)
          => serviceProvider
            .GetRequiredKeyedService<HidSharpUsbHidServiceBuilder<TServiceKey>>(serviceKey)
            .Build(serviceProvider)
      )
    );

    return services;
  }
}
