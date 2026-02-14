// SPDX-FileCopyrightText: 2026 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;

using Microsoft.Extensions.Logging;

namespace Smdn.Devices.UsbHid;

/// <summary>
/// A class that represents the options for the Route B session configurations.
/// </summary>
// To enable the addition of options in future versions, exposes as a class rather than a struct.
public sealed class LibUsbDotNetOptions {
  [CLSCompliant(false)]
  public LogLevel DebugLevel { get; set; }

#pragma warning disable IDE0360 // Property accessor can be simplified
#pragma warning disable SA1513 // Closing brace should be followed by blank line
  public int ReadEndPointBufferSize {
    get => field;
    set => field = value; // TODO: validation
  } = 0x100;

  public TimeSpan ReadEndPointTimeout {
    get => field;
    set => field = value; // TODO: validation
  } = TimeSpan.FromSeconds(10);

  public TimeSpan WriteEndPointTimeout {
    get => field;
    set => field = value; // TODO: validation
  } = TimeSpan.FromSeconds(10);
#pragma warning restore SA1513
#pragma warning restore IDE0360

  internal LibUsbDotNet.LogLevel LibUsbDotNetDebugLevel => DebugLevel switch {
    LogLevel.Trace or LogLevel.Debug => LibUsbDotNet.LogLevel.Debug,
    LogLevel.Information => LibUsbDotNet.LogLevel.Info,
    LogLevel.Warning => LibUsbDotNet.LogLevel.Warning,
    LogLevel.Error => LibUsbDotNet.LogLevel.Error,
    LogLevel.Critical => LibUsbDotNet.LogLevel.Error,
    LogLevel.None or _ => LibUsbDotNet.LogLevel.None,
  };

  /// <summary>
  /// Configure this instance to have the same values as the instance passed as an argument.
  /// </summary>
  /// <param name="baseOptions">
  /// A <see cref="LibUsbDotNetOptions"/> that holds the values that are used to configure this instance.
  /// </param>
  /// <exception cref="ArgumentNullException">
  /// <paramref name="baseOptions"/> is <see langword="null"/>.
  /// </exception>
  /// <returns>
  /// The current <see cref="LibUsbDotNetOptions"/> so that additional calls can be chained.
  /// </returns>
  public LibUsbDotNetOptions Configure(LibUsbDotNetOptions baseOptions)
  {
    if (baseOptions is null)
      throw new ArgumentNullException(nameof(baseOptions));

    DebugLevel = baseOptions.DebugLevel;

    return this;
  }
}
