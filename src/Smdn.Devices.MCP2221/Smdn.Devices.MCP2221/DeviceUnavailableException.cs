// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT

using System;

using Smdn.Devices.UsbHid;

namespace Smdn.Devices.MCP2221 {
  class DeviceUnavailableException : UnauthorizedAccessException {
    public DeviceUnavailableException(Exception innerException)
      : base("MCP2221/MCP2221A is not available, not privileged or disconnected.", innerException)
    {
    }
  }
}
