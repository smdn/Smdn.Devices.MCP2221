// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;

using Microsoft.Extensions.Logging;

namespace Smdn.Devices.Mcp2221A.Peripherals.I2c;

public sealed partial class I2cController {
  internal static readonly EventId EventIdI2cCommand = new(10, "I2C command");
  internal static readonly EventId EventIdI2cEngineState = new(11, "I2C engine state");

  internal Mcp2221A Device { get; }

  private readonly ILogger? logger;

  internal I2cController(Mcp2221A device, ILogger? logger)
  {
    Device = device;
    this.logger = logger;
  }

  /// <summary>
  /// Creates a new <see cref="System.Device.I2c.I2cBus"/> adapter.
  /// This adapter allows the MCP2221/MCP2221A to interface with the extensive collection of I2C device
  /// bindings provided by the <see href="https://www.nuget.org/packages/Iot.Device.Bindings">Iot.Device.Bindings</see>
  /// library.
  /// </summary>
  /// <param name="shouldDisposeMcp2221A">
  /// <see langword="true"/> to automatically dispose the underlying <see cref="Mcp2221A"/>
  /// when this adapter is disposed; <see langword="false"/> to keep the <see cref="Mcp2221A"/> open.
  /// The default is <see langword="false"/>.
  /// </param>
  /// <returns>
  /// A <see cref="Mcp2221AI2cBus"/> instance that wraps the MCP2221 I2C functionality.
  /// </returns>
  /// <exception cref="ObjectDisposedException">
  /// Thrown if the parent <see cref="Mcp2221A"/> has already been disposed.
  /// </exception>
  [CLSCompliant(false)]
  public Mcp2221AI2cBus CreateI2cBusAdapter(
    bool shouldDisposeMcp2221A = false
  )
  {
    Device.ThrowIfDisposed();

    return new(
      i2cBus: this,
      shouldDisposeMcp2221A: shouldDisposeMcp2221A
    );
  }

  /// <summary>
  /// Creates a new <see cref="System.Device.I2c.I2cDevice"/> adapter for a specific I2C address.
  /// This adapter allows the MCP2221/MCP2221A to interface with the extensive collection of I2C device
  /// bindings provided by the <see href="https://www.nuget.org/packages/Iot.Device.Bindings">Iot.Device.Bindings</see>
  /// library.
  /// </summary>
  /// <param name="deviceAddress">The I2C address of the target device.</param>
  /// <param name="shouldDisposeMcp2221A">
  /// <see langword="true"/> to automatically dispose the underlying <see cref="Mcp2221A"/>
  /// when this adapter is disposed; <see langword="false"/> to keep the <see cref="Mcp2221A"/> open.
  /// The default is <see langword="false"/>.
  /// </param>
  /// <returns>
  /// A <see cref="Mcp2221AI2cDevice"/> instance that wraps the MCP2221 I2C functionality
  /// for the specified address.
  /// </returns>
  /// <exception cref="ObjectDisposedException">
  /// Thrown if the parent <see cref="Mcp2221A"/> has already been disposed.
  /// </exception>
  [CLSCompliant(false)]
  public Mcp2221AI2cDevice CreateI2cDeviceAdapter(
    I2cAddress deviceAddress,
    bool shouldDisposeMcp2221A = false
  )
    => CreateI2cDeviceAdapter(
      deviceAddress: deviceAddress,
      transmissionSpeedInKbps: DefaultTransmissionSpeedInKbps,
      shouldDisposeMcp2221A: shouldDisposeMcp2221A
    );

  /// <summary>
  /// Creates a new <see cref="System.Device.I2c.I2cDevice"/> adapter for a specific I2C address.
  /// This adapter allows the MCP2221/MCP2221A to interface with the extensive collection of I2C device
  /// bindings provided by the <see href="https://www.nuget.org/packages/Iot.Device.Bindings">Iot.Device.Bindings</see>
  /// library.
  /// </summary>
  /// <param name="deviceAddress">The I2C address of the target device.</param>
  /// <param name="transmissionSpeedInKbps">
  /// The transmission speed used for reading and writing to the I2C bus in [kbps] units.
  /// </param>
  /// <param name="shouldDisposeMcp2221A">
  /// <see langword="true"/> to automatically dispose the underlying <see cref="Mcp2221A"/>
  /// when this adapter is disposed; <see langword="false"/> to keep the <see cref="Mcp2221A"/> open.
  /// The default is <see langword="false"/>.
  /// </param>
  /// <returns>
  /// A <see cref="Mcp2221AI2cDevice"/> instance that wraps the MCP2221 I2C functionality
  /// for the specified address.
  /// </returns>
  /// <exception cref="ObjectDisposedException">
  /// Thrown if the parent <see cref="Mcp2221A"/> has already been disposed.
  /// </exception>
  /// <remarks>
  ///   <include
  ///     file="../Smdn.Devices.Mcp2221A.docs.xml"
  ///     path="docs/I2cReadWriteTransmissionSpeedParameter/remarks/*"
  ///   />
  /// </remarks>
  [CLSCompliant(false)]
  public Mcp2221AI2cDevice CreateI2cDeviceAdapter(
    I2cAddress deviceAddress,
    int transmissionSpeedInKbps,
    bool shouldDisposeMcp2221A = false
  )
  {
    Device.ThrowIfDisposed();

    return new(
      i2cBus: this,
      deviceAddress: deviceAddress,
      shouldDisposeMcp2221A: shouldDisposeMcp2221A
    ) {
      TransmissionSpeedInKbps = transmissionSpeedInKbps,
    };
  }
}
