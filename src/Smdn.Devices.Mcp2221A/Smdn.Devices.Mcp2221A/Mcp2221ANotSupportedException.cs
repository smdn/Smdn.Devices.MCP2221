// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT

using System;

namespace Smdn.Devices.Mcp2221A;

public class Mcp2221ANotSupportedException : NotSupportedException {
  public Mcp2221ANotSupportedException()
    : base("The requested MCP2221/MCP2221A is a device with an unsupported hardware revision and/or firmware revision.")
  {
  }

  public Mcp2221ANotSupportedException(string message)
    : base(message)
  {
  }

  public Mcp2221ANotSupportedException(string message, Exception innerException)
    : base(message, innerException)
  {
  }
}
