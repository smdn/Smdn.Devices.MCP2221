// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT

using System;

namespace Smdn.Devices.Mcp2221A;

public class Mcp2221ACommandException : InvalidOperationException {
  private const string DefaultMessage = "The command to the MCP2221/MCP2221A failed.";

  public Mcp2221ACommandException()
    : base(DefaultMessage)
  {
  }

  public Mcp2221ACommandException(string? message)
    : base(message ?? DefaultMessage)
  {
  }

  public Mcp2221ACommandException(string? message, Exception? innerException)
    : base(message ?? DefaultMessage, innerException)
  {
  }
}
