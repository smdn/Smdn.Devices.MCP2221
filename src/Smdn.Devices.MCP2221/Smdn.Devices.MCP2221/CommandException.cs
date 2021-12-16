// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT

using System;

using Smdn.Devices.UsbHid;

namespace Smdn.Devices.MCP2221;

public class CommandException : InvalidOperationException {
  public CommandException(string message) : base(message) { }
  public CommandException(string message, Exception innerException) : base(message, innerException) { }
}
