// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT

using System;

namespace Smdn.Devices.Mcp2221A.Peripherals.I2c;

public class I2cReadException : I2cCommandException {
  private const string DefaultMessage = "The requested I2C read operation failed.";

  public I2cReadException()
    : base(DefaultMessage)
  {
  }

  public I2cReadException(string? message)
    : base(message ?? DefaultMessage)
  {
  }

  public I2cReadException(string? message, Exception? innerException)
    : base(message ?? DefaultMessage, innerException)
  {
  }

  public I2cReadException(I2cAddress address, string? message)
    : base(address, message ?? DefaultMessage)
  {
  }

  public I2cReadException(I2cAddress address, string? message, Exception? innerException)
    : base(address, message ?? DefaultMessage, innerException)
  {
  }
}
