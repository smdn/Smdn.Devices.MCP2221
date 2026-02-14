// SPDX-FileCopyrightText: 2026 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using LibUsbDotNet;
using LibUsbDotNet.LibUsb;

using Microsoft.Extensions.Logging;

namespace Smdn.Devices.UsbHid;

internal sealed class UsbHidService : IUsbHidService {
  public LibUsbDotNetOptions Options { get; }

  private readonly ILogger? logger;
  private UsbContext context;

  public UsbHidService(
    LibUsbDotNetOptions options,
    ILogger? logger
  )
  {
    Options = options ?? throw new ArgumentNullException(nameof(options));

    this.logger = logger;

    context = new UsbContext();
    context.SetDebugLevel(Options.LibUsbDotNetDebugLevel);
  }

  public IReadOnlyList<IUsbHidDevice> GetDevices(
    CancellationToken cancellationToken = default
  )
  {
    cancellationToken.ThrowIfCancellationRequested();

    var deviceList = context.List();
    var list = new List<IUsbHidDevice>(capacity: deviceList.Count);

    foreach (var device in deviceList.OfType<UsbDevice>()) {
      if (device.Configs.SelectMany(c => c.Interfaces).Any(i => i.Class == ClassCode.Hid))
        list.Add(new UsbHidDevice(this, device, logger));
    }

    return list;
  }

  public void Dispose()
  {
    context.Dispose();
    context = null!;
  }

  public ValueTask DisposeAsync()
  {
    // UsbContext does not implement IAsyncDisposable
    context.Dispose();
    context = null!;

    return default;
  }

  public override string? ToString()
    => GetType().Assembly.GetName().Name;
}
