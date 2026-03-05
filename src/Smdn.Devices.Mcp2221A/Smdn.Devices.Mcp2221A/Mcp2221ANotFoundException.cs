// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT

using System;

using Smdn.IO.UsbHid;

namespace Smdn.Devices.Mcp2221A;

public class Mcp2221ANotFoundException : InvalidOperationException {
  public Mcp2221ANotFoundException()
    : base("MCP2221/MCP2221A not found")
  {
  }

  public Mcp2221ANotFoundException(string message)
    : base(message)
  {
  }

  public Mcp2221ANotFoundException(string message, Exception innerException)
    : base(message, innerException)
  {
  }

  internal Mcp2221ANotFoundException(IUsbHidService usbHidService, Predicate<IUsbHidDevice>? predicate)
    : base($"{nameof(IUsbHidService)} could not find an MCP2221/MCP2221A matching the specified predicate. ({nameof(IUsbHidService)}: {usbHidService}, {nameof(predicate)}: {predicate?.ToString() ?? "null"})")
  {
  }
}
