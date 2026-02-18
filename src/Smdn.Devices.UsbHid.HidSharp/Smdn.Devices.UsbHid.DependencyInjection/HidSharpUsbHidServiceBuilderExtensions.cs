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
  public static UsbHidServiceBuilder<TServiceKey> AddResiliencePipelineForOpenEndPoint<TServiceKey>(
    this UsbHidServiceBuilder<TServiceKey> builder
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
  public static UsbHidServiceBuilder<TServiceKey> AddResiliencePipelineForOpenEndPoint<TServiceKey>(
    this UsbHidServiceBuilder<TServiceKey> builder,
    RetryStrategyOptions retryOptions
  )
    => AddResiliencePipelineForOpenEndPoint(
      builder: builder ?? throw new ArgumentNullException(nameof(builder)),
      configure: (pipelineBuilder, _) => {
        pipelineBuilder.AddRetry(retryOptions ?? throw new ArgumentNullException(nameof(retryOptions)));
      }
    );

  [CLSCompliant(false)]
  public static UsbHidServiceBuilder<TServiceKey> AddResiliencePipelineForOpenEndPoint<TServiceKey>(
    this UsbHidServiceBuilder<TServiceKey> builder,
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
