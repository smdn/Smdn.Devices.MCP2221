// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT

using System;

namespace Smdn.Devices.MCP2221 {
  public readonly struct GPIOValue /* TODO: IEquatable<GPIOValue/int/byte/bool>*/ {
    public static readonly GPIOValue Default = default;
    public static readonly GPIOValue Low = new GPIOValue(GPIOLevel.Low);
    public static readonly GPIOValue High = new GPIOValue(GPIOLevel.High);

    private readonly GPIOLevel state;

    public bool IsHigh => state != GPIOLevel.Low;
    public bool IsLow => state == GPIOLevel.Low;

    public GPIOValue(byte value)
    {
      this.state = value == 0 ? GPIOLevel.Low : GPIOLevel.High;
    }

    public static explicit operator byte(GPIOValue value) => value.state == GPIOLevel.Low ? (byte)0x00 : (byte)0x01;
    public static implicit operator GPIOValue(byte value) => new GPIOValue(value);

    public GPIOValue(int value)
    {
      this.state = value == 0 ? GPIOLevel.Low : GPIOLevel.High;
    }

    public static explicit operator int(GPIOValue value) => value.state == GPIOLevel.Low ? 0x00 : 0x01;
    public static implicit operator GPIOValue(int value) => new GPIOValue(value);

    public GPIOValue(bool value)
    {
      this.state = value ? GPIOLevel.High : GPIOLevel.Low;
    }

    public static explicit operator bool(GPIOValue value) => value.state == GPIOLevel.Low ? false : true;
    public static implicit operator GPIOValue(bool value) => new GPIOValue(value);

    public GPIOValue(GPIOLevel state)
    {
      this.state = state switch {
        GPIOLevel.Low => GPIOLevel.Low,
        GPIOLevel.High => GPIOLevel.High,
        _ => throw new ArgumentOutOfRangeException(nameof(state), state, $"must be {nameof(GPIOLevel.Low)} or {nameof(GPIOLevel.High)}"),
      };
    }

    public static explicit operator GPIOLevel(GPIOValue value) => value.state;
    public static implicit operator GPIOValue(GPIOLevel value) => value == GPIOLevel.Low ? Low : High;

    public override string ToString() => state.ToString();
  }
}
