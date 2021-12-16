// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT

using System;

namespace Smdn.Devices.MCP2221;

public class I2CCommandException : CommandException {
  public I2CAddress Address { get; }

  public I2CCommandException(string message) : this(I2CAddress.Zero, message) { }
  public I2CCommandException(string message, Exception innerException) : this(I2CAddress.Zero, message, innerException) { }

  public I2CCommandException(I2CAddress address, string message) : base(message)
    => this.Address = address;

  public I2CCommandException(I2CAddress address, string message, Exception innerException) : base(message, innerException)
    => this.Address = address;
}
