// SPDX-FileCopyrightText: 2026 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT

using System;
using System.Collections.Generic;
using System.Threading;

namespace Smdn.Devices.UsbHid;

public interface IUsbHidService : IDisposable, IAsyncDisposable {
  IReadOnlyList<IUsbHidDevice> GetDevices(CancellationToken cancellationToken);
}
