// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT

using System;

using Smdn.IO.UsbHid;

namespace Smdn.Devices.MCP2221;

public class Mcp2221NotFoundException : InvalidOperationException {
  public Mcp2221NotFoundException()
    : base("MCP2221/MCP2221A not found")
  {
  }

  public Mcp2221NotFoundException(string message)
    : base(message)
  {
  }

  public Mcp2221NotFoundException(string message, Exception innerException)
    : base(message, innerException)
  {
  }

  internal Mcp2221NotFoundException(IUsbHidService usbHidService, Predicate<IUsbHidDevice>? predicate)
    : base($"{nameof(IUsbHidService)} could not find an MCP2221/MCP2221A matching the specified predicate. ({nameof(IUsbHidService)}: {usbHidService}, {nameof(predicate)}: {predicate?.ToString() ?? "null"})")
  {
  }
}
