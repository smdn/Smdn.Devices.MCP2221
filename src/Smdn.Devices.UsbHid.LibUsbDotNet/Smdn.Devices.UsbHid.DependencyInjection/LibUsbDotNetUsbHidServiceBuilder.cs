// SPDX-FileCopyrightText: 2026 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Smdn.Devices.UsbHid.DependencyInjection;

/// <summary>
/// A builder for <see cref="LibUsbDotNetUsbHidService"/>.
/// </summary>
[CLSCompliant(false)]

public sealed class LibUsbDotNetUsbHidServiceBuilder<TServiceKey> : UsbHidServiceBuilder<TServiceKey> {
  internal LibUsbDotNetUsbHidServiceBuilder(
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
    var options = (serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider)))
      .GetRequiredService<IOptionsMonitor<LibUsbDotNetOptions>>()
      .Get(name: GetOptionsName());

    return new LibUsbDotNetUsbHidService(
      options: options,
      resiliencePipelineProvider: serviceProvider.GetResiliencePipelineProviderForLibUsbDotNetUsbHidService(
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
