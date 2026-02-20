// SPDX-FileCopyrightText: 2026 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;

using Microsoft.Extensions.DependencyInjection;

using NUnit.Framework;

using Polly.Retry;
using Polly.Registry;

namespace Smdn.Devices.UsbHid.DependencyInjection;

[TestFixture]
public class LibUsbDotNetUsbHidServiceProviderExtensionsTests {
  [Test]
  public void GetResiliencePipelineProviderForLibUsbDotNetUsbHidService_ServiceProviderNull()
    => Assert.That(
      () => (null as IServiceProvider)!.GetResiliencePipelineProviderForLibUsbDotNetUsbHidService("service-key"),
      Throws.ArgumentNullException.With.Property(nameof(ArgumentNullException.ParamName)).EqualTo("serviceProvider")
    );

  [Test]
  public void GetResiliencePipelineProviderForLibUsbDotNetUsbHidService()
  {
    const string ServiceKey = nameof(ServiceKey);

    var services = new ServiceCollection();

    services.AddLibUsbDotNetUsbHid(
      ServiceKey,
      (builder, options) => {
        builder.AddResiliencePipelineForOpenEndPoint(
          new RetryStrategyOptions()
        );
      }
    );
    services.AddLibUsbDotNetUsbHid(
      (builder, options) => {
        builder.AddResiliencePipelineForOpenEndPoint(
          new RetryStrategyOptions()
        );
      }
    );

    var provider = services.BuildServiceProvider();

    Assert.That(
      provider.GetResiliencePipelineProviderForLibUsbDotNetUsbHidService(ServiceKey),
      Is.Not.Null
    );

    Assert.That(
      provider.GetResiliencePipelineProviderForLibUsbDotNetUsbHidService(null),
      Is.Not.Null
    );
  }
}