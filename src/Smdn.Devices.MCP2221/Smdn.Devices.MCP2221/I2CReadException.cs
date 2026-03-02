// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT

using System;

namespace Smdn.Devices.MCP2221;

public class I2cReadException : I2cCommandException {
  public I2cReadException() : base("The requested I2C read operation failed.") { }
  public I2cReadException(string message) : base(message) { }
  public I2cReadException(string message, Exception innerException) : base(message, innerException) { }
  public I2cReadException(I2cAddress address, string message) : base(address, message) { }
  public I2cReadException(I2cAddress address, string message, Exception innerException) : base(address, message, innerException) { }
}
