// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT

using System;
using System.Threading.Tasks;

namespace Smdn.Devices.UsbHid;

public interface IUsbHidDevice :
  IDisposable,
  IAsyncDisposable
{
  string ProductName { get; }
  string Manufacturer { get; }
  int VendorID { get; }
  int ProductID { get; }
  string SerialNumber { get; }
  Version ReleaseNumber { get; }
  string DevicePath { get; }
  string FileSystemName { get; }

  ValueTask<IUsbHidStream> OpenStreamAsync();
  IUsbHidStream OpenStream();
}
