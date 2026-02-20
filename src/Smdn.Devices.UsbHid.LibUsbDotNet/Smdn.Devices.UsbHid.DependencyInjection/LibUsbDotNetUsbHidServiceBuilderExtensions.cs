// SPDX-FileCopyrightText: 2026 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;

using Polly;
using Polly.DependencyInjection;
using Polly.Retry;

namespace Smdn.Devices.UsbHid.DependencyInjection;

public static class LibUsbDotNetUsbHidServiceBuilderExtensions {
  [CLSCompliant(false)]
  public static LibUsbDotNetUsbHidServiceBuilder<TServiceKey> AddResiliencePipelineForOpenEndPoint<TServiceKey>(
    this LibUsbDotNetUsbHidServiceBuilder<TServiceKey> builder,
    RetryStrategyOptions retryOptions
  )
  {
    if (retryOptions is null)
      throw new ArgumentNullException(nameof(retryOptions));

    return AddResiliencePipelineForOpenEndPoint(
      builder: builder ?? throw new ArgumentNullException(nameof(builder)),
      configure: (pipelineBuilder, _) => {
        pipelineBuilder.AddRetry(retryOptions);
      }
    );
  }

  [CLSCompliant(false)]
  public static LibUsbDotNetUsbHidServiceBuilder<TServiceKey> AddResiliencePipelineForOpenEndPoint<TServiceKey>(
    this LibUsbDotNetUsbHidServiceBuilder<TServiceKey> builder,
    Action<ResiliencePipelineBuilder, AddResiliencePipelineContext<LibUsbDotNetResiliencePipelineKeyPair<TServiceKey>>> configure
  )
  {
    _ = (builder ?? throw new ArgumentNullException(nameof(builder))).Services.AddResiliencePipelineForOpenEndPoint(
      serviceKey: builder.ServiceKey,
      configure: configure ?? throw new ArgumentNullException(nameof(configure))
    );

    return builder;
  }
}
