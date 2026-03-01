// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT

using System;

using Smdn.IO.UsbHid;

namespace Smdn.Devices.MCP2221;

public class DeviceNotFoundException : InvalidOperationException {
  public DeviceNotFoundException()
    : base("MCP2221/MCP2221A not found")
  {
  }

  public DeviceNotFoundException(string message)
    : base(message)
  {
  }

  public DeviceNotFoundException(string message, Exception innerException)
    : base(message, innerException)
  {
  }

  internal DeviceNotFoundException(IUsbHidService usbHidService, Predicate<IUsbHidDevice>? predicate)
    : base($"{nameof(IUsbHidService)} could not find an MCP2221/MCP2221A matching the specified predicate. ({nameof(IUsbHidService)}: {usbHidService}, {nameof(predicate)}: {predicate?.ToString() ?? "null"})")
  {
  }
}
