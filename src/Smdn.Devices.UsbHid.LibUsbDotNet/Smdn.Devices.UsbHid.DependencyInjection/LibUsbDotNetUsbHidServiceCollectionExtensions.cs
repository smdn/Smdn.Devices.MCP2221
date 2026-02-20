// SPDX-FileCopyrightText: 2026 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;

using Microsoft.Extensions.DependencyInjection;

using Polly;
using Polly.DependencyInjection;

namespace Smdn.Devices.UsbHid.DependencyInjection;

internal static class LibUsbDotNetUsbHidServiceCollectionExtensions {
  private static
  LibUsbDotNetResiliencePipelineKeyPair<TServiceKey>
  CreateResiliencePipelineKeyPair<TServiceKey>(TServiceKey serviceKey, string pipelineKey)
    => new(serviceKey, pipelineKey);

  // /* non-public */ [CLSCompliant(false)] // RetryStrategyOptions is not CLS compliant
  public static IServiceCollection AddResiliencePipelineForOpenEndPoint(
    this IServiceCollection services,
    Action<ResiliencePipelineBuilder, AddResiliencePipelineContext<string>> configure
  )
  {
    if (services is null)
      throw new ArgumentNullException(nameof(services));

    services
      .AddResiliencePipeline(
        key: LibUsbDotNetUsbHidDevice.ResiliencePipelineKeyForOpenEndPoint,
        configure: configure
      );

    return services;
  }

  // /* non-public */ [CLSCompliant(false)] // RetryStrategyOptions is not CLS compliant
  public static IServiceCollection AddResiliencePipelineForOpenEndPoint<TServiceKey>(
    this IServiceCollection services,
    TServiceKey serviceKey,
    Action<ResiliencePipelineBuilder, AddResiliencePipelineContext<LibUsbDotNetResiliencePipelineKeyPair<TServiceKey>>> configure
  )
  {
    if (services is null)
      throw new ArgumentNullException(nameof(services));

    services
      .AddResiliencePipeline(
        serviceKey: serviceKey,
        pipelineKey: LibUsbDotNetUsbHidDevice.ResiliencePipelineKeyForOpenEndPoint,
        createResiliencePipelineKeyPair: CreateResiliencePipelineKeyPair,
        configure: configure
      );

    return services;
  }
}
