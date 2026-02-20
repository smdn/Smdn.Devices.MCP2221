// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
#if NULL_STATE_STATIC_ANALYSIS_ATTRIBUTES
using System.Diagnostics.CodeAnalysis;
#endif
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using LibUsbDotNet;
using LibUsbDotNet.Info;
using LibUsbDotNet.LibUsb;
using LibUsbDotNet.Main;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

using Polly;
using Polly.Registry;

using Smdn.Devices.UsbHid.Logging;

namespace Smdn.Devices.UsbHid;

/// <summary>
/// An implementation of <see cref="IUsbHidDevice"/> that uses
/// <see cref="UsbDevice"/> of LibUsbDotNet as the backend.
/// </summary>
public sealed partial class LibUsbDotNetUsbHidDevice : IUsbHidDevice<UsbDevice> {
  /// <summary>
  /// Gets the key to retrieve the <see cref="ResiliencePipeline"/> for the
  /// <see cref="OpenEndPointAsync(bool, bool, bool, CancellationToken)"/> method
  /// from the <see cref="ResiliencePipelineProvider{TKey}"/>.
  /// </summary>
  public static string ResiliencePipelineKeyForOpenEndPoint { get; } = nameof(LibUsbDotNetUsbHidDevice) + "." + nameof(resiliencePipelineOpenEndPoint);

  private readonly LibUsbDotNetUsbHidService service;

  private UsbDevice? deviceImplementation;

  /// <inheritdoc/>
  [CLSCompliant(false)]
  public UsbDevice DeviceImplementation => deviceImplementation ?? throw new ObjectDisposedException(GetType().FullName);

  /// <inheritdoc/>
  public int VendorId => DeviceImplementation.VendorId;

  /// <inheritdoc/>
  public int ProductId => DeviceImplementation.ProductId;

  private readonly ResiliencePipeline resiliencePipelineOpenEndPoint;
  private readonly ILogger logger;

  internal LibUsbDotNetUsbHidDevice(
    LibUsbDotNetUsbHidService service,
    UsbDevice device,
    ResiliencePipelineProvider<string>? resiliencePipelineProvider,
    ILogger? logger
  )
  {
    this.service = service ?? throw new ArgumentNullException(nameof(service));
    deviceImplementation = device ?? throw new ArgumentNullException(nameof(device));
    this.logger = logger ?? NullLogger.Instance;

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

  /// <inheritdoc/>
  /// <remarks>
  /// If the device is not open, this method attempts to open it.
  /// </remarks>
  public bool TryGetProductName(
#if NULL_STATE_STATIC_ANALYSIS_ATTRIBUTES
    [NotNullWhen(true)]
#endif
    out string? productName
  )
  {
    productName = default;

    if (!DeviceImplementation.IsOpen && !DeviceImplementation.TryOpen())
      return false;

    return (productName = DeviceImplementation.Descriptor.Product) is not null;
  }

  /// <inheritdoc/>
  /// <remarks>
  /// If the device is not open, this method attempts to open it.
  /// </remarks>
  public bool TryGetManufacturer(
#if NULL_STATE_STATIC_ANALYSIS_ATTRIBUTES
    [NotNullWhen(true)]
#endif
    out string? manufacturer
  )
  {
    manufacturer = default;

    if (!DeviceImplementation.IsOpen && !DeviceImplementation.TryOpen())
      return false;

    return (manufacturer = DeviceImplementation.Descriptor.Manufacturer) is not null;
  }

  /// <inheritdoc/>
  /// <remarks>
  /// If the device is not open, this method attempts to open it.
  /// </remarks>
  public bool TryGetSerialNumber(
#if NULL_STATE_STATIC_ANALYSIS_ATTRIBUTES
    [NotNullWhen(true)]
#endif
    out string? serialNumber
  )
  {
    serialNumber = default;

    if (!DeviceImplementation.IsOpen && !DeviceImplementation.TryOpen())
      return false;

    return (serialNumber = DeviceImplementation.Descriptor.SerialNumber) is not null;
  }

  /// <inheritdoc/>
  public bool TryGetDeviceIdentifier(
#if NULL_STATE_STATIC_ANALYSIS_ATTRIBUTES
    [NotNullWhen(true)]
#endif
    out string? deviceIdentifier
  )
    => (deviceIdentifier = DeviceImplementation.LocationId.ToString()) is not null;

  /// <inheritdoc/>
  public void Dispose()
  {
    deviceImplementation?.Dispose();
    deviceImplementation = null;
  }

  /// <inheritdoc/>
  /// <remarks>
  /// This implementation performs a synchronous disposal, as the
  /// underlying <see cref="UsbDevice"/> does not support asynchronous disposal.
  /// </remarks>
  public ValueTask DisposeAsync()
  {
    // UsbDevice does not implement IAsyncDisposable
    deviceImplementation?.Dispose();
    deviceImplementation = null;

    return default;
  }

  private LibUsbDotNetUsbHidEndPoint OpenEndPointCore(
    bool openOutEndPoint,
    bool openInEndPoint,
    bool shouldDisposeDevice,
    CancellationToken cancellationToken
  )
  {
    if (!openOutEndPoint && !openInEndPoint)
      throw new InvalidOperationException("At least one of the IN or OUT endpoints must be opened.");

    UsbConfigInfo? config = null;
    UsbInterfaceInfo? hidInterface = null;
    UsbEndpointInfo? outEndpoint = null;
    UsbEndpointInfo? inEndpoint = null;

    const byte EndpointAddressInOutBitMask  = 0b_1000_0000;
    const byte EndpointAddressIn            = 0b_1000_0000;
    const byte EndpointAddressOut           = 0b_0000_0000;
    // const byte EndpointAddressNumberMask    = 0b_0000_0111;
    const byte AttributesTransferTypeMask   = 0b_0000_0011;

    foreach (var cfg in DeviceImplementation.Configs) {
      foreach (var iface in cfg.Interfaces) {
        if (iface.Class == ClassCode.Hid) {
          config = cfg;
          hidInterface = iface;
          outEndpoint = iface.Endpoints.FirstOrDefault(ep => (ep.EndpointAddress & EndpointAddressInOutBitMask) == EndpointAddressOut);
          inEndpoint = iface.Endpoints.FirstOrDefault(ep => (ep.EndpointAddress & EndpointAddressInOutBitMask) == EndpointAddressIn);

          break;
        }
      }

      if (hidInterface is not null)
        break;
    }

    if (config is null)
      throw new UsbHidException("USB configuration not found");
    if (hidInterface is null)
      throw new UsbHidException("HID interface not found");
    if (openOutEndPoint && outEndpoint is null)
      throw new UsbHidException("HID OUT endpoint not found");
    if (openInEndPoint && inEndpoint is null)
      throw new UsbHidException("HID IN endpoint not found");

    LogUsbHidOpenEndPointAttemptToOpen(hidInterface.Number, hidInterface.Interface);

    cancellationToken.ThrowIfCancellationRequested();

    if (!DeviceImplementation.IsOpen)
      DeviceImplementation.Open();

    // try set configuration
    foreach (var cfg in new[] { config.ConfigurationValue, 0 /* fallback */ }) {
      try {
        DeviceImplementation.SetConfiguration(cfg);
      }
      catch (UsbException ex) when (ex.ErrorCode == Error.Busy) {
        // [LibUsbDotNet 3.0.87-alpha] SetConfiguration always throw UsbException with Error.Busy
        LogUsbHidOpenEndPointSetConfigurationBusy(ex, hidInterface.Number, hidInterface.Interface, cfg);
        continue; // expected, continue
      }
      catch (Exception ex) {
        LogUsbHidOpenEndPointSetConfigurationFailed(ex, hidInterface.Number, hidInterface.Interface, cfg);
        throw; // unexpected
      }
    }

    // try claim HID interface
    try {
      DeviceImplementation.ClaimInterface(hidInterface.Number);
    }
    catch (Exception ex) {
      LogUsbHidOpenEndPointClaimInterfaceFailed(ex, hidInterface.Number, hidInterface.Interface);
      throw;
    }

    // open HID endpoint
    return new(
      device: this,
      endPointWriter: openOutEndPoint
        ? DeviceImplementation.OpenEndpointWriter(
            writeEndpointID: (WriteEndpointID)outEndpoint!.EndpointAddress,
            endpointType: (EndpointType)(outEndpoint.Attributes & AttributesTransferTypeMask)
          )
        : null,
      maxOutEndPointPacketSize: openInEndPoint
        ? outEndpoint!.MaxPacketSize
        : 0,
      writeEndPointTimeout: service.Options.WriteEndPointTimeout,
      endPointReader: openInEndPoint
        ? DeviceImplementation.OpenEndpointReader(
            readBufferSize: service.Options.ReadEndPointBufferSize,
            readEndpointID: (ReadEndpointID)inEndpoint!.EndpointAddress,
            endpointType: (EndpointType)(inEndpoint.Attributes & AttributesTransferTypeMask)
          )
        : null,
      maxInEndPointPacketSize: openInEndPoint
        ? inEndpoint!.MaxPacketSize
        : 0,
      readEndPointTimeout: service.Options.ReadEndPointTimeout,
      shouldDisposeDevice: shouldDisposeDevice
    );
  }

  [LoggerMessage(
    EventId = EventIds.UsbHidOpenEndPoint,
    Level = Microsoft.Extensions.Logging.LogLevel.Debug,
    Message = "Attempt to open endpoint (HID interface #{Number}, {Iface})"
  )]
  private partial void LogUsbHidOpenEndPointAttemptToOpen(int number, string iface);

  [LoggerMessage(
    EventId = EventIds.UsbHidOpenEndPoint + 1,
    Level = Microsoft.Extensions.Logging.LogLevel.Warning,
    Message = "Attempt to open endpoint (HID interface #{Number}, {Iface}): Configuration #{Configuration} resource busy."
  )]
  private partial void LogUsbHidOpenEndPointSetConfigurationBusy(Exception ex, int number, string iface, int configuration);

  [LoggerMessage(
    EventId = EventIds.UsbHidOpenEndPoint + 2,
    Level = Microsoft.Extensions.Logging.LogLevel.Critical,
    Message = "Attempt to open endpoint (HID interface #{Number}, {Iface}): Set configuration #{Configuration} failed."
  )]
  private partial void LogUsbHidOpenEndPointSetConfigurationFailed(Exception ex, int number, string iface, int configuration);

  [LoggerMessage(
    EventId = EventIds.UsbHidOpenEndPoint + 3,
    Level = Microsoft.Extensions.Logging.LogLevel.Critical,
    Message = "Open endpoint failed (HID interface #{Number}, {Iface}): Claim interface #{Number} failed."
  )]
  private partial void LogUsbHidOpenEndPointClaimInterfaceFailed(Exception ex, int number, string iface);

  /// <inheritdoc/>
  public IUsbHidEndPoint OpenEndPoint(
    bool openOutEndPoint,
    bool openInEndPoint,
    bool shouldDisposeDevice,
    CancellationToken cancellationToken
  )
  {
    ThrowIfDisposed();

    cancellationToken.ThrowIfCancellationRequested();

    var resilienceContext = ResilienceContextPool.Shared.Get(cancellationToken);

    try {
      return resiliencePipelineOpenEndPoint.Execute(
        callback: ctx => OpenEndPointCore(
          openOutEndPoint: openOutEndPoint,
          openInEndPoint: openInEndPoint,
          shouldDisposeDevice: shouldDisposeDevice,
          cancellationToken: cancellationToken
        ),
        context: resilienceContext
      );
    }
    finally {
      ResilienceContextPool.Shared.Return(resilienceContext);
    }
  }

  /// <inheritdoc/>
  /// <remarks>
  /// This implementation performs a synchronous open, as the underlying
  /// LibUsbDotNet APIs do not support asynchronous operations for opening endpoints.
  /// </remarks>
  public ValueTask<IUsbHidEndPoint> OpenEndPointAsync(
    bool openOutEndPoint,
    bool openInEndPoint,
    bool shouldDisposeDevice,
    CancellationToken cancellationToken
  )
  {
    ThrowIfDisposed();

    cancellationToken.ThrowIfCancellationRequested();

    return OpenEndPointAsyncCore();

    async ValueTask<IUsbHidEndPoint> OpenEndPointAsyncCore()
    {
      var resilienceContext = ResilienceContextPool.Shared.Get(cancellationToken);

      try {
        return await resiliencePipelineOpenEndPoint.ExecuteAsync(
          callback: ctx =>
#pragma warning disable SA1114
#if SYSTEM_THREADING_TASKS_VALUETASK_FROMRESULT
            ValueTask.FromResult<IUsbHidEndPoint>(
#else
            new ValueTask<IUsbHidEndPoint>(
#endif
#pragma warning disable CA2000
              result: OpenEndPointCore(
                openOutEndPoint: openOutEndPoint,
                openInEndPoint: openInEndPoint,
                shouldDisposeDevice: shouldDisposeDevice,
                cancellationToken: cancellationToken
              )
#pragma warning restore CA2000
            ),
#pragma warning restore SA1114
          context: resilienceContext
        ).ConfigureAwait(false);
      }
      finally {
        ResilienceContextPool.Shared.Return(resilienceContext);
      }
    }
  }

  /// <inheritdoc/>
  public override string? ToString()
    => $"{GetType().FullName} (DeviceImplementation='{DeviceImplementation}')";
}
