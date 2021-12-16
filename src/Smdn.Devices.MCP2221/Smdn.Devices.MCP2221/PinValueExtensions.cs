// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT

using System.Device.Gpio;

namespace Smdn.Devices.MCP2221;

// Language Feature: Extension Everything
// https://github.com/dotnet/roslyn/issues/11159
// public extension class PinValueExtensions : PinValue { }
internal static class PinValueExtensions {
  public static bool IsLow(this PinValue val) => PinValue.Low.Equals(val);
  public static bool IsHigh(this PinValue val) => !PinValue.Low.Equals(val);
}
