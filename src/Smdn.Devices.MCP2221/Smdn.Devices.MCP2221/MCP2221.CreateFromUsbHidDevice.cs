// SPDX-FileCopyrightText: 2026 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Smdn.IO.UsbHid;

namespace Smdn.Devices.MCP2221;

#pragma warning disable IDE0040, CA1724
partial class MCP2221 {
#pragma warning restore IDE0040, CA1724
  public static ValueTask<MCP2221> CreateAsync(
    IUsbHidDevice usbHidDevice,
    bool shouldDisposeUsbHidDevice = false,
    IServiceProvider? serviceProvider = null,
    CancellationToken cancellationToken = default
  )
    => CreateFromUsbHidDeviceAsyncCore(
      usbHidDevice: usbHidDevice ?? throw new ArgumentNullException(nameof(usbHidDevice)),
      shouldDisposeUsbHidDevice: shouldDisposeUsbHidDevice,
      serviceProvider: serviceProvider,
      serviceKey: (object?)null,
      cancellationToken: cancellationToken
    );

  public static MCP2221 Create(
    IUsbHidDevice usbHidDevice,
    bool shouldDisposeUsbHidDevice = false,
    IServiceProvider? serviceProvider = null,
    CancellationToken cancellationToken = default
  )
    => CreateFromUsbHidDeviceCore(
      usbHidDevice: usbHidDevice ?? throw new ArgumentNullException(nameof(usbHidDevice)),
      shouldDisposeUsbHidDevice: shouldDisposeUsbHidDevice,
      serviceProvider: serviceProvider,
      serviceKey: (object?)null,
      cancellationToken: cancellationToken
    );

  public static ValueTask<MCP2221> CreateAsync<TServiceKey>(
    IUsbHidDevice usbHidDevice,
    IServiceProvider serviceProvider,
    TServiceKey serviceKey,
    bool shouldDisposeUsbHidDevice = false,
    CancellationToken cancellationToken = default
  )
    => CreateFromUsbHidDeviceAsyncCore(
      usbHidDevice: usbHidDevice ?? throw new ArgumentNullException(nameof(usbHidDevice)),
      shouldDisposeUsbHidDevice: shouldDisposeUsbHidDevice,
      serviceProvider: serviceProvider,
      serviceKey: serviceKey,
      cancellationToken: cancellationToken
    );

  public static MCP2221 Create<TServiceKey>(
    IUsbHidDevice usbHidDevice,
    IServiceProvider serviceProvider,
    TServiceKey serviceKey,
    bool shouldDisposeUsbHidDevice = false,
    CancellationToken cancellationToken = default
  )
    => CreateFromUsbHidDeviceCore(
      usbHidDevice: usbHidDevice ?? throw new ArgumentNullException(nameof(usbHidDevice)),
      shouldDisposeUsbHidDevice: shouldDisposeUsbHidDevice,
      serviceProvider: serviceProvider,
      serviceKey: serviceKey,
      cancellationToken: cancellationToken
    );

  private static async ValueTask<MCP2221> CreateFromUsbHidDeviceAsyncCore<TServiceKey>(
    IUsbHidDevice usbHidDevice,
    IServiceProvider? serviceProvider,
#pragma warning disable IDE0060
    TServiceKey? serviceKey, // for future extension
#pragma warning restore IDE0060
    bool shouldDisposeUsbHidDevice,
    CancellationToken cancellationToken
  )
  {
    MCP2221? device = null;

    try {
      device = new MCP2221(
        hidDevice: usbHidDevice,
        shouldDisposeUsbHidDevice: shouldDisposeUsbHidDevice,
        logger: serviceProvider?.GetService<ILoggerFactory>()?.CreateLogger<MCP2221>()
      );

      try {
        await device.OpenEndPointAsync(
          cancellationToken: cancellationToken
        ).ConfigureAwait(false);
      }
      catch (Exception ex) {
        throw new DeviceUnavailableException(ex, usbHidDevice);
      }

      await device.RetrieveChipInformationAsync(
        ValidateHardwareRevision,
        ValidateFirmwareRevision,
        cancellationToken: cancellationToken
      ).ConfigureAwait(false);

      return device;
    }
    catch {
      await device!.DisposeAsync().ConfigureAwait(false);

      throw;
    }
  }

  private static MCP2221 CreateFromUsbHidDeviceCore<TServiceKey>(
    IUsbHidDevice usbHidDevice,
    bool shouldDisposeUsbHidDevice,
    IServiceProvider? serviceProvider,
#pragma warning disable IDE0060
    TServiceKey? serviceKey, // for future extension
#pragma warning restore IDE0060
    CancellationToken cancellationToken
  )
  {
    MCP2221? device = null;

    try {
      device = new MCP2221(
        hidDevice: usbHidDevice,
        shouldDisposeUsbHidDevice: shouldDisposeUsbHidDevice,
        logger: serviceProvider?.GetService<ILoggerFactory>()?.CreateLogger<MCP2221>()
      );

      try {
        device.OpenEndPoint(
          cancellationToken: cancellationToken
        );
      }
      catch (Exception ex) {
        throw new DeviceUnavailableException(ex, usbHidDevice);
      }

      device.RetrieveChipInformation(
        ValidateHardwareRevision,
        ValidateFirmwareRevision,
        cancellationToken: cancellationToken
      );

      return device;
    }
    catch {
      device!.Dispose();

      throw;
    }
  }
}
