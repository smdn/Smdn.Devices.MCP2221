// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT

using System;

namespace Smdn.Devices.MCP2221 {
  [Obsolete("use System.Device.Gpio.PinValue")]
  public enum GPIOLevel : byte {
    Low = 0x00,
    High = 0x01,
  }
}
