// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT

#pragma warning disable CA1848

#if USBHIDDRIVER_HIDSHARP

using System;
using System.Threading;
using System.Threading.Tasks;

using HidSharp;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using HidDevice = HidSharp.HidDevice;

namespace Smdn.Devices.UsbHid.HidSharp;

internal class Device : IUsbHidDevice {
  /*
   * static members
   */
  public static Device Find(Predicate<Device> predicate, IServiceProvider serviceProvider = null)
  {
    if (predicate is null)
      throw new ArgumentNullException(nameof(predicate));

    var logger = serviceProvider?.GetService<ILoggerFactory>()?.CreateLogger<Device>();

    foreach (var hidDevice in DeviceList.Local.GetHidDevices()) {
      var d = new Device(hidDevice, logger);

      logger?.LogDebug(EventIds.UsbHidListDevice, $"found: VID={d.VendorID:x4} PID={d.ProductID:x4} ({d.Manufacturer}, {d.ProductName}, {d.SerialNumber ?? "[no serial number]"}) {d.FileSystemName} {d.DevicePath}");

      if (predicate(d)) {
        logger?.LogInformation(EventIds.UsbHidSelectDevice, $"selected VID={d.VendorID:x4} PID={d.ProductID:x4} ({d.Manufacturer}, {d.ProductName}, {d.SerialNumber ?? "[no serial number]"}) {d.FileSystemName} {d.DevicePath}");

        return d;
      }

      d.Dispose();
    }

    return null;
  }

  /*
   * instance members
   */
  private HidDevice hidDevice;
  private HidDevice HidDevice => hidDevice ?? throw new ObjectDisposedException(GetType().Name);

  private readonly ILogger logger;

  public string ProductName => HidDevice.GetProductName();
  public string Manufacturer => HidDevice.GetManufacturer();
  public int VendorID => HidDevice.VendorID;
  public int ProductID => HidDevice.ProductID;
  public string SerialNumber {
    get {
      try {
        return HidDevice.GetSerialNumber();
      }

      // HidSharp.Exceptions.DeviceIOException is internal class
      catch (Exception ex) when (ex.GetType().FullName.Equals("HidSharp.Exceptions.DeviceIOException", StringComparison.Ordinal)) {
        return null;
      }
    }
  }

  public Version ReleaseNumber => HidDevice.ReleaseNumber;
  public string DevicePath => HidDevice.DevicePath;
  public string FileSystemName => HidDevice.GetFileSystemName();

  internal Device(HidDevice hidDevice, ILogger logger)
  {
    this.hidDevice = hidDevice;
    this.logger = logger;
  }

#pragma warning disable SA1203
  private const int MaxRetryOpen = 5;
  private const int RetryOpenIntervalMilliseconds = 200;
#pragma warning restore SA1203

  public async ValueTask<IUsbHidStream> OpenStreamAsync()
  {
    for (var n = 1; ; n++) {
      try {
        return new Stream(HidDevice.Open());
      }
      catch (Exception ex) when (n < MaxRetryOpen) {
        logger?.LogWarning(EventIds.UsbHidOpenEndPoint, ex, $"retry open ({n}/{MaxRetryOpen})");

        await Task.Delay(RetryOpenIntervalMilliseconds).ConfigureAwait(false); // then continue
      }
    }
  }

  public IUsbHidStream OpenStream()
  {
    for (var n = 1; ; n++) {
      try {
        return new Stream(HidDevice.Open());
      }
      catch (Exception ex) when (n < MaxRetryOpen) {
        logger?.LogWarning(EventIds.UsbHidOpenEndPoint, ex, $"retry open ({n}/{MaxRetryOpen})");

        Thread.Sleep(RetryOpenIntervalMilliseconds);
      }
    }
  }

  public void Dispose()
  {
    // HidDevice does not implement IDisposable
    // _hidDevice?.Dispose();
    hidDevice = null;
  }

  public ValueTask DisposeAsync()
  {
    hidDevice = null;

#if NET5_0_OR_GREATER
    return ValueTask.CompletedTask;
#else
    return new ValueTask(Task.CompletedTask);
#endif
  }
}

#endif // USBHIDDRIVER_HIDSHARP
