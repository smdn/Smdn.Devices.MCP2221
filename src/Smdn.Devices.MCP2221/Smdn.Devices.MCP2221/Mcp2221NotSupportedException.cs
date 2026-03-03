// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT

using System;

namespace Smdn.Devices.MCP2221;

public class Mcp2221NotSupportedException : NotSupportedException {
  public Mcp2221NotSupportedException()
    : base("The requested MCP2221/MCP2221A is a device with an unsupported hardware revision and/or firmware revision.")
  {
  }

  public Mcp2221NotSupportedException(string message)
    : base(message)
  {
  }

  public Mcp2221NotSupportedException(string message, Exception innerException)
    : base(message, innerException)
  {
  }
}
