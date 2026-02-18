// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;

using Microsoft.Extensions.Logging;

namespace Smdn.Devices.UsbHid.Logging;

public static class EventIds {
  [CLSCompliant(false)]
  public static EventId UsbHidListDevice { get; } = new(10, "USB HID List device");

  [CLSCompliant(false)]
  public static EventId UsbHidSelectDevice { get; } = new(11, "USB HID Select device");

  [CLSCompliant(false)]
  public static EventId UsbHidOpenEndPoint { get; } = new(12, "USB HID Open end point");
}
