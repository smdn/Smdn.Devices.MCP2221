// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT

using Microsoft.Extensions.Logging;

namespace Smdn.Devices.UsbHid;

internal static class EventIds {
  internal static readonly EventId UsbHidListDevice = new(10, "USB HID List device");
  internal static readonly EventId UsbHidSelectDevice = new(11, "USB HID Select device");
  internal static readonly EventId UsbHidOpenEndPoint = new(12, "USB HID Open end point");
}
