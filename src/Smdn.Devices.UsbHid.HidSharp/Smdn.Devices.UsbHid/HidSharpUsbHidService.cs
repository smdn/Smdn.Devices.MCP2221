// SPDX-FileCopyrightText: 2026 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using HidSharp;

using Microsoft.Extensions.Logging;

using Polly.Registry;

namespace Smdn.Devices.UsbHid;

/// <summary>
/// An implementation of <see cref="IUsbHidService"/> that uses HidSharp as the backend.
/// </summary>
internal sealed class HidSharpUsbHidService(
  ResiliencePipelineProvider<string>? resiliencePipelineProvider,
  ILoggerFactory? loggerFactory
) : IUsbHidService {
  private bool disposed = false;

  /// <inheritdoc/>
  public IReadOnlyList<IUsbHidDevice> GetDevices(
    CancellationToken cancellationToken = default
  )
  {
    ThrowIfDisposed();

    cancellationToken.ThrowIfCancellationRequested();

    return DeviceList
      .Local
      .GetHidDevices()
      .Select(
        d => new HidSharpUsbHidDevice(
          device: d,
          resiliencePipelineProvider: resiliencePipelineProvider,
          logger: loggerFactory?.CreateLogger<HidSharpUsbHidDevice>()
        )
      )
      .ToList();
  }

  private void ThrowIfDisposed()
  {
    if (disposed)
      throw new ObjectDisposedException(GetType().FullName);
  }

  /// <inheritdoc/>
  public void Dispose()
  {
    // nothing to do
    disposed = true;
  }

  /// <inheritdoc/>
  public ValueTask DisposeAsync()
  {
    // nothing to do
    disposed = true;

    return default;
  }

  public override string? ToString()
    => GetType().Assembly.GetName().Name;
}
