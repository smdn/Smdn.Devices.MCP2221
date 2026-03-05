// SPDX-FileCopyrightText: 2026 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Smdn.IO.UsbHid;

namespace Smdn.Devices.Mcp2221A;

#pragma warning disable IDE0040, CA1724
partial class Mcp2221A {
#pragma warning restore IDE0040, CA1724
  public static ValueTask<Mcp2221A> CreateAsync(
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

  public static Mcp2221A Create(
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

  public static ValueTask<Mcp2221A> CreateAsync<TServiceKey>(
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

  public static Mcp2221A Create<TServiceKey>(
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

  private static async ValueTask<Mcp2221A> CreateFromUsbHidDeviceAsyncCore<TServiceKey>(
    IUsbHidDevice usbHidDevice,
    IServiceProvider? serviceProvider,
#pragma warning disable IDE0060
    TServiceKey? serviceKey, // for future extension
#pragma warning restore IDE0060
    bool shouldDisposeUsbHidDevice,
    CancellationToken cancellationToken
  )
  {
    Mcp2221A? device = null;

    try {
      device = new Mcp2221A(
        hidDevice: usbHidDevice,
        shouldDisposeUsbHidDevice: shouldDisposeUsbHidDevice,
        logger: serviceProvider?.GetService<ILoggerFactory>()?.CreateLogger<Mcp2221A>()
      );

      try {
        await device.OpenEndPointAsync(
          cancellationToken: cancellationToken
        ).ConfigureAwait(false);
      }
      catch (Exception ex) {
        throw new Mcp2221AUnavailableException(ex, usbHidDevice);
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

  private static Mcp2221A CreateFromUsbHidDeviceCore<TServiceKey>(
    IUsbHidDevice usbHidDevice,
    bool shouldDisposeUsbHidDevice,
    IServiceProvider? serviceProvider,
#pragma warning disable IDE0060
    TServiceKey? serviceKey, // for future extension
#pragma warning restore IDE0060
    CancellationToken cancellationToken
  )
  {
    Mcp2221A? device = null;

    try {
      device = new Mcp2221A(
        hidDevice: usbHidDevice,
        shouldDisposeUsbHidDevice: shouldDisposeUsbHidDevice,
        logger: serviceProvider?.GetService<ILoggerFactory>()?.CreateLogger<Mcp2221A>()
      );

      try {
        device.OpenEndPoint(
          cancellationToken: cancellationToken
        );
      }
      catch (Exception ex) {
        throw new Mcp2221AUnavailableException(ex, usbHidDevice);
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
