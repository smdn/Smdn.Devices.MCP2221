// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT

namespace Smdn.Devices.MCP2221;

public readonly struct I2CScanBusProgress {
  public I2CAddress ScanningAddress { get; }
  public I2CAddress AddressRangeMin { get; }
  public I2CAddress AddressRangeMax { get; }
  public int ProgressInPercent => 100 * ((int)ScanningAddress - (int)AddressRangeMin) / ((int)AddressRangeMax - (int)AddressRangeMin);

  internal I2CScanBusProgress(
    I2CAddress scanningAddress,
    I2CAddress addressRangeMin,
    I2CAddress addressRangeMax
  )
  {
    this.ScanningAddress = scanningAddress;
    this.AddressRangeMin = addressRangeMin;
    this.AddressRangeMax = addressRangeMax;
  }
}
