// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT

using System;

namespace Smdn.Devices.MCP2221 {
  [Obsolete("use System.Device.Gpio.PinMode")]
  public enum GPIODirection : byte {
    Output = 0x00,
    Input = 0x01,
  }
}
