// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT

#nullable enable annotations

using System;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Smdn.Devices.UsbHid;

namespace Smdn.Devices.MCP2221;

#pragma warning disable IDE0055
public partial class MCP2221 :
  IDisposable,
  IAsyncDisposable
{
#pragma warning restore IDE0055
  public const int DeviceVendorID = 0x04d8;
  public const int DeviceProductID = 0x00dd;

  // MCP2221 (not tested)
  public const string HardwareRevisionMCP2221 = "A.6";
  public const string FirmwareRevisionMCP2221 = "1.1";

  // MCP2221A
  public const string HardwareRevisionMCP2221A = "A.6";
  public const string FirmwareRevisionMCP2221A = "1.2";

  public static ValueTask<MCP2221> OpenAsync(IServiceProvider? serviceProvider = null)
    => OpenAsync(findDevicePredicate: null, serviceProvider);

  public static MCP2221 Open(IServiceProvider? serviceProvider = null)
    => Open(findDevicePredicate: null, serviceProvider);

  private class MCP2221DeviceFinder {
    private readonly Predicate<IUsbHidDevice> findDevicePredicate;

    public MCP2221DeviceFinder(Predicate<IUsbHidDevice> findDevicePredicate)
    {
      this.findDevicePredicate = findDevicePredicate;
    }

    public bool Find(IUsbHidDevice device)
    {
      if (device.VendorID != DeviceVendorID)
        return false;
      if (device.ProductID != DeviceProductID)
        return false;
      if (findDevicePredicate is null)
        return true; // select first device

      return findDevicePredicate(device);
    }
  }

  private static Func<IUsbHidDevice> Create(Predicate<IUsbHidDevice> findDevicePredicate, IServiceProvider serviceProvider)
  {
    Predicate<IUsbHidDevice> predicate = new MCP2221DeviceFinder(findDevicePredicate).Find;

#if USBHIDDRIVER_HIDSHARP
    return () => Smdn.Devices.UsbHid.HidSharp.Device.Find(predicate, serviceProvider);
#elif USBHIDDRIVER_LIBUSBDOTNET
    return () => Smdn.Devices.UsbHid.LibUsbDotNet.Device.Find(predicate, serviceProvider);
#else
#error USB-HID driver must be specified.
    throw new NotImplementedException("USB-HID driver must be specified");
#endif
  }

  public static ValueTask<MCP2221> OpenAsync(
    Predicate<IUsbHidDevice>? findDevicePredicate,
    IServiceProvider? serviceProvider = null
  )
    => OpenAsync(
      Create(findDevicePredicate, serviceProvider),
      serviceProvider
    );

  public static MCP2221 Open(
    Predicate<IUsbHidDevice>? findDevicePredicate,
    IServiceProvider? serviceProvider = null
  )
    => Open(
      Create(findDevicePredicate, serviceProvider),
      serviceProvider
    );

  private static void ValidateHardwareRevision(string revision)
  {
    switch (revision) {
      // case HardwareRevisionMCP2221A:
      case HardwareRevisionMCP2221:
        break;

      default:
        throw new DeviceNotSupportedException($"hardware revision '{revision}' is not supported");
    }
  }

  private static void ValidateFirmwareRevision(string revision)
  {
    switch (revision) {
      case FirmwareRevisionMCP2221:
      case FirmwareRevisionMCP2221A:
        break;

      default:
        throw new DeviceNotSupportedException($"firmware revision '{revision}' is not supported");
    }
  }

  public static async ValueTask<MCP2221> OpenAsync(Func<IUsbHidDevice> createHidDevice, IServiceProvider? serviceProvider = null)
  {
    if (createHidDevice is null)
      throw new ArgumentNullException(nameof(createHidDevice));

    MCP2221 device = null;

    try {
      IUsbHidDevice baseDevice = null;

      try {
        baseDevice = createHidDevice() ?? throw new DeviceNotFoundException();

        device = new MCP2221(
          baseDevice,
          await baseDevice.OpenStreamAsync().ConfigureAwait(false),
          serviceProvider
        );
      }
      catch (Exception ex) when (ex is not DeviceNotFoundException) {
        throw new DeviceUnavailableException(ex, baseDevice);
      }

      await device.RetrieveChipInformationAsync(
        ValidateHardwareRevision,
        ValidateFirmwareRevision
      ).ConfigureAwait(false);

      return device;
    }
    catch {
      if (device != null)
        await device.DisposeAsync().ConfigureAwait(false);

      throw;
    }
  }

  public static MCP2221 Open(Func<IUsbHidDevice> createHidDevice, IServiceProvider? serviceProvider = null)
  {
    if (createHidDevice is null)
      throw new ArgumentNullException(nameof(createHidDevice));

    MCP2221 device = null;

    try {
      IUsbHidDevice baseDevice = null;

      try {
        baseDevice = createHidDevice() ?? throw new DeviceNotFoundException();

        device = new MCP2221(
          baseDevice,
          baseDevice.OpenStream(),
          serviceProvider
        );
      }
      catch (Exception ex) when (ex is not DeviceNotFoundException) {
        throw new DeviceUnavailableException(ex, baseDevice);
      }

      device.RetrieveChipInformation(
        ValidateHardwareRevision,
        ValidateFirmwareRevision
      );

      return device;
    }
    catch {
      if (device != null)
        device.Dispose();

      throw;
    }
  }

  /*
   * instance members
   */
  private IUsbHidDevice hidDevice;
  public IUsbHidDevice HidDevice => hidDevice ?? throw new ObjectDisposedException(GetType().Name);

  private IUsbHidStream hidStream;
  private IUsbHidStream HidStream => hidStream ?? throw new ObjectDisposedException(GetType().Name);

  private readonly ILogger logger;

  public string HardwareRevision { get; private set; } = null;
  public string FirmwareRevision { get; private set; } = null;
  public string ManufacturerDescriptor { get; private set; } = null;
  public string ProductDescriptor { get; private set; } = null;
  public string SerialNumberDescriptor { get; private set; } = null;

  /// <remarks>Always returns <c>01234567</c>.</remarks>
  public string ChipFactorySerialNumber { get; private set; } = null;

  private MCP2221(IUsbHidDevice hidDevice, IUsbHidStream hidStream, IServiceProvider? serviceProvider)
  {
    this.hidDevice = hidDevice ?? throw new ArgumentNullException(nameof(hidDevice));
    this.hidStream = hidStream ?? throw new ArgumentNullException(nameof(hidStream));

    this.GP0 = new GP0Functionality(this);
    this.GP1 = new GP1Functionality(this);
    this.GP2 = new GP2Functionality(this);
    this.GP3 = new GP3Functionality(this);
    this.GPs = new GPFunctionality[] {
      this.GP0,
      this.GP1,
      this.GP2,
      this.GP3,
    };

    this.I2C = new I2CFunctionality(this);

    this.logger = serviceProvider?.GetService<ILoggerFactory>()?.CreateLogger<MCP2221>();
  }

  public void Dispose()
  {
    hidStream?.Dispose();
    hidStream = null;

    hidDevice?.Dispose();
    hidDevice = null;
  }

  public async ValueTask DisposeAsync()
  {
    if (hidStream != null) {
      await hidStream.DisposeAsync().ConfigureAwait(false);
      hidStream = null;
    }

    if (hidDevice != null) {
      await hidDevice.DisposeAsync().ConfigureAwait(false);
      hidDevice = null;
    }
  }

  private void ThrowIfDisposed() => _ = hidStream ?? throw new ObjectDisposedException(GetType().Name);
}
