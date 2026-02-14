// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;

using Microsoft.Extensions.Logging;

namespace Smdn.Devices.UsbHid.Logging;

public static class EventIds {
  [CLSCompliant(false)]
  public static EventId UsbHidOpenEndPoint { get; } = new(10, "USB HID Open end point");

  [CLSCompliant(false)]
  public static EventId UsbHidGetDeviceInfo { get; } = new(11, "USB HID Get device information");
}
