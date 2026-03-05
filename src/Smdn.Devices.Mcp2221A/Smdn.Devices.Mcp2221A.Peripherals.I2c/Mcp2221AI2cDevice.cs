// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT

using System;
using System.Device.I2c;

namespace Smdn.Devices.Mcp2221A.Peripherals.I2c;

internal sealed class Mcp2221AI2cDevice : I2cDevice {
  private Mcp2221A.I2cFunctionality i2cBus;
  internal Mcp2221A.I2cFunctionality I2cBus => i2cBus ?? throw new ObjectDisposedException(GetType().FullName);

  private readonly I2cAddress deviceAddress;
  private readonly bool shouldDisposeMcp2221A;

  public override I2cConnectionSettings ConnectionSettings { get; }

  public Mcp2221AI2cDevice(
    Mcp2221A.I2cFunctionality i2cBus,
    I2cAddress deviceAddress,
    bool shouldDisposeMcp2221A
  )
  {
    this.i2cBus = i2cBus ?? throw new ArgumentNullException(nameof(i2cBus));
    this.deviceAddress = deviceAddress;
    this.shouldDisposeMcp2221A = shouldDisposeMcp2221A;

    ConnectionSettings = new(busId: 0, deviceAddress: (int)deviceAddress);
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

  /// <inheritdoc/>
  public override byte ReadByte()
  {
    Span<byte> buffer = stackalloc byte[1];

    Read(buffer);

    return buffer[0];
  }

  /// <inheritdoc/>
  public override void Read(Span<byte> buffer)
    => I2cBus.Read(deviceAddress, buffer);

  /// <inheritdoc/>
  public override void WriteByte(byte value)
    => Write([value]);

  /// <inheritdoc/>
  public override void Write(ReadOnlySpan<byte> buffer)
    => I2cBus.Write(deviceAddress, buffer);

  /// <inheritdoc/>
  public override void WriteRead(ReadOnlySpan<byte> writeBuffer, Span<byte> readBuffer)
  {
    Write(writeBuffer);
    Read(readBuffer);
  }
}
