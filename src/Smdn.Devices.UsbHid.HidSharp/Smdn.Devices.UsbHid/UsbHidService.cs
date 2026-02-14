// SPDX-FileCopyrightText: 2026 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using HidSharp;

using Microsoft.Extensions.Logging;

namespace Smdn.Devices.UsbHid;

internal sealed class UsbHidService(
  ILogger? logger,
  IServiceProvider? serviceProvider,
  object? serviceKey
) : IUsbHidService {
  public IReadOnlyList<IUsbHidDevice> GetDevices(
    CancellationToken cancellationToken = default
  )
  {
    cancellationToken.ThrowIfCancellationRequested();

    return DeviceList
      .Local
      .GetHidDevices()
      .Select(
        d => new UsbHidDevice(d, logger, serviceProvider, serviceKey)
      )
      .ToList();
  }

  public void Dispose()
  {
    // nothing to do
  }

  public ValueTask DisposeAsync()
  {
    // nothing to do
    return default;
  }

  public override string? ToString()
    => GetType().Assembly.GetName().Name;
}
