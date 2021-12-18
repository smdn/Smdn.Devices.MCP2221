// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
#pragma warning disable CA1069 // CA1069: Enums should not have duplicate values

namespace Smdn.Devices.MCP2221;

public enum I2CBusSpeed {
  Default = default,

  StandardMode = Default,
  LowSpeedMode,
  FastMode,

  Speed10kBitsPerSec = LowSpeedMode,
  Speed100kBitsPerSec = StandardMode,
  Speed400kBitsPerSec = FastMode,
}
