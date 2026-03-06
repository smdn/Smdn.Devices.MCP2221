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

  /// <remarks>
  /// This property shares its value with <see cref="I2cController.BusSpeed"/>.
  /// Therefore, changing the value of this property affects <see cref="I2cController.BusSpeed"/>
  /// and other <see cref="Mcp2221AI2cBus"/> instances.
  /// </remarks>
  public I2cBusSpeed BusSpeed {
    get => I2cBus.BusSpeed;
    set => I2cBus.BusSpeed = value;
  }

  internal Mcp2221AI2cBus(
    I2cController i2cBus,
    bool shouldDisposeMcp2221A
  )
  {
    this.i2cBus = i2cBus ?? throw new ArgumentNullException(nameof(i2cBus));
    this.shouldDisposeMcp2221A = shouldDisposeMcp2221A;
  }

  /// <inheritdoc/>
  public override I2cDevice CreateDevice(int deviceAddress)
    => CreateDevice(new I2cAddress(deviceAddress));

  public I2cDevice CreateDevice(I2cAddress deviceAddress)
    => I2cBus.CreateI2cDeviceAdapter(
      deviceAddress: deviceAddress,
      // The lifecycle of the MCP2221 is managed by this class, so regardless of the
      // value specified for shouldDisposeMcp2221, the created device adapter
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
