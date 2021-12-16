// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT

using System;

namespace Smdn.Devices.MCP2221;

public class I2CNAckException : I2CCommandException {
  public I2CNAckException(string message) : base(message) { }
  public I2CNAckException(string message, Exception innerException) : base(message, innerException) { }
  public I2CNAckException(I2CAddress address) : base(address, CreateDefaultMessage(address)) { }
  public I2CNAckException(I2CAddress address, Exception innerException) : base(address, CreateDefaultMessage(address), innerException) { }

  private static string CreateDefaultMessage(I2CAddress address) => $"I2C device 0x{address} not respond.";
}
