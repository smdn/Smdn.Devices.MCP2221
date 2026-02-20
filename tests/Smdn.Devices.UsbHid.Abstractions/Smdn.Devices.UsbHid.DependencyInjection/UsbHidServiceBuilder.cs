// SPDX-FileCopyrightText: 2026 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;

using Microsoft.Extensions.DependencyInjection;

using NUnit.Framework;

namespace Smdn.Devices.UsbHid.DependencyInjection;

[TestFixture]
public class UsbHidServiceBuilderTests {
  private class ConcreteUsbHidServiceBuilder<TServiceKey>(
    IServiceCollection services,
    TServiceKey serviceKey,
    Func<TServiceKey, string?>? selectOptionsNameForServiceKey
  ) : UsbHidServiceBuilder<TServiceKey>(
    serviceKey: serviceKey,
    services: services,
    selectOptionsNameForServiceKey: selectOptionsNameForServiceKey
  ) {
    public override IUsbHidService Build(IServiceProvider serviceProvider)
      => throw new NotImplementedException();
  }

  [Test]
  public void Ctor_ArgumentNullException_Services()
    => Assert.That(
      () => new ConcreteUsbHidServiceBuilder<string>(
        services: null!,
        serviceKey: "serviceKey",
        selectOptionsNameForServiceKey: serviceKey => serviceKey
      ),
      Throws
        .ArgumentNullException
        .With
        .Property(nameof(ArgumentNullException.ParamName))
        .EqualTo("services")
    );

  [Test]
  public void Ctor_ArgumentNull_ServiceKey()
    => Assert.That(
      () => new ConcreteUsbHidServiceBuilder<object?>(
        services: new ServiceCollection(),
        serviceKey: (object?)null,
        selectOptionsNameForServiceKey: null
      ),
      Throws.Nothing
    );

  [Test]
  public void Ctor_ArgumentNull_SelectOptionsNameForServiceKey()
    => Assert.That(
      () => new ConcreteUsbHidServiceBuilder<string>(
        services: new ServiceCollection(),
        serviceKey: "serviceKey",
        selectOptionsNameForServiceKey: null
      ),
      Throws.Nothing
    );

  [Test]
  public void Services()
  {
    var services = new ServiceCollection();
    var builder = new ConcreteUsbHidServiceBuilder<string>(
      services: services,
      serviceKey: "serviceKey",
      selectOptionsNameForServiceKey: null
    );

    Assert.That(builder.Services, Is.SameAs(services));
  }

  [Test]
  public void ServiceKey()
  {
    const string serviceKey = "serviceKey";

    var builder = new ConcreteUsbHidServiceBuilder<string>(
      services: new ServiceCollection(),
      serviceKey: serviceKey,
      selectOptionsNameForServiceKey: null
    );

    Assert.That(builder.ServiceKey, Is.SameAs(serviceKey));
  }

  [Test]
  public void GetOptionsName()
  {
    Assert.That(
      new ConcreteUsbHidServiceBuilder<int>(
        services: new ServiceCollection(),
        serviceKey: 0,
        selectOptionsNameForServiceKey: serviceKey => $"Options#{serviceKey}"
      ).GetOptionsName(),
      Is.EqualTo("Options#0")
    );
  }

  [Test]
  public void GetOptionsName_SelectOptionsNameForServiceKeyNotProvided()
  {
    Assert.That(
      () => new ConcreteUsbHidServiceBuilder<object?>(
        services: new ServiceCollection(),
        serviceKey: (object?)null,
        selectOptionsNameForServiceKey: null
      ).GetOptionsName(),
      Throws.InvalidOperationException
    );
  }
}
