// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT

using System;

using Smdn.IO.UsbHid;

namespace Smdn.Devices.MCP2221;

public class Mcp2221UnavailableException : UnauthorizedAccessException {
  private const string DefaultMessage = "The requested MCP2221/MCP2221A is unavailable due to reasons such as unprivileged access, being disconnected, or being blocked by another driver.";

  public Mcp2221UnavailableException()
    : base(DefaultMessage)
  {
  }

  public Mcp2221UnavailableException(string message)
    : base(message)
  {
  }

  public Mcp2221UnavailableException(string message, Exception innerException)
    : base(message, innerException)
  {
  }

  public Mcp2221UnavailableException(Exception innerException, IUsbHidDevice? device = null)
    : base(
      message: $"{DefaultMessage} (device='{device?.ToIdentificationString() ?? "?"}')",
      inner: innerException
    )
  {
  }
}
