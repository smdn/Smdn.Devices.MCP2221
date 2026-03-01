// SPDX-FileCopyrightText: 2026 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System.Threading;
using System.Threading.Tasks;

namespace Smdn.Devices.MCP2221;

#pragma warning disable IDE0040, CA1724
partial class MCP2221 {
#pragma warning restore IDE0040, CA1724
  public static ValueTask<MCP2221> CreateAsync(
    CancellationToken cancellationToken = default
  )
    // future: create with implementation using linux kernel module
    => CreateWithDeviceFactoryAsyncCore(
      usbHidDeviceFactory: Mcp2221DefaultUsbHidDeviceFactory.Instance,
      serviceProvider: null,
      serviceKey: (object?)null,
      predicate: null,
      cancellationToken: cancellationToken
    );

  public static MCP2221 Create(
    CancellationToken cancellationToken = default
  )
    // future: create with implementation using linux kernel module
    => CreateWithDeviceFactoryCore(
      usbHidDeviceFactory: Mcp2221DefaultUsbHidDeviceFactory.Instance,
      serviceProvider: null,
      serviceKey: (object?)null,
      predicate: null,
      cancellationToken: cancellationToken
    );
}
