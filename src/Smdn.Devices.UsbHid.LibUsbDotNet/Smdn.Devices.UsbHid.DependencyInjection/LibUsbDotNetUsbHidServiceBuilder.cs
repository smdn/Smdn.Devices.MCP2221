// SPDX-FileCopyrightText: 2026 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Smdn.Devices.UsbHid.DependencyInjection;

internal sealed class LibUsbDotNetUsbHidServiceBuilder<TServiceKey>(
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
    var options = (serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider)))
      .GetRequiredService<IOptionsMonitor<LibUsbDotNetOptions>>()
      .Get(name: GetOptionsName());

    return new UsbHidService(
      options: options,
      logger:
        // Attempt to get the ILoggerFactory with the specified service key
        serviceProvider.GetKeyedService<ILoggerFactory>(serviceKey)?.CreateLogger<UsbHidService>() ??
        // After that, attempt to get the default ILoggerFactory
        serviceProvider.GetService<ILoggerFactory>()?.CreateLogger<UsbHidService>()
    );
  }
}
