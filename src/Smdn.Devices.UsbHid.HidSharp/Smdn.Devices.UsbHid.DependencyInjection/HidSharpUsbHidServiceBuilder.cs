// SPDX-FileCopyrightText: 2026 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
#if false
using Microsoft.Extensions.Options;
#endif

namespace Smdn.Devices.UsbHid.DependencyInjection;

/// <summary>
/// A builder for <see cref="HidSharpUsbHidService"/>.
/// </summary>
[CLSCompliant(false)]
public sealed class HidSharpUsbHidServiceBuilder<TServiceKey> : UsbHidServiceBuilder<TServiceKey> {
  internal HidSharpUsbHidServiceBuilder(
    IServiceCollection services,
    TServiceKey serviceKey,
    Func<TServiceKey, string?> selectOptionsNameForServiceKey
  ) : base(
      services: services,
      serviceKey: serviceKey,
      selectOptionsNameForServiceKey: selectOptionsNameForServiceKey
    )
  {
  }

  /// <inheritdoc/>
  public override IUsbHidService Build(
    IServiceProvider serviceProvider
  )
  {
    if (serviceProvider is null)
      throw new ArgumentNullException(nameof(serviceProvider));

#if false
    var options = (serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider)))
      .GetRequiredService<IOptionsMonitor<HidSharpOptions>>()
      .Get(name: GetOptionsName());
#endif

    return new HidSharpUsbHidService(
      resiliencePipelineProvider: serviceProvider.GetResiliencePipelineProviderForHidSharpUsbHidService(
        serviceKey: ServiceKey
      ),
      loggerFactory:
        // Attempt to get the ILoggerFactory with the specified service key
        serviceProvider.GetKeyedService<ILoggerFactory>(ServiceKey) ??
        // After that, attempt to get the default ILoggerFactory
        serviceProvider.GetService<ILoggerFactory>()
    );
  }
}
