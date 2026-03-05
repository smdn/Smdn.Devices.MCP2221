// SPDX-FileCopyrightText: 2026 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System.Threading;
using System.Threading.Tasks;

namespace Smdn.Devices.Mcp2221A;

#pragma warning disable IDE0040, CA1724
partial class Mcp2221A {
#pragma warning restore IDE0040, CA1724
  public static ValueTask<Mcp2221A> CreateAsync(
    CancellationToken cancellationToken = default
  )
    // future: create with implementation using linux kernel module
    => CreateWithDeviceFactoryAsyncCore(
      usbHidDeviceFactory: Mcp2221ADefaultUsbHidDeviceFactory.Instance,
      serviceProvider: null,
      serviceKey: (object?)null,
      predicate: null,
      cancellationToken: cancellationToken
    );

  public static Mcp2221A Create(
    CancellationToken cancellationToken = default
  )
    // future: create with implementation using linux kernel module
    => CreateWithDeviceFactoryCore(
      usbHidDeviceFactory: Mcp2221ADefaultUsbHidDeviceFactory.Instance,
      serviceProvider: null,
      serviceKey: (object?)null,
      predicate: null,
      cancellationToken: cancellationToken
    );
}
