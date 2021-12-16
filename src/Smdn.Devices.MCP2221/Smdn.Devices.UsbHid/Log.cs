// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT

using System;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

namespace Smdn.Devices.UsbHid;

public static class Log {
  /// <summary>log level for native library</summary>
  public static LogLevel NativeLibraryLogLevel { get; set; } = LogLevel.None;

#if USBHIDDRIVER_LIBUSBDOTNET
  internal static global::LibUsbDotNet.LogLevel LibUsbDotNetLogLevel => NativeLibraryLogLevel switch {
    LogLevel.Trace or LogLevel.Debug  => global::LibUsbDotNet.LogLevel.Debug,
    LogLevel.Information              => global::LibUsbDotNet.LogLevel.Info,
    LogLevel.Warning                  => global::LibUsbDotNet.LogLevel.Warning,
    LogLevel.Error                    => global::LibUsbDotNet.LogLevel.Error,
    LogLevel.Critical                 => global::LibUsbDotNet.LogLevel.Error,
    LogLevel.None or _                => global::LibUsbDotNet.LogLevel.None,
  };
#endif
}
