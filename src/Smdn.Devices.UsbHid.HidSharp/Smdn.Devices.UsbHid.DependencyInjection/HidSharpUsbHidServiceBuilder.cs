// SPDX-FileCopyrightText: 2026 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
#if false
using Microsoft.Extensions.Options;
#endif

namespace Smdn.Devices.UsbHid.DependencyInjection;

internal sealed class HidSharpUsbHidServiceBuilder<TServiceKey>(
  IServiceCollection services,
  TServiceKey serviceKey,
  Func<TServiceKey, string?> selectOptionsNameForServiceKey
)
: UsbHidServiceBuilder<TServiceKey>(
  services: services,
  serviceKey: serviceKey,
  selectOptionsNameForServiceKey: selectOptionsNameForServiceKey
) {
  private readonly TServiceKey serviceKey = serviceKey;

  public override IUsbHidService Build(
    IServiceProvider serviceProvider
  )
  {
#if false
    var options = (serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider)))
      .GetRequiredService<IOptionsMonitor<HidSharpOptions>>()
      .Get(name: GetOptionsName());
#endif

    return new UsbHidService(
      logger:
        // Attempt to get the ILoggerFactory with the specified service key
        serviceProvider.GetKeyedService<ILoggerFactory>(serviceKey)?.CreateLogger<UsbHidService>() ??
        // After that, attempt to get the default ILoggerFactory
        serviceProvider.GetService<ILoggerFactory>()?.CreateLogger<UsbHidService>(),
      serviceProvider: serviceProvider,
      serviceKey: serviceKey
    );
  }
}
