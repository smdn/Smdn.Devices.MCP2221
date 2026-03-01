// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
#if SYSTEM_DIAGNOSTICS_CODEANALYSIS_MEMBERNOTNULLATTRIBUTE
using System.Diagnostics.CodeAnalysis;
#endif
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using Smdn.IO.UsbHid;

namespace Smdn.Devices.MCP2221;

#pragma warning disable IDE0055, CA1724
public partial class MCP2221 :
  IDisposable,
  IAsyncDisposable
{
#pragma warning restore IDE0055, CA1724
  public const int DeviceVendorID = 0x04d8;
  public const int DeviceProductID = 0x00dd;

  // MCP2221 (not tested)
  public const string HardwareRevisionMCP2221 = "A.6";
  public const string FirmwareRevisionMCP2221 = "1.1";

  // MCP2221A
  public const string HardwareRevisionMCP2221A = "A.6";
  public const string FirmwareRevisionMCP2221A = "1.2";

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

  [Obsolete($"Use {nameof(CreateAsync)} with {nameof(IUsbHidDevice)} instead.", error: true)]
  public static async ValueTask<MCP2221> OpenAsync(Func<IUsbHidDevice?> createHidDevice, IServiceProvider? serviceProvider = null)
    => throw new NotSupportedException($"Use {nameof(CreateAsync)} with {nameof(IUsbHidDevice)} instead.");

  [Obsolete($"Use {nameof(Create)} with {nameof(IUsbHidDevice)} instead.", error: true)]
  public static MCP2221 Open(Func<IUsbHidDevice?> createHidDevice, IServiceProvider? serviceProvider = null)
    => throw new NotSupportedException($"Use {nameof(Create)} with {nameof(IUsbHidDevice)} instead.");

  /*
   * instance members
   */
  private readonly bool shouldDisposeUsbHidDevice;

  private IUsbHidDevice? hidDevice;
  public IUsbHidDevice HidDevice => hidDevice ?? throw new ObjectDisposedException(GetType().Name);

  private IUsbHidEndPoint? hidStream;
  private IUsbHidEndPoint HidStream => hidStream ?? throw new ObjectDisposedException(GetType().Name);

  private readonly ILogger? logger;

  public string? HardwareRevision { get; private set; } = null;
  public string? FirmwareRevision { get; private set; } = null;
  public string? ManufacturerDescriptor { get; private set; } = null;
  public string? ProductDescriptor { get; private set; } = null;
  public string? SerialNumberDescriptor { get; private set; } = null;

  /// <remarks>Always returns <c>01234567</c>.</remarks>
  public string? ChipFactorySerialNumber { get; private set; } = null;

  private MCP2221(
    IUsbHidDevice hidDevice,
    bool shouldDisposeUsbHidDevice,
    ILogger? logger
  )
  {
    this.hidDevice = hidDevice ?? throw new ArgumentNullException(nameof(hidDevice));
    this.shouldDisposeUsbHidDevice = shouldDisposeUsbHidDevice;

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

    this.logger = logger;
  }

  public void Dispose()
  {
    Dispose(disposing: true);

    GC.SuppressFinalize(this);
  }

  public async ValueTask DisposeAsync()
  {
    await DisposeAsyncCore().ConfigureAwait(false);

    Dispose(disposing: false);

    GC.SuppressFinalize(this);
  }

  protected virtual void Dispose(bool disposing)
  {
    if (disposing) {
      hidStream?.Dispose();
      hidStream = null;

      if (shouldDisposeUsbHidDevice)
        hidDevice?.Dispose();

      hidDevice = null;
    }
  }

  protected virtual async ValueTask DisposeAsyncCore()
  {
    if (hidStream is not null) {
      await hidStream.DisposeAsync().ConfigureAwait(false);
      hidStream = null;
    }

    if (hidDevice is not null) {
      if (shouldDisposeUsbHidDevice)
        await hidDevice.DisposeAsync().ConfigureAwait(false);

      hidDevice = null;
    }
  }

#if SYSTEM_DIAGNOSTICS_CODEANALYSIS_MEMBERNOTNULLATTRIBUTE
  [MemberNotNull(nameof(hidDevice))]
#endif
  private void ThrowIfDisposed() => _ = hidDevice ?? throw new ObjectDisposedException(GetType().Name);

  internal void OpenEndPoint(CancellationToken cancellationToken)
  {
    ThrowIfDisposed();

    hidStream =
#if SYSTEM_DIAGNOSTICS_CODEANALYSIS_MEMBERNOTNULLATTRIBUTE
      hidDevice
#else
      hidDevice!
#endif
        .OpenEndPoint(
          shouldDisposeDevice: false, // the source device must not be disposed when disposing of an endpoint
          cancellationToken: cancellationToken
        );
  }

  internal async ValueTask OpenEndPointAsync(CancellationToken cancellationToken)
  {
    ThrowIfDisposed();

    hidStream = await
#if SYSTEM_DIAGNOSTICS_CODEANALYSIS_MEMBERNOTNULLATTRIBUTE
      hidDevice
#else
      hidDevice!
#endif
        .OpenEndPointAsync(
          shouldDisposeDevice: false, // the source device must not be disposed when disposing of an endpoint
          cancellationToken: cancellationToken
        ).ConfigureAwait(false);
  }
}
