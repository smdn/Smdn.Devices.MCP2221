// SPDX-FileCopyrightText: 2026 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
#pragma warning disable CA1848

using System;
#if NULL_STATE_STATIC_ANALYSIS_ATTRIBUTES
using System.Diagnostics.CodeAnalysis;
#endif
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using HidSharp;

using Microsoft.Extensions.Logging;

using Polly;

using Smdn.Devices.UsbHid.DependencyInjection;
using Smdn.Devices.UsbHid.Logging;

namespace Smdn.Devices.UsbHid;

internal sealed class UsbHidDevice : IUsbHidDevice<HidDevice> {
  public static readonly string ResiliencePipelineKeyForOpenEndPoint = nameof(UsbHidDevice) + "." + nameof(resiliencePipelineOpenEndPoint);

  private HidDevice? deviceImplementation;
  public HidDevice DeviceImplementation => deviceImplementation ?? throw new ObjectDisposedException(GetType().FullName);

  public int VendorId => DeviceImplementation.VendorID;
  public int ProductId => DeviceImplementation.ProductID;

  private readonly ILogger? logger;
  private readonly ResiliencePipeline resiliencePipelineOpenEndPoint;

  public UsbHidDevice(
    HidDevice device,
    ILogger? logger,
    IServiceProvider? serviceProvider,
    object? serviceKey
  )
  {
    deviceImplementation = device ?? throw new ArgumentNullException(nameof(device));
    this.logger = logger;

    var resiliencePipelineProvider = serviceProvider?.GetResiliencePipelineProviderForHidSharpUsbHidService(
      serviceKey: serviceKey
    );

    ResiliencePipeline? resiliencePipelineOpenEndPoint = null;

    _ = resiliencePipelineProvider?.TryGetPipeline(
      ResiliencePipelineKeyForOpenEndPoint,
      out resiliencePipelineOpenEndPoint
    );

    this.resiliencePipelineOpenEndPoint = resiliencePipelineOpenEndPoint ?? ResiliencePipeline.Empty;
  }

  private void ThrowIfDisposed()
  {
    if (deviceImplementation is null)
      throw new ObjectDisposedException(GetType().FullName);
  }

  public bool TryGetProductName(
#if NULL_STATE_STATIC_ANALYSIS_ATTRIBUTES
    [NotNullWhen(true)]
#endif
    out string? productName
  )
  {
    productName = default;

    try {
      productName = DeviceImplementation.GetProductName();
    }
    catch (IOException ex) {
      logger?.LogTrace(EventIds.UsbHidGetDeviceInfo, ex, "IOException occurred while getting product name.");

      return false;
    }
    catch (UnauthorizedAccessException ex) {
      logger?.LogTrace(EventIds.UsbHidGetDeviceInfo, ex, "UnauthorizedAccessException occurred while getting product name.");

      return false;
    }

    return productName is not null;
  }

  public bool TryGetManufacturer(
#if NULL_STATE_STATIC_ANALYSIS_ATTRIBUTES
    [NotNullWhen(true)]
#endif
    out string? manufacturer
  )
  {
    manufacturer = default;

    try {
      manufacturer = DeviceImplementation.GetManufacturer();
    }
    catch (IOException ex) {
      logger?.LogTrace(EventIds.UsbHidGetDeviceInfo, ex, "IOException occurred while getting manufacturer name.");

      return false;
    }
    catch (UnauthorizedAccessException ex) {
      logger?.LogTrace(EventIds.UsbHidGetDeviceInfo, ex, "UnauthorizedAccessException occurred while getting manufacturer name.");

      return false;
    }

    return manufacturer is not null;
  }

  public bool TryGetSerialNumber(
#if NULL_STATE_STATIC_ANALYSIS_ATTRIBUTES
    [NotNullWhen(true)]
#endif
    out string? serialNumber
  )
  {
    serialNumber = default;

    try {
      serialNumber = DeviceImplementation.GetSerialNumber();
    }
    catch (IOException ex) {
      logger?.LogTrace(EventIds.UsbHidGetDeviceInfo, ex, "IOException occurred while getting serial number.");

      return false;
    }
    catch (UnauthorizedAccessException ex) {
      logger?.LogTrace(EventIds.UsbHidGetDeviceInfo, ex, "UnauthorizedAccessException occurred while getting serial number.");

      return false;
    }

    return serialNumber is not null;
  }

  public bool TryGetDeviceIdentifier(
#if NULL_STATE_STATIC_ANALYSIS_ATTRIBUTES
    [NotNullWhen(true)]
#endif
    out string? deviceIdentifier
  )
    => (deviceIdentifier = DeviceImplementation.DevicePath) is not null;

  public void Dispose()
  {
    // HidDevice does not implement IDisposable
    deviceImplementation = null;
  }

  public ValueTask DisposeAsync()
  {
    // HidDevice does not implement IDisposable
    deviceImplementation = null;

    return default;
  }

  public IUsbHidEndPoint OpenEndPoint(
    bool shouldDisposeDevice,
    CancellationToken cancellationToken
  )
  {
    ThrowIfDisposed();

    var resilienceContext = ResilienceContextPool.Shared.Get(cancellationToken);

    try {
      return resiliencePipelineOpenEndPoint.Execute(
        callback: ctx => new UsbHidEndPoint(this, DeviceImplementation.Open(), shouldDisposeDevice),
        context: resilienceContext
      );
    }
    finally {
      ResilienceContextPool.Shared.Return(resilienceContext);
    }
  }

  public ValueTask<IUsbHidEndPoint> OpenEndPointAsync(
    bool shouldDisposeDevice,
    CancellationToken cancellationToken
  )
  {
    ThrowIfDisposed();

    return OpenEndPointCoreAsync();

    async ValueTask<IUsbHidEndPoint> OpenEndPointCoreAsync()
    {
      var resilienceContext = ResilienceContextPool.Shared.Get(cancellationToken);

      try {
        return await resiliencePipelineOpenEndPoint.ExecuteAsync(
          callback: ctx => {
            var endPoint = new UsbHidEndPoint(this, DeviceImplementation.Open(), shouldDisposeDevice);

            return
#if SYSTEM_THREADING_TASKS_VALUETASK_FROMRESULT
              ValueTask.FromResult(endPoint);
#else
              new ValueTask<IUsbHidEndPoint>(result: endPoint);
#endif
          },
          context: resilienceContext
        ).ConfigureAwait(false);
      }
      finally {
        ResilienceContextPool.Shared.Return(resilienceContext);
      }
    }
  }

  public override string? ToString()
    => $"{GetType().FullName} (DeviceImplementation='{DeviceImplementation}')";
}
