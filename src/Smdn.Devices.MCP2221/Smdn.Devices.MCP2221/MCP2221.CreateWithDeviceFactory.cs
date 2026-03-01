// SPDX-FileCopyrightText: 2026 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.Threading;
using System.Threading.Tasks;

using Smdn.IO.UsbHid;

namespace Smdn.Devices.MCP2221;

#pragma warning disable IDE0040, CA1724
partial class MCP2221 {
#pragma warning restore IDE0040, CA1724
  public static ValueTask<MCP2221> CreateAsync(
    IMcp2221UsbHidDeviceFactory usbHidDeviceFactory,
    IServiceProvider? serviceProvider = null,
    CancellationToken cancellationToken = default
  )
    => CreateWithDeviceFactoryAsyncCore(
      usbHidDeviceFactory: usbHidDeviceFactory ?? throw new ArgumentNullException(nameof(usbHidDeviceFactory)),
      serviceProvider: serviceProvider,
      serviceKey: (object?)null,
      predicate: null,
      cancellationToken: cancellationToken
    );

  public static MCP2221 Create(
    IMcp2221UsbHidDeviceFactory usbHidDeviceFactory,
    IServiceProvider? serviceProvider = null,
    CancellationToken cancellationToken = default
  )
    => CreateWithDeviceFactoryCore(
      usbHidDeviceFactory: usbHidDeviceFactory ?? throw new ArgumentNullException(nameof(usbHidDeviceFactory)),
      serviceProvider: serviceProvider,
      serviceKey: (object?)null,
      predicate: null,
      cancellationToken: cancellationToken
    );

  public static ValueTask<MCP2221> CreateAsync<TServiceKey>(
    IMcp2221UsbHidDeviceFactory usbHidDeviceFactory,
    IServiceProvider serviceProvider,
    TServiceKey serviceKey,
    CancellationToken cancellationToken = default
  )
    => CreateWithDeviceFactoryAsyncCore(
      usbHidDeviceFactory: usbHidDeviceFactory ?? throw new ArgumentNullException(nameof(usbHidDeviceFactory)),
      serviceProvider: serviceProvider,
      serviceKey: serviceKey,
      predicate: null,
      cancellationToken: cancellationToken
    );

  public static MCP2221 Create<TServiceKey>(
    IMcp2221UsbHidDeviceFactory usbHidDeviceFactory,
    IServiceProvider serviceProvider,
    TServiceKey serviceKey,
    CancellationToken cancellationToken = default
  )
    => CreateWithDeviceFactoryCore(
      usbHidDeviceFactory: usbHidDeviceFactory ?? throw new ArgumentNullException(nameof(usbHidDeviceFactory)),
      serviceProvider: serviceProvider,
      serviceKey: serviceKey,
      predicate: null,
      cancellationToken: cancellationToken
    );

  private static async ValueTask<MCP2221> CreateWithDeviceFactoryAsyncCore<TServiceKey>(
    IMcp2221UsbHidDeviceFactory usbHidDeviceFactory,
    IServiceProvider? serviceProvider,
    TServiceKey? serviceKey,
    Predicate<IUsbHidDevice>? predicate,
    CancellationToken cancellationToken = default
  )
    => await CreateFromUsbHidDeviceAsyncCore(
      usbHidDevice: await usbHidDeviceFactory.CreateAsync(
        serviceProvider,
        serviceKey,
        predicate,
        cancellationToken
      ).ConfigureAwait(false),
      shouldDisposeUsbHidDevice: true,
      serviceProvider: serviceProvider,
      serviceKey: serviceKey,
      cancellationToken: cancellationToken
    ).ConfigureAwait(false);

  private static MCP2221 CreateWithDeviceFactoryCore<TServiceKey>(
    IMcp2221UsbHidDeviceFactory usbHidDeviceFactory,
    IServiceProvider? serviceProvider,
    TServiceKey? serviceKey,
    Predicate<IUsbHidDevice>? predicate,
    CancellationToken cancellationToken = default
  )
    => CreateFromUsbHidDeviceCore(
      usbHidDevice: usbHidDeviceFactory.Create(
        serviceProvider,
        serviceKey,
        predicate,
        cancellationToken
      ),
      shouldDisposeUsbHidDevice: true,
      serviceProvider: serviceProvider,
      serviceKey: serviceKey,
      cancellationToken: cancellationToken
    );
}
