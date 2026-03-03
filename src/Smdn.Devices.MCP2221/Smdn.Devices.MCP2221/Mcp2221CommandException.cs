// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT

using System;

namespace Smdn.Devices.MCP2221;

public class Mcp2221CommandException : InvalidOperationException {
  public Mcp2221CommandException() : base("command failed") { }
  public Mcp2221CommandException(string message) : base(message) { }
  public Mcp2221CommandException(string message, Exception innerException) : base(message, innerException) { }
}
