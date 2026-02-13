// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT

using System;

namespace Smdn.Devices.MCP2221;

internal class DeviceNotSupportedException : NotSupportedException {
  public DeviceNotSupportedException()
    : base("requested device is not supported")
  {
  }

  public DeviceNotSupportedException(string message)
    : base(message)
  {
  }

  public DeviceNotSupportedException(string message, Exception innerException)
    : base(message, innerException)
  {
  }
}
