// SPDX-FileCopyrightText: 2026 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;

using Microsoft.Extensions.DependencyInjection;

using NUnit.Framework;

using Polly.Retry;
using Polly.Registry;

namespace Smdn.Devices.UsbHid.DependencyInjection;

[TestFixture]
public class HidSharpUsbHidServiceProviderExtensionsTests {
  [Test]
  public void GetResiliencePipelineProviderForHidSharpUsbHidService_ServiceProviderNull()
    => Assert.That(
      () => (null as IServiceProvider)!.GetResiliencePipelineProviderForHidSharpUsbHidService("service-key"),
      Throws.ArgumentNullException.With.Property(nameof(ArgumentNullException.ParamName)).EqualTo("serviceProvider")
    );

  [Test]
  public void GetResiliencePipelineProviderForHidSharpUsbHidService()
  {
    const string ServiceKey = nameof(ServiceKey);

    var services = new ServiceCollection();

    services.AddHidSharpUsbHid(
      ServiceKey,
      builder => {
        builder.AddResiliencePipelineForOpenEndPoint(
          new RetryStrategyOptions()
        );
      }
    );
    services.AddHidSharpUsbHid(
      builder => {
        builder.AddResiliencePipelineForOpenEndPoint(
          new RetryStrategyOptions()
        );
      }
    );

    var provider = services.BuildServiceProvider();

    Assert.That(
      provider.GetResiliencePipelineProviderForHidSharpUsbHidService(ServiceKey),
      Is.Not.Null
    );

    Assert.That(
      provider.GetResiliencePipelineProviderForHidSharpUsbHidService(null),
      Is.Not.Null
    );
  }
}