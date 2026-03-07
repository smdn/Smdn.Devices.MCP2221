// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT

using System;

namespace Smdn.Devices.Mcp2221A.Peripherals.I2c;

public class I2cCommandException : Mcp2221ACommandException {
  private const string DefaultMessage = "The requested I2C command failed.";

  public I2cAddress Address { get; }

  public I2cCommandException()
    : this(I2cAddress.Zero, DefaultMessage)
  {
  }

  public I2cCommandException(string? message)
    : this(I2cAddress.Zero, message)
  {
  }

  public I2cCommandException(string? message, Exception? innerException)
    : this(I2cAddress.Zero, message, innerException)
  {
  }

  public I2cCommandException(I2cAddress address, string? message)
    : this(address, message, innerException: null)
  {
  }

  public I2cCommandException(I2cAddress address, string? message, Exception? innerException)
    : base(message ?? DefaultMessage, innerException)
  {
    Address = address;
  }
}
