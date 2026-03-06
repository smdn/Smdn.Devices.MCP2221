// SPDX-FileCopyrightText: 2026 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT

using System;
using System.Device.I2c;

namespace Smdn.Devices.Mcp2221A.Peripherals.I2c;

[CLSCompliant(false)]
public sealed class Mcp2221AI2cBus : I2cBus {
  private I2cController i2cBus;
  internal I2cController I2cBus => i2cBus ?? throw new ObjectDisposedException(GetType().FullName);

  private readonly bool shouldDisposeMcp2221A;

  internal Mcp2221AI2cBus(
    I2cController i2cBus,
    bool shouldDisposeMcp2221A
  )
  {
    this.i2cBus = i2cBus ?? throw new ArgumentNullException(nameof(i2cBus));
    this.shouldDisposeMcp2221A = shouldDisposeMcp2221A;
  }

  /// <inheritdoc/>
  public override Mcp2221AI2cDevice CreateDevice(int deviceAddress)
    => CreateDevice(new I2cAddress(deviceAddress));

  /// <param name="deviceAddress">The I2C address of the target device.</param>
  /// <param name="transmissionSpeedInKbps">
  /// The transmission speed used for reading and writing to the I2C bus in [kbps] units.
  /// </param>
  /// <remarks>
  ///   <include
  ///     file="../Smdn.Devices.Mcp2221A.docs.xml"
  ///     path="docs/I2cReadWriteTransmissionSpeedParameter/remarks/*"
  ///   />
  /// </remarks>
  public Mcp2221AI2cDevice CreateDevice(
    I2cAddress deviceAddress,
    int transmissionSpeedInKbps = I2cController.DefaultTransmissionSpeedInKbps
  )
    => I2cBus.CreateI2cDeviceAdapter(
      deviceAddress: deviceAddress,
      transmissionSpeedInKbps: transmissionSpeedInKbps,
      // The lifecycle of the MCP2221 is managed by this class, so regardless of the
      // value specified for shouldDisposeMcp2221A, the created device adapter
      // is not allowed to dispose of the MCP2221.
      shouldDisposeMcp2221A: false
    );

  /// <inheritdoc/>
  /// <remarks>
  /// The current implementation of this method does nothing.
  /// After calling this method, the created <see cref="I2cDevice"/> remains
  /// available for use.
  /// </remarks>
  public override void RemoveDevice(int deviceAddress)
    => RemoveDevice(new I2cAddress(deviceAddress));

  /// <remarks>
  /// The current implementation of this method does nothing.
  /// After calling this method, the created <see cref="I2cDevice"/> remains
  /// available for use.
  /// </remarks>
#pragma warning disable IDE0060, CA1822
  public void RemoveDevice(I2cAddress deviceAddress)
#pragma warning restore IDE0060, CA1822
  {
    // do nothing in this class
  }

  /// <inheritdoc/>
  protected override void Dispose(bool disposing)
  {
    if (disposing) {
      if (shouldDisposeMcp2221A)
        i2cBus?.Device?.Dispose();

      i2cBus = null!;
    }

    base.Dispose(disposing);
  }
}
