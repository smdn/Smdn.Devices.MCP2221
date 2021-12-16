// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT

using System;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

namespace Smdn.Devices.UsbHid;

internal static class EventIds {
  internal static readonly EventId UsbHidListDevice = new EventId(10, "USB HID List device");
  internal static readonly EventId UsbHidSelectDevice = new EventId(11, "USB HID Select device");
  internal static readonly EventId UsbHidOpenEndPoint = new EventId(12, "USB HID Open end point");
}
