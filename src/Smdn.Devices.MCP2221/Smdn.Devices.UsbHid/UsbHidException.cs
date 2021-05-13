// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT

using System;

using Smdn.Devices.UsbHid;

namespace Smdn.Devices.UsbHid {
  public class UsbHidException : InvalidOperationException {
    public UsbHidException() : base() { }
    public UsbHidException(string message) : base(message) { }
  }
}
