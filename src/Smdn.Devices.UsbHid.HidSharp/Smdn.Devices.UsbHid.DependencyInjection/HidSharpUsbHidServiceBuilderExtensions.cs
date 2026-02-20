// SPDX-FileCopyrightText: 2026 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;

using Polly;
using Polly.DependencyInjection;
using Polly.Retry;

namespace Smdn.Devices.UsbHid.DependencyInjection;

public static class HidSharpUsbHidServiceBuilderExtensions {
  private const int OpenEndPointMaxRetryAttempts = 5;
  private const int OpenEndPointRetryIntervalInMilliseconds = 200;

  [CLSCompliant(false)]
  public static HidSharpUsbHidServiceBuilder<TServiceKey> AddResiliencePipelineForOpenEndPoint<TServiceKey>(
    this HidSharpUsbHidServiceBuilder<TServiceKey> builder
  )
    => AddResiliencePipelineForOpenEndPoint(
      builder: builder ?? throw new ArgumentNullException(nameof(builder)),
      retryOptions: new() {
        MaxRetryAttempts = OpenEndPointMaxRetryAttempts,
        Delay = TimeSpan.FromMilliseconds(OpenEndPointRetryIntervalInMilliseconds),
        BackoffType = DelayBackoffType.Constant,
      }
    );

  [CLSCompliant(false)]
  public static HidSharpUsbHidServiceBuilder<TServiceKey> AddResiliencePipelineForOpenEndPoint<TServiceKey>(
    this HidSharpUsbHidServiceBuilder<TServiceKey> builder,
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
  public static HidSharpUsbHidServiceBuilder<TServiceKey> AddResiliencePipelineForOpenEndPoint<TServiceKey>(
    this HidSharpUsbHidServiceBuilder<TServiceKey> builder,
    Action<ResiliencePipelineBuilder, AddResiliencePipelineContext<HidSharpResiliencePipelineKeyPair<TServiceKey>>> configure
  )
  {
    _ = (builder ?? throw new ArgumentNullException(nameof(builder))).Services.AddResiliencePipelineForOpenEndPoint(
      serviceKey: builder.ServiceKey,
      configure: configure ?? throw new ArgumentNullException(nameof(configure))
    );

    return builder;
  }
}
