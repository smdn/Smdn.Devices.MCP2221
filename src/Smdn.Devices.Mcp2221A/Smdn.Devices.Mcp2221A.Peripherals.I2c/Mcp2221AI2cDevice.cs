// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT

using System;
using System.Device.I2c;

namespace Smdn.Devices.Mcp2221A.Peripherals.I2c;

[CLSCompliant(false)]
public sealed class Mcp2221AI2cDevice : I2cDevice {
  private I2cController i2cBus;
  internal I2cController I2cBus => i2cBus ?? throw new ObjectDisposedException(GetType().FullName);

  /// <summary>
  /// Gets or sets the transmission speed used for reading and writing to the I2C bus in [kbps] units.
  /// </summary>
  /// <remarks>
  ///   <para>
  ///     Setting this property to <c>0</c> or a negative value will throw
  ///     an <see cref="ArgumentOutOfRangeException"/>. For any other value,
  ///     even if the transmission speed is unsupported by the device,
  ///     this property will not throw an exception. However, an <see cref="ArgumentOutOfRangeException"/>
  ///     will be thrown when the actual read or write operation is performed.
  ///   </para>
  ///   <include
  ///     file="../Smdn.Devices.Mcp2221A.docs.xml"
  ///     path="docs/I2cReadWriteTransmissionSpeedParameter/remarks/*"
  ///   />
  /// </remarks>
#pragma warning disable SA1513
  public int TransmissionSpeedInKbps {
    get;
    set {
      if (value <= 0) {
        throw new ArgumentOutOfRangeException(
          paramName: nameof(TransmissionSpeedInKbps),
          actualValue: value,
          message: "Must be non-zero positive value."
        );
      }

      field = value;
    }
  } = I2cController.DefaultTransmissionSpeedInKbps;
#pragma warning restore SA1513

  private readonly I2cAddress deviceAddress;
  private readonly bool shouldDisposeMcp2221A;

  public override I2cConnectionSettings ConnectionSettings { get; }

  public Mcp2221AI2cDevice(
    I2cController i2cBus,
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
    => I2cBus.Read(deviceAddress, TransmissionSpeedInKbps, buffer);

  /// <inheritdoc/>
  public override void WriteByte(byte value)
    => Write([value]);

  /// <inheritdoc/>
  public override void Write(ReadOnlySpan<byte> buffer)
    => I2cBus.Write(deviceAddress, TransmissionSpeedInKbps, buffer);

  /// <inheritdoc/>
  public override void WriteRead(ReadOnlySpan<byte> writeBuffer, Span<byte> readBuffer)
  {
    Write(writeBuffer);
    Read(readBuffer);
  }

  /// <summary>
  /// Sets the I2C bus speed to Standard Mode (100 kbps).
  /// </summary>
  /// <returns>The current <see cref="Mcp2221AI2cDevice"/> instance for fluent chaining.</returns>
  public Mcp2221AI2cDevice WithStandardMode()
  {
    TransmissionSpeedInKbps = 100;
    return this;
  }

  /// <summary>
  /// Sets the I2C bus speed to Fast Mode (400 kbps).
  /// </summary>
  /// <returns>The current <see cref="Mcp2221AI2cDevice"/> instance for fluent chaining.</returns>
  public Mcp2221AI2cDevice WithFastMode()
  {
    TransmissionSpeedInKbps = 400;
    return this;
  }
}
