// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT

using System;

namespace Smdn.Devices.MCP2221;

public class I2CReadException : I2CCommandException {
  public I2CReadException(string message) : base(message) { }
  public I2CReadException(string message, Exception innerException) : base(message, innerException) { }
  public I2CReadException(I2CAddress address, string message) : base(address, message) { }
  public I2CReadException(I2CAddress address, string message, Exception innerException) : base(address, message, innerException) { }
}
