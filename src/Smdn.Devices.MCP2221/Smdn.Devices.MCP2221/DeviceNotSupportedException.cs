// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT

using System;

namespace Smdn.Devices.MCP2221;

internal class DeviceNotSupportedException : NotSupportedException {
  public DeviceNotSupportedException(string message)
    : base(message)
  {
  }
}
