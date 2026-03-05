// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT

using System;

using Smdn.IO.UsbHid;

namespace Smdn.Devices.Mcp2221A;

public class Mcp2221AUnavailableException : UnauthorizedAccessException {
  private const string DefaultMessage = "The requested MCP2221/MCP2221A is unavailable due to reasons such as unprivileged access, being disconnected, or being blocked by another driver.";

  public Mcp2221AUnavailableException()
    : base(DefaultMessage)
  {
  }

  public Mcp2221AUnavailableException(string message)
    : base(message)
  {
  }

  public Mcp2221AUnavailableException(string message, Exception innerException)
    : base(message, innerException)
  {
  }

  public Mcp2221AUnavailableException(Exception innerException, IUsbHidDevice? device = null)
    : base(
      message: $"{DefaultMessage} (device='{device?.ToIdentificationString() ?? "?"}')",
      inner: innerException
    )
  {
  }
}
