// SPDX-FileCopyrightText: 2026 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

using NUnit.Framework;

namespace Smdn.Devices.UsbHid.DependencyInjection;

[TestFixture]
public class IUsbHidDeviceExtensionsTests {
  private class PseudoUsbHidDevice : IUsbHidDevice {
    public bool IsDisposed { get; private set; }

    public int VendorId { get; }
    public int ProductId { get; }

    private readonly string? deviceIdentifier;
    private readonly string? serialNumber;
    private readonly string? toStringValue;

    public PseudoUsbHidDevice(
      int vendorId,
      int productId,
      string? deviceIdentifier = null,
      string? serialNumber = null,
      string? toStringValue = null
    )
    {
      VendorId = vendorId;
      ProductId = productId;
      this.deviceIdentifier = deviceIdentifier;
      this.serialNumber = serialNumber;
      this.toStringValue = toStringValue;
    }

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
      => (serialNumber = this.serialNumber) is not null;

    public bool TryGetDeviceIdentifier([NotNullWhen(true)] out string? deviceIdentifier)
      => (deviceIdentifier = this.deviceIdentifier) is not null;

    public IUsbHidEndPoint OpenEndPoint(
      bool openOutEndPoint,
      bool openInEndPoint,
      bool shouldDisposeDevice,
      CancellationToken cancellationToken
    )
      => new PseudoUsbHidEndPoint(this, openOutEndPoint, openInEndPoint, shouldDisposeDevice, cancellationToken);

    public ValueTask<IUsbHidEndPoint> OpenEndPointAsync(
      bool openOutEndPoint,
      bool openInEndPoint,
      bool shouldDisposeDevice,
      CancellationToken cancellationToken
    )
      => new(new PseudoUsbHidEndPoint(this, openOutEndPoint, openInEndPoint, shouldDisposeDevice, cancellationToken));

    public override string? ToString() => toStringValue;
  }

  private class PseudoUsbHidEndPoint(
    IUsbHidDevice device,
    bool openOutEndPoint,
    bool openInEndPoint,
    bool shouldDisposeDevice,
    CancellationToken cancellationTokenPassedToOpenEndPoint
  ) : IUsbHidEndPoint {
    public IUsbHidDevice Device { get; } = device;
    public bool CanWrite { get; } = openOutEndPoint;
    public bool CanRead { get; } = openInEndPoint;
    public bool ShouldDisposeDevice { get; } = shouldDisposeDevice;
    public CancellationToken CancellationTokenPassedToOpenEndPoint { get; } = cancellationTokenPassedToOpenEndPoint;

    public void Dispose()
    {
      // nothing to do
    }

    public ValueTask DisposeAsync()
    {
      // nothing to do
      return default;
    }

    public int Read(Span<byte> buffer, CancellationToken cancellationToken)
      => throw new NotImplementedException();

    public ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken)
      => throw new NotImplementedException();

    public void Write(ReadOnlySpan<byte> buffer, CancellationToken cancellationToken)
      => throw new NotImplementedException();

    public ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken)
      => throw new NotImplementedException();
  }

  [Test]
  public ValueTask OpenEndPoint_WithCancellationToken()
    => OpenEndPoint_WithCancellationToken_Core(
      static (d, cancellationToken) => new ValueTask<PseudoUsbHidEndPoint>(
        (PseudoUsbHidEndPoint)d.OpenEndPoint(cancellationToken)
      )
    );

  [Test]
  public ValueTask OpenEndPointAsync_WithCancellationToken()
    => OpenEndPoint_WithCancellationToken_Core(
      async static (d, cancellationToken)
        => (PseudoUsbHidEndPoint)await d.OpenEndPointAsync(cancellationToken).ConfigureAwait(false)
    );

  private async ValueTask OpenEndPoint_WithCancellationToken_Core(
    Func<IUsbHidDevice, CancellationToken, ValueTask<PseudoUsbHidEndPoint>> openEndPointFunc
  )
  {
    using var cts = new CancellationTokenSource();

    var device = new PseudoUsbHidDevice(0, 0);

    using var endPoint = await openEndPointFunc(device, cts.Token).ConfigureAwait(false);

    Assert.That(endPoint.ShouldDisposeDevice, Is.False);
    Assert.That(endPoint.CanWrite, Is.True);
    Assert.That(endPoint.CanRead, Is.True);
    Assert.That(endPoint.CancellationTokenPassedToOpenEndPoint, Is.EqualTo(cts.Token));
  }

  [Test]
  public ValueTask OpenEndPoint_WithShouldDisposeDevice(
    [Values] bool shouldDisposeDevice
  )
    => OpenEndPoint_WithShouldDisposeDevice_Core(
      shouldDisposeDevice,
      static (d, shouldDisposeDevice) => new ValueTask<PseudoUsbHidEndPoint>(
        (PseudoUsbHidEndPoint)d.OpenEndPoint(shouldDisposeDevice)
      )
    );

  [Test]
  public ValueTask OpenEndPointAsync_WithShouldDisposeDevice(
    [Values] bool shouldDisposeDevice
  )
    => OpenEndPoint_WithShouldDisposeDevice_Core(
      shouldDisposeDevice,
      async static (d, shouldDisposeDevice)
        => (PseudoUsbHidEndPoint)await d.OpenEndPointAsync(shouldDisposeDevice).ConfigureAwait(false)
    );

  private async ValueTask OpenEndPoint_WithShouldDisposeDevice_Core(
    bool shouldDisposeDevice,
    Func<IUsbHidDevice, bool, ValueTask<PseudoUsbHidEndPoint>> openEndPointFunc
  )
  {
    using var cts = new CancellationTokenSource();

    var device = new PseudoUsbHidDevice(0, 0);

    using var endPoint = await openEndPointFunc(device, shouldDisposeDevice).ConfigureAwait(false);

    Assert.That(endPoint.ShouldDisposeDevice, Is.EqualTo(shouldDisposeDevice));
    Assert.That(endPoint.CanWrite, Is.True);
    Assert.That(endPoint.CanRead, Is.True);
    Assert.That(endPoint.CancellationTokenPassedToOpenEndPoint, Is.EqualTo(CancellationToken.None));
  }

  [Test]
  public ValueTask OpenEndPoint_WithShouldDisposeDeviceAndCancellationToken(
    [Values] bool shouldDisposeDevice
  )
    => OpenEndPoint_WithShouldDisposeDeviceAndCancellationToken_Core(
      shouldDisposeDevice,
      static (d, shouldDisposeDevice, cancellationToken) => new ValueTask<PseudoUsbHidEndPoint>(
        (PseudoUsbHidEndPoint)d.OpenEndPoint(shouldDisposeDevice, cancellationToken)
      )
    );

  [Test]
  public ValueTask OpenEndPointAsync_WithShouldDisposeDeviceAndCancellationToken(
    [Values] bool shouldDisposeDevice
  )
    => OpenEndPoint_WithShouldDisposeDeviceAndCancellationToken_Core(
      shouldDisposeDevice,
      async static (d, shouldDisposeDevice, cancellationToken)
        => (PseudoUsbHidEndPoint)await d.OpenEndPointAsync(shouldDisposeDevice, cancellationToken).ConfigureAwait(false)
    );

  private async ValueTask OpenEndPoint_WithShouldDisposeDeviceAndCancellationToken_Core(
    bool shouldDisposeDevice,
    Func<IUsbHidDevice, bool, CancellationToken, ValueTask<PseudoUsbHidEndPoint>> openEndPointFunc
  )
  {
    using var cts = new CancellationTokenSource();

    var device = new PseudoUsbHidDevice(0, 0);

    using var endPoint = await openEndPointFunc(device, shouldDisposeDevice, cts.Token).ConfigureAwait(false);

    Assert.That(endPoint.ShouldDisposeDevice, Is.EqualTo(shouldDisposeDevice));
    Assert.That(endPoint.CanWrite, Is.True);
    Assert.That(endPoint.CanRead, Is.True);
    Assert.That(endPoint.CancellationTokenPassedToOpenEndPoint, Is.EqualTo(cts.Token));
  }

  [Test]
  public void ToIdentificationString_ArgumentNull()
  {
    IUsbHidDevice device = null!;

    Assert.That(
      () => device.ToIdentificationString(),
      Throws
        .ArgumentNullException
        .With
        .Property(nameof(ArgumentNullException.ParamName))
        .EqualTo("device")
    );
  }

  [TestCase(null, false)]
  [TestCase("", false)]
  [TestCase("0", true)]
  [TestCase("ID", true)]
  public void ToIdentificationString_WithDeviceIdentifier(string? deviceIdentifier, bool expectIdentificationStringEqualsToDeviceIdentifier)
  {
    var device = new PseudoUsbHidDevice(0, 0, deviceIdentifier: deviceIdentifier);

    Assert.That(
      device.ToIdentificationString(),
      expectIdentificationStringEqualsToDeviceIdentifier
        ? Is.EqualTo(deviceIdentifier)
        : Is.Not.EqualTo(deviceIdentifier)
    );
  }

  [TestCase("X")]
  [TestCase("serial-number")]
  public void ToIdentificationString_WithSerialNumber(string serialNumber)
  {
    var device = new PseudoUsbHidDevice(
      vendorId: 0xCAFE,
      productId: 0xBEEF,
      serialNumber: serialNumber
    );

    Assert.That(
      device.ToIdentificationString(),
      Is.EqualTo($"{{CAFE:BEEF;{serialNumber}}}")
    );
  }

  [TestCase(null, null)]
  [TestCase("", null)]
  [TestCase(null, "")]
  [TestCase("", "")]
  public void ToIdentificationString_WithoutDeviceIdentifierAndSerialNumber_WithToString(
    string? deviceIdentifier,
    string? serialNumber
  )
  {
    const string ToStringValue = nameof(PseudoUsbHidDevice);

    var device = new PseudoUsbHidDevice(
      vendorId: 0x04d8,
      productId: 0x003f,
      serialNumber: serialNumber,
      deviceIdentifier: deviceIdentifier,
      toStringValue: ToStringValue
    );

    Assert.That(
      device.ToIdentificationString(),
      Is.EqualTo(ToStringValue)
    );
  }

  [TestCase(null, null)]
  [TestCase("", null)]
  [TestCase(null, "")]
  [TestCase("", "")]
  public void ToIdentificationString_WithoutDeviceIdentifierAndSerialNumber_WithoutToString(
    string? deviceIdentifier,
    string? serialNumber
  )
  {
    var device = new PseudoUsbHidDevice(
      vendorId: 0x04d8,
      productId: 0x003f,
      serialNumber: serialNumber,
      deviceIdentifier: deviceIdentifier
    );

    Assert.That(
      device.ToIdentificationString(),
      Is.EqualTo("{04D8:003F}")
    );
  }
}
