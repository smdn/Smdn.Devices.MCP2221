// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT

using System;

namespace Smdn.Devices.Mcp2221A.Peripherals.I2c;

public class I2cNackException : I2cCommandException {
  private const string DefaultMessage = "A NACK response was returned for the requested I2C command.";

  public I2cNackException()
    : base(DefaultMessage)
  {
  }

  public I2cNackException(string? message)
    : base(message ?? DefaultMessage)
  {
  }

  public I2cNackException(string? message, Exception? innerException)
    : base(message ?? DefaultMessage, innerException)
  {
  }

  public I2cNackException(I2cAddress address)
    : base(address, CreateDefaultMessage(address))
  {
  }

  public I2cNackException(I2cAddress address, Exception? innerException)
    : base(address, CreateDefaultMessage(address), innerException)
  {
  }

  private static string CreateDefaultMessage(I2cAddress address) => $"I2C device 0x{address} not respond.";
}
