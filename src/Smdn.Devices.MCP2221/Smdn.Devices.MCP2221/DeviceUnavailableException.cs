// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT

using System;

using Smdn.Devices.UsbHid;

namespace Smdn.Devices.MCP2221;

internal class DeviceUnavailableException : UnauthorizedAccessException {
  public DeviceUnavailableException(Exception innerException, IUsbHidDevice? device = null)
    : base($"MCP2221/MCP2221A is not available, not privileged or disconnected. (device='{device?.FileSystemName ?? device?.DevicePath ?? "?"}')", innerException)
  {
  }
}
