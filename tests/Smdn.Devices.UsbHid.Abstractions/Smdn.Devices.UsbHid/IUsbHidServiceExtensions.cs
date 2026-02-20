// SPDX-FileCopyrightText: 2026 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

using NUnit.Framework;

namespace Smdn.Devices.UsbHid.DependencyInjection;

[TestFixture]
public class IUsbHidServiceExtensionsTests {
  private class PseudoUsbHidService(IReadOnlyList<IUsbHidDevice> allDevices) : IUsbHidService {
    public IReadOnlyList<IUsbHidDevice> GetDevices(CancellationToken cancellationToken)
      => allDevices;

    public void Dispose()
    {
      // nothing to do
    }

    public ValueTask DisposeAsync()
    {
      // nothing to do
      return default;
    }
  }

  private class PseudoUsbHidDevice(int vendorId, int productId) : IUsbHidDevice {
    public bool IsDisposed { get; private set; }

    public int VendorId { get; } = vendorId;
    public int ProductId { get; } = productId;

    public string? DeviceType { get; }

    public PseudoUsbHidDevice(int vendorId, int productId, string deviceType)
      : this(vendorId, productId)
      => DeviceType = deviceType;

    public void Dispose()
    {
      IsDisposed = true;
    }

    public ValueTask DisposeAsync()
    {
      IsDisposed = true;

      return default;
    }

    public bool TryGetProductName([NotNullWhen(true)] out string? productName)
      => throw new NotImplementedException();

    public bool TryGetManufacturer([NotNullWhen(true)] out string? manufacturer)
      => throw new NotImplementedException();

    public bool TryGetSerialNumber([NotNullWhen(true)] out string? serialNumber)
      => throw new NotImplementedException();

    public bool TryGetDeviceIdentifier([NotNullWhen(true)] out string? deviceIdentifier)
      => throw new NotImplementedException();

    public IUsbHidEndPoint OpenEndPoint(
      bool openOutEndPoint,
      bool openInEndPoint,
      bool shouldDisposeDevice,
      CancellationToken cancellationToken
    )
      => throw new NotImplementedException();

    public ValueTask<IUsbHidEndPoint> OpenEndPointAsync(
      bool openOutEndPoint,
      bool openInEndPoint,
      bool shouldDisposeDevice,
      CancellationToken cancellationToken
    )
      => throw new NotImplementedException();
  }

  private record class PseudoDeviceImplementation(int VendorId, int ProductId, string DeviceType);

  private class PseudoUsbHidDeviceWrapper(PseudoDeviceImplementation implementation) :
    PseudoUsbHidDevice(implementation.VendorId, implementation.ProductId),
    IUsbHidDevice<PseudoDeviceImplementation>
  {
    public PseudoDeviceImplementation DeviceImplementation { get; } = implementation;
  }

  [Test]
  public void GetDevices_ArgumentNullException()
  {
    IUsbHidService? usbHidService = null;

    Assert.That(
      () => usbHidService!.GetDevices(vendorId: null, productId: null, cancellationToken: default),
      Throws
        .ArgumentNullException
        .With
        .Property(nameof(ArgumentNullException.ParamName))
        .EqualTo("usbHidService")
    );
  }

  [Test]
  public void GetDevices_WithVendorId()
  {
    var allDevices = new PseudoUsbHidDevice[] {
      new(1, 0),
      new(1, 1),
      new(2, 0),
    };
    var usbHidService = new PseudoUsbHidService(allDevices);

    var devices = usbHidService.GetDevices(vendorId: 1, cancellationToken: default);

    Assert.That(devices, Has.Count.EqualTo(2));
    Assert.That(devices[0].VendorId, Is.EqualTo(1));
    Assert.That(devices[0].ProductId, Is.Zero);
    Assert.That(devices[1].VendorId, Is.EqualTo(1));
    Assert.That(devices[1].ProductId, Is.EqualTo(1));

    Assert.That(allDevices[0].IsDisposed, Is.False, "device 0 not disposed");
    Assert.That(allDevices[1].IsDisposed, Is.False, "device 1 not disposed");
    Assert.That(allDevices[2].IsDisposed, Is.True, "device 2 disposed");
  }

  [Test]
  public void GetDevices_WithProductId()
  {
    var allDevices = new PseudoUsbHidDevice[] {
      new(1, 0),
      new(1, 1),
      new(2, 1),
    };
    var usbHidService = new PseudoUsbHidService(allDevices);

    var devices = usbHidService.GetDevices(productId: 1);

    Assert.That(devices, Has.Count.EqualTo(2));
    Assert.That(devices[0].VendorId, Is.EqualTo(1));
    Assert.That(devices[0].ProductId, Is.EqualTo(1));
    Assert.That(devices[1].VendorId, Is.EqualTo(2));
    Assert.That(devices[1].ProductId, Is.EqualTo(1));

    Assert.That(allDevices[0].IsDisposed, Is.True, "device 0 disposed");
    Assert.That(allDevices[1].IsDisposed, Is.False, "device 1 not disposed");
    Assert.That(allDevices[2].IsDisposed, Is.False, "device 2 not disposed");
  }

  [Test]
  public void GetDevices_WithVendorIdAndProductId()
  {
    var allDevices = new PseudoUsbHidDevice[] {
      new(1, 0),
      new(1, 1),
      new(2, 0),
    };
    var usbHidService = new PseudoUsbHidService(allDevices);

    var devices = usbHidService.GetDevices(vendorId: 1, productId: 1);

    Assert.That(devices, Has.Count.EqualTo(1));
    Assert.That(devices[0].VendorId, Is.EqualTo(1));
    Assert.That(devices[0].ProductId, Is.EqualTo(1));

    Assert.That(allDevices[0].IsDisposed, Is.True, "device 0 disposed");
    Assert.That(allDevices[1].IsDisposed, Is.False, "device 1 not disposed");
    Assert.That(allDevices[2].IsDisposed, Is.True, "device 2 disposed");
  }

  [Test]
  public void GetDevices_NothingMatched()
  {
    var allDevices = new PseudoUsbHidDevice[] {
      new(1, 0),
      new(1, 1),
      new(2, 0),
    };
    var usbHidService = new PseudoUsbHidService(allDevices);

    var devices = usbHidService.GetDevices(vendorId: 9, productId: 9);

    Assert.That(devices, Is.Empty);

    Assert.That(allDevices[0].IsDisposed, Is.True, "device 0 disposed");
    Assert.That(allDevices[1].IsDisposed, Is.True, "device 1 disposed");
    Assert.That(allDevices[2].IsDisposed, Is.True, "device 2 disposed");
  }

  [Test]
  public void GetDevices_CancellationRequested()
  {
    var devices = new PseudoUsbHidDevice[] {
      new(1, 0),
      new(1, 1),
      new(1, 2),
    };
    var usbHidService = new PseudoUsbHidService(devices);

    using var cts = new CancellationTokenSource();

    cts.Cancel();

    Assert.That(
      () => usbHidService.GetDevices(
        vendorId: null,
        productId: null,
        cancellationToken: cts.Token
      ),
      Throws
        .InstanceOf<OperationCanceledException>()
        .With
        .Property(nameof(OperationCanceledException.CancellationToken))
        .EqualTo(cts.Token)
    );

    for (var i = 0; i < devices.Length; i++) {
      Assert.That(devices[i].IsDisposed, Is.True, $"devices[{i}] disposed");
    }
  }

  [Test]
  public void FindDevice_OfIUsbHidDevice_ArgumentNullException()
  {
    IUsbHidService? usbHidService = null;

    Assert.That(
      () => usbHidService!.FindDevice(
        vendorId: null,
        productId: null,
        predicate: null,
        cancellationToken: default
      ),
      Throws
        .ArgumentNullException
        .With
        .Property(nameof(ArgumentNullException.ParamName))
        .EqualTo("usbHidService")
    );
  }

  [Test]
  public void FindDevice_OfIUsbHidDevice()
  {
    var allDevices = new PseudoUsbHidDevice[] {
      new(1, 1, "mouse"),
      new(1, 2, "keyboard"),
      new(1, 3, "touchscreen"),
    };
    var usbHidService = new PseudoUsbHidService(allDevices);

    var device = usbHidService.FindDevice(
      vendorId: null,
      productId: null,
      predicate: d => d is PseudoUsbHidDevice { DeviceType: "keyboard" }
    );

    Assert.That(device, Is.Not.Null);
    Assert.That(device.VendorId, Is.EqualTo(1));
    Assert.That(device.ProductId, Is.EqualTo(2));
    Assert.That(device, Is.TypeOf<PseudoUsbHidDevice>());
    Assert.That((device as PseudoUsbHidDevice)!.DeviceType, Is.EqualTo("keyboard"));

    Assert.That(allDevices[0].IsDisposed, Is.True, "device 0 disposed");
    Assert.That(allDevices[1].IsDisposed, Is.False, "device 1 not disposed");
    Assert.That(allDevices[2].IsDisposed, Is.True, "device 2 disposed");
  }

  [Test]
  public void FindDevice_OfIUsbHidDevice_WithVendorIdAndProductId()
  {
    var allDevices = new PseudoUsbHidDevice[] {
      new(1, 1, "keyboard"), // disposed (vid mismatch)
      new(2, 1, "mouse"), // disposed (predicate mismatch)
      new(2, 1, "keyboard"), // returned
      new(2, 1, "touchscreen"), // disposed (not evaluated)
    };
    var usbHidService = new PseudoUsbHidService(allDevices);

    var device = usbHidService.FindDevice(
      vendorId: 2,
      productId: 1,
      predicate: d => d is PseudoUsbHidDevice { DeviceType: "keyboard" }
    );

    Assert.That(device, Is.Not.Null);
    Assert.That(device.VendorId, Is.EqualTo(2));
    Assert.That(device.ProductId, Is.EqualTo(1));
    Assert.That(device, Is.TypeOf<PseudoUsbHidDevice>());
    Assert.That((device as PseudoUsbHidDevice)!.DeviceType, Is.EqualTo("keyboard"));

    Assert.That(allDevices[0].IsDisposed, Is.True, "device 0 disposed");
    Assert.That(allDevices[1].IsDisposed, Is.True, "device 1 disposed");
    Assert.That(allDevices[2].IsDisposed, Is.False, "device 2 not disposed");
    Assert.That(allDevices[3].IsDisposed, Is.True, "device 3 disposed");
  }

  [Test]
  public void FindDevice_OfIUsbHidDevice_NothingMatched()
  {
    var allDevices = new PseudoUsbHidDevice[] {
      new(1, 1, "mouse"),
      new(1, 2, "keyboard"),
      new(1, 3, "touchscreen"),
    };
    var usbHidService = new PseudoUsbHidService(allDevices);

    var device = usbHidService.FindDevice(
      vendorId: null,
      productId: null,
      predicate: d => d is PseudoUsbHidDevice { DeviceType: "gamepad" }
    );

    Assert.That(device, Is.Null);

    Assert.That(allDevices[0].IsDisposed, Is.True, "device 0 disposed");
    Assert.That(allDevices[1].IsDisposed, Is.True, "device 1 disposed");
    Assert.That(allDevices[2].IsDisposed, Is.True, "device 2 disposed");
  }

  [Test]
  public void FindDevice_OfIUsbHidDevice_CancellationRequested()
  {
    var devices = new PseudoUsbHidDevice[] {
      new(1, 0),
      new(1, 1),
      new(1, 2),
    };
    var usbHidService = new PseudoUsbHidService(devices);

    using var cts = new CancellationTokenSource();

    cts.Cancel();

    Assert.That(
      () => usbHidService.FindDevice(
        vendorId: null,
        productId: null,
        predicate: _ => true,
        cancellationToken: cts.Token
      ),
      Throws
        .InstanceOf<OperationCanceledException>()
        .With
        .Property(nameof(OperationCanceledException.CancellationToken))
        .EqualTo(cts.Token)
    );

    for (var i = 0; i < devices.Length; i++) {
      Assert.That(devices[i].IsDisposed, Is.True, $"devices[{i}] disposed");
    }
  }

  [Test]
  public void FindDevice_OfTDevice_ArgumentNullException_UsbHidService()
  {
    IUsbHidService? usbHidService = null;

    Assert.That(
      () => usbHidService!.FindDevice<PseudoDeviceImplementation>(
        vendorId: null,
        productId: null,
        predicate: _ => true,
        cancellationToken: default
      ),
      Throws
        .ArgumentNullException
        .With
        .Property(nameof(ArgumentNullException.ParamName))
        .EqualTo("usbHidService")
    );
  }

  [Test]
  public void FindDevice_OfTDevice_ArgumentNullException_Predicate()
  {
    var usbHidService = new PseudoUsbHidService([]);

    Assert.That(
      () => usbHidService.FindDevice<PseudoDeviceImplementation>(
        vendorId: null,
        productId: null,
        predicate: null!,
        cancellationToken: default
      ),
      Throws
        .ArgumentNullException
        .With
        .Property(nameof(ArgumentNullException.ParamName))
        .EqualTo("predicate")
    );
  }

  [Test]
  public void FindDevice_OfTDevice()
  {
    var allDevices = new PseudoUsbHidDeviceWrapper[] {
      new(new(1, 1, "mouse")),
      new(new(1, 2, "keyboard")),
      new(new(1, 3, "touchscreen")),
    };
    var usbHidService = new PseudoUsbHidService(allDevices);

    var device = usbHidService.FindDevice<PseudoDeviceImplementation>(
      vendorId: null,
      productId: null,
      predicate: d => d.DeviceType == "keyboard"
    );

    Assert.That(device, Is.Not.Null);
    Assert.That(device, Is.TypeOf<PseudoUsbHidDeviceWrapper>());
    Assert.That((device as PseudoUsbHidDeviceWrapper)!.DeviceImplementation, Is.Not.Null);
    Assert.That((device as PseudoUsbHidDeviceWrapper)!.DeviceImplementation.VendorId, Is.EqualTo(1));
    Assert.That((device as PseudoUsbHidDeviceWrapper)!.DeviceImplementation.ProductId, Is.EqualTo(2));
    Assert.That((device as PseudoUsbHidDeviceWrapper)!.DeviceImplementation.DeviceType, Is.EqualTo("keyboard"));

    Assert.That(allDevices[0].IsDisposed, Is.True, "device 0 disposed");
    Assert.That(allDevices[1].IsDisposed, Is.False, "device 1 not disposed");
    Assert.That(allDevices[2].IsDisposed, Is.True, "device 2 disposed");
  }

  [Test]
  public void FindDevice_OfTDevice_WithVendorIdAndProductId()
  {
    var allDevices = new PseudoUsbHidDeviceWrapper[] {
      new(new(1, 1, "keyboard")), // disposed (vid mismatch)
      new(new(2, 1, "mouse")), // disposed (predicate mismatch)
      new(new(2, 1, "keyboard")), // returned
      new(new(2, 1, "touchscreen")), // disposed (not evaluated)
    };
    var usbHidService = new PseudoUsbHidService(allDevices);

    var device = usbHidService.FindDevice<PseudoDeviceImplementation>(
      vendorId: 2,
      productId: 1,
      predicate: d => d.DeviceType == "keyboard"
    );

    Assert.That(device, Is.Not.Null);
    Assert.That(device, Is.TypeOf<PseudoUsbHidDeviceWrapper>());
    Assert.That((device as PseudoUsbHidDeviceWrapper)!.DeviceImplementation, Is.Not.Null);
    Assert.That((device as PseudoUsbHidDeviceWrapper)!.DeviceImplementation.VendorId, Is.EqualTo(2));
    Assert.That((device as PseudoUsbHidDeviceWrapper)!.DeviceImplementation.ProductId, Is.EqualTo(1));
    Assert.That((device as PseudoUsbHidDeviceWrapper)!.DeviceImplementation.DeviceType, Is.EqualTo("keyboard"));

    Assert.That(allDevices[0].IsDisposed, Is.True, "device 0 disposed");
    Assert.That(allDevices[1].IsDisposed, Is.True, "device 1 disposed");
    Assert.That(allDevices[2].IsDisposed, Is.False, "device 2 not disposed");
    Assert.That(allDevices[3].IsDisposed, Is.True, "device 3 disposed");
  }

  [Test]
  public void FindDevice_OfTDevice_NothingMatched()
  {
    var allDevices = new PseudoUsbHidDeviceWrapper[] {
      new(new(1, 1, "mouse")),
      new(new(1, 2, "keyboard")),
      new(new(1, 3, "touchscreen")),
    };
    var usbHidService = new PseudoUsbHidService(allDevices);

    var device = usbHidService.FindDevice<PseudoDeviceImplementation>(
      vendorId: null,
      productId: null,
      predicate: d => d.DeviceType == "gamepad"
    );

    Assert.That(device, Is.Null);

    Assert.That(allDevices[0].IsDisposed, Is.True, "device 0 disposed");
    Assert.That(allDevices[1].IsDisposed, Is.True, "device 1 disposed");
    Assert.That(allDevices[2].IsDisposed, Is.True, "device 2 disposed");
  }

  [Test]
  public void FindDevice_OfTDevice_CancellationRequested()
  {
    var devices = new PseudoUsbHidDeviceWrapper[] {
      new(new(1, 1, "mouse")),
      new(new(1, 2, "keyboard")),
      new(new(1, 3, "touchscreen")),
    };
    var usbHidService = new PseudoUsbHidService(devices);

    using var cts = new CancellationTokenSource();

    cts.Cancel();

    Assert.That(
      () => usbHidService.FindDevice<PseudoDeviceImplementation>(
        vendorId: null,
        productId: null,
        predicate: _ => true,
        cancellationToken: cts.Token
      ),
      Throws
        .InstanceOf<OperationCanceledException>()
        .With
        .Property(nameof(OperationCanceledException.CancellationToken))
        .EqualTo(cts.Token)
    );

    for (var i = 0; i < devices.Length; i++) {
      Assert.That(devices[i].IsDisposed, Is.True, $"devices[{i}] disposed");
    }
  }
}
