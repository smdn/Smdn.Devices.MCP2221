// SPDX-FileCopyrightText: 2026 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

using Smdn.IO.UsbHid;

namespace Smdn.Devices.Mcp2221A;

internal sealed class Mcp2221ADefaultUsbHidDeviceFactory : IMcp2221AUsbHidDeviceFactory {
  public static readonly Mcp2221ADefaultUsbHidDeviceFactory Instance = new();

  [DoesNotReturn]
  private static void ThrowServiceProviderMustBeProvidedException()
    => throw new InvalidOperationException($"{nameof(IServiceProvider)} must be provided in order to create {nameof(IUsbHidDevice)} if {nameof(IMcp2221AUsbHidDeviceFactory)} is not provided.");

  public ValueTask<IUsbHidDevice> CreateAsync<TServiceKey>(
    IServiceProvider? serviceProvider,
    TServiceKey? serviceKey,
    Predicate<IUsbHidDevice>? predicate,
    CancellationToken cancellationToken
  )
#pragma warning disable SA1114
#if SYSTEM_THREADING_TASKS_VALUETASK_FROMRESULT
    => ValueTask.FromResult<IUsbHidDevice>(
#else
    => new(
#endif
#pragma warning disable CA2000
      result: Create(
        serviceProvider: serviceProvider,
        serviceKey: serviceKey,
        predicate: predicate,
        cancellationToken: cancellationToken
      )
#pragma warning restore CA2000
    );
#pragma warning restore SA1114

  public IUsbHidDevice Create<TServiceKey>(
    IServiceProvider? serviceProvider,
    TServiceKey? serviceKey,
    Predicate<IUsbHidDevice>? predicate,
    CancellationToken cancellationToken
  )
  {
    if (serviceProvider is null)
      ThrowServiceProviderMustBeProvidedException();

    cancellationToken.ThrowIfCancellationRequested();

    var usbHidService =
      serviceProvider.GetKeyedService<IUsbHidService>(serviceKey) ??
      serviceProvider.GetRequiredService<IUsbHidService>();

    return usbHidService.FindDevice(
      Mcp2221A.DeviceVendorId,
      Mcp2221A.DeviceProductId,
      predicate,
      cancellationToken
    ) ?? throw new Mcp2221ANotFoundException(usbHidService, predicate);
  }
}
