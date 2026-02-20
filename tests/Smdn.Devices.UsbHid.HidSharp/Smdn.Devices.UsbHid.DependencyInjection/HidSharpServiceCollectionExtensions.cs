// SPDX-FileCopyrightText: 2026 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;

using Microsoft.Extensions.DependencyInjection;

using NUnit.Framework;

namespace Smdn.Devices.UsbHid.DependencyInjection;

[TestFixture]
public class HidSharpServiceCollectionExtensionsTests {
  [Test]
  public void AddHidSharpUsbHid_ServicesNull()
  {
    Assert.That(
      () => (null as IServiceCollection)!.AddHidSharpUsbHid(),
      Throws.ArgumentNullException.With.Property(nameof(ArgumentNullException.ParamName)).EqualTo("services")
    );
    Assert.That(
      () => (null as IServiceCollection)!.AddHidSharpUsbHid(builder => { }),
      Throws.ArgumentNullException.With.Property(nameof(ArgumentNullException.ParamName)).EqualTo("services")
    );
    Assert.That(
      () => (null as IServiceCollection)!.AddHidSharpUsbHid("key"),
      Throws.ArgumentNullException.With.Property(nameof(ArgumentNullException.ParamName)).EqualTo("services")
    );
    Assert.That(
      () => (null as IServiceCollection)!.AddHidSharpUsbHid("key", builder => { }),
      Throws.ArgumentNullException.With.Property(nameof(ArgumentNullException.ParamName)).EqualTo("services")
    );
    Assert.That(
      () => (null as IServiceCollection)!.AddHidSharpUsbHid<string>("key", static key => key),
      Throws.ArgumentNullException.With.Property(nameof(ArgumentNullException.ParamName)).EqualTo("services")
    );
    Assert.That(
      () => (null as IServiceCollection)!.AddHidSharpUsbHid<string>("key", static key => key, builder => { }),
      Throws.ArgumentNullException.With.Property(nameof(ArgumentNullException.ParamName)).EqualTo("services")
    );
  }

  [Test]
  public void AddHidSharpUsbHid_ConfigureNull()
  {
    var services = new ServiceCollection();

    Assert.That(
      () => services.AddHidSharpUsbHid(configure: null!),
      Throws.ArgumentNullException.With.Property(nameof(ArgumentNullException.ParamName)).EqualTo("configure")
    );
    Assert.That(
      () => services.AddHidSharpUsbHid("key", configure: null!),
      Throws.ArgumentNullException.With.Property(nameof(ArgumentNullException.ParamName)).EqualTo("configure")
    );
    Assert.That(
      () => services.AddHidSharpUsbHid<string>("key", static key => key, configure: null!),
      Throws.ArgumentNullException.With.Property(nameof(ArgumentNullException.ParamName)).EqualTo("configure")
    );
  }

  [Test]
  public void AddHidSharpUsbHid_SelectOptionsNameForServiceKeyNull()
  {
    var services = new ServiceCollection();

    Assert.That(
      () => services.AddHidSharpUsbHid("key", selectOptionsNameForServiceKey: null!),
      Throws.ArgumentNullException.With.Property(nameof(ArgumentNullException.ParamName)).EqualTo("selectOptionsNameForServiceKey")
    );
    Assert.That(
      () => services.AddHidSharpUsbHid("key", selectOptionsNameForServiceKey: null!, builder => { }),
      Throws.ArgumentNullException.With.Property(nameof(ArgumentNullException.ParamName)).EqualTo("selectOptionsNameForServiceKey")
    );
  }

  [Test]
  public void AddHidSharpUsbHid()
  {
    var services = new ServiceCollection();

    services.AddHidSharpUsbHid();

    var provider = services.BuildServiceProvider();
    using var usbHidService = provider.GetService<IUsbHidService>();

    Assert.That(usbHidService, Is.Not.Null);
  }

  [Test]
  public void AddHidSharpUsbHid_WithServiceKey_OfStringKey()
  {
    const string ServiceKey = nameof(ServiceKey);
    var services = new ServiceCollection();

    services.AddHidSharpUsbHid(ServiceKey);

    var provider = services.BuildServiceProvider();
    using var usbHidService = provider.GetKeyedService<IUsbHidService>(ServiceKey);

    Assert.That(usbHidService, Is.Not.Null);
  }

  [Test]
  public void AddHidSharpUsbHid_WithServiceKey_OfNonStringKey()
  {
    const int ServiceKey = 1;
    var services = new ServiceCollection();

    services.AddHidSharpUsbHid(
      ServiceKey,
      static key => key.ToString(provider: null)
    );

    var provider = services.BuildServiceProvider();
    using var usbHidService = provider.GetKeyedService<IUsbHidService>(ServiceKey);

    Assert.That(usbHidService, Is.Not.Null);
  }

  [Test]
  public void AddHidSharpUsbHid_Configure()
  {
    var services = new ServiceCollection();
    var configureCalled = false;

    services.AddHidSharpUsbHid(
      builder => {
        configureCalled = true;

        Assert.That(builder, Is.Not.Null);
      }
    );

    var provider = services.BuildServiceProvider();

    Assert.That(configureCalled, Is.True);

    using var usbHidService = provider.GetService<IUsbHidService>();

    Assert.That(usbHidService, Is.Not.Null);
  }
}