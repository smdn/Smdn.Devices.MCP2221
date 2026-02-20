// SPDX-FileCopyrightText: 2026 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using NUnit.Framework;

namespace Smdn.Devices.UsbHid.DependencyInjection;

[TestFixture]
public class LibUsbDotNetServiceCollectionExtensionsTests {
  [Test]
  public void AddLibUsbDotNetUsbHid_ServicesNull()
  {
    Assert.That(
      () => (null as IServiceCollection)!.AddLibUsbDotNetUsbHid(),
      Throws.ArgumentNullException.With.Property(nameof(ArgumentNullException.ParamName)).EqualTo("services")
    );
    Assert.That(
      () => (null as IServiceCollection)!.AddLibUsbDotNetUsbHid((builder, options) => { }),
      Throws.ArgumentNullException.With.Property(nameof(ArgumentNullException.ParamName)).EqualTo("services")
    );
    Assert.That(
      () => (null as IServiceCollection)!.AddLibUsbDotNetUsbHid("key"),
      Throws.ArgumentNullException.With.Property(nameof(ArgumentNullException.ParamName)).EqualTo("services")
    );
    Assert.That(
      () => (null as IServiceCollection)!.AddLibUsbDotNetUsbHid("key", (builder, options) => { }),
      Throws.ArgumentNullException.With.Property(nameof(ArgumentNullException.ParamName)).EqualTo("services")
    );
    Assert.That(
      () => (null as IServiceCollection)!.AddLibUsbDotNetUsbHid<string>("key", static key => key),
      Throws.ArgumentNullException.With.Property(nameof(ArgumentNullException.ParamName)).EqualTo("services")
    );
    Assert.That(
      () => (null as IServiceCollection)!.AddLibUsbDotNetUsbHid<string>("key", static key => key, (builder, options) => { }),
      Throws.ArgumentNullException.With.Property(nameof(ArgumentNullException.ParamName)).EqualTo("services")
    );
  }

  [Test]
  public void AddLibUsbDotNetUsbHid_ConfigureNull()
  {
    var services = new ServiceCollection();

    Assert.That(
      () => services.AddLibUsbDotNetUsbHid(configure: null!),
      Throws.ArgumentNullException.With.Property(nameof(ArgumentNullException.ParamName)).EqualTo("configure")
    );
    Assert.That(
      () => services.AddLibUsbDotNetUsbHid("key", configure: null!),
      Throws.ArgumentNullException.With.Property(nameof(ArgumentNullException.ParamName)).EqualTo("configure")
    );
    Assert.That(
      () => services.AddLibUsbDotNetUsbHid<string>("key", static key => key, configure: null!),
      Throws.ArgumentNullException.With.Property(nameof(ArgumentNullException.ParamName)).EqualTo("configure")
    );
  }

  [Test]
  public void AddLibUsbDotNetUsbHid_SelectOptionsNameForServiceKeyNull()
  {
    var services = new ServiceCollection();

    Assert.That(
      () => services.AddLibUsbDotNetUsbHid("key", selectOptionsNameForServiceKey: null!),
      Throws.ArgumentNullException.With.Property(nameof(ArgumentNullException.ParamName)).EqualTo("selectOptionsNameForServiceKey")
    );
    Assert.That(
      () => services.AddLibUsbDotNetUsbHid("key", selectOptionsNameForServiceKey: null!, (builder, options) => { }),
      Throws.ArgumentNullException.With.Property(nameof(ArgumentNullException.ParamName)).EqualTo("selectOptionsNameForServiceKey")
    );
  }

  [Test]
  public void AddLibUsbDotNetUsbHid()
  {
    var services = new ServiceCollection();

    services.AddLibUsbDotNetUsbHid(
      static (_, options) => {
        options.DebugLevel = LogLevel.None;
      }
    );

    var provider = services.BuildServiceProvider();
    using var usbHidService = provider.GetService<IUsbHidService>();

    Assert.That(usbHidService, Is.Not.Null);
  }

  [Test]
  public void AddLibUsbDotNetUsbHid_WithServiceKey_OfStringKey()
  {
    const string ServiceKey = nameof(ServiceKey);
    var services = new ServiceCollection();

    services.AddLibUsbDotNetUsbHid(
      ServiceKey,
      static (_, options) => {
        options.DebugLevel = LogLevel.None;
      }
    );

    var provider = services.BuildServiceProvider();
    using var usbHidService = provider.GetKeyedService<IUsbHidService>(ServiceKey);

    Assert.That(usbHidService, Is.Not.Null);
  }

  [Test]
  public void AddLibUsbDotNetUsbHid_WithServiceKey_OfNonStringKey()
  {
    const int ServiceKey = 1;
    var services = new ServiceCollection();

    services.AddLibUsbDotNetUsbHid(
      ServiceKey,
      static key => key.ToString(provider: null),
      static (_, options) => {
        options.DebugLevel = LogLevel.None;
      }
    );

    var provider = services.BuildServiceProvider();
    using var usbHidService = provider.GetKeyedService<IUsbHidService>(ServiceKey);

    Assert.That(usbHidService, Is.Not.Null);
  }

  [Test]
  public void AddLibUsbDotNetUsbHid_Configure()
  {
    var services = new ServiceCollection();
    var configureCalled = false;

    services.AddLogging();
    services.AddLibUsbDotNetUsbHid(
      (builder, options) => {
        configureCalled = true;

        Assert.That(builder, Is.Not.Null);
        Assert.That(options, Is.Not.Null);

        options.DebugLevel = LogLevel.Warning;
      }
    );

    var provider = services.BuildServiceProvider();
    var resolvedOptions = provider.GetRequiredService<IOptions<LibUsbDotNetOptions>>().Value;

    Assert.That(configureCalled, Is.True);
    Assert.That(resolvedOptions.DebugLevel, Is.EqualTo(LogLevel.Warning));
  }
}