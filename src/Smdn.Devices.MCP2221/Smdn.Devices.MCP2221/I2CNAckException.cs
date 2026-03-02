// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT

using System;

namespace Smdn.Devices.MCP2221;

public class I2cNAckException : I2cCommandException {
  public I2cNAckException() : base("A NACK response was returned for the requested I2C command.") { }
  public I2cNAckException(string message) : base(message) { }
  public I2cNAckException(string message, Exception innerException) : base(message, innerException) { }
  public I2cNAckException(I2cAddress address) : base(address, CreateDefaultMessage(address)) { }
  public I2cNAckException(I2cAddress address, Exception innerException) : base(address, CreateDefaultMessage(address), innerException) { }

  private static string CreateDefaultMessage(I2cAddress address) => $"I2C device 0x{address} not respond.";
}
