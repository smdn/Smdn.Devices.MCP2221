// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
namespace Smdn.Devices.UsbHid.Logging;

/// <summary>
/// Defines the event IDs commonly used for logging with
/// <see cref="Microsoft.Extensions.Logging.ILogger"/> in USB-HID device operations.
/// </summary>
public static class EventIds {
  public const int UsbHidOpenEndPoint = 10;
  public const int UsbHidGetDeviceInfo = 20;
}
