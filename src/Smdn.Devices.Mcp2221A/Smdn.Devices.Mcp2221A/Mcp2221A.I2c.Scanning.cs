// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using IReadOnlyI2cAddressSet =
#if SYSTEM_COLLECTIONS_GENERIC_IREADONLYSET
System.Collections.Generic.IReadOnlySet
#else
System.Collections.Generic.IReadOnlyCollection
#endif
<Smdn.Devices.Mcp2221A.I2cAddress>;

namespace Smdn.Devices.Mcp2221A;

#pragma warning disable IDE0040
partial class Mcp2221A {
  partial class I2cFunctionality {
#pragma warning restore IDE0040
    public async ValueTask<(IReadOnlyI2cAddressSet WriteAddressSet, IReadOnlyI2cAddressSet ReadAddressSet)> ScanBusAsync(
      I2cAddress addressRangeMin = default,
      I2cAddress addressRangeMax = default,
      IProgress<I2cScanBusProgress>? progress = null,
      CancellationToken cancellationToken = default
    )
    {
      if ((int)addressRangeMax < (int)addressRangeMin)
        throw new ArgumentException($"{nameof(addressRangeMax)}({addressRangeMax}) must be greater than or equals to {nameof(addressRangeMin)}({addressRangeMin})", nameof(addressRangeMax));

      if (addressRangeMin.Equals(I2cAddress.Zero))
        addressRangeMin = I2cAddress.DeviceMinValue;
      if (addressRangeMax.Equals(I2cAddress.Zero))
        addressRangeMax = I2cAddress.DeviceMaxValue;

      var writeAddressSet = new SortedSet<I2cAddress>();
      var readAddressSet = new SortedSet<I2cAddress>();

      for (var addr = (int)addressRangeMin; addr <= (int)addressRangeMax; addr++) {
        var address = new I2cAddress(addr);

        progress?.Report(new I2cScanBusProgress(address, addressRangeMin, addressRangeMax));

        try {
          await WriteAsync(address, default, cancellationToken).ConfigureAwait(false);

          writeAddressSet.Add(address);
        }
        catch (I2cNackException ex) when (ex.Address.Equals(address)) {
          // expected exception
        }

        try {
          await ReadByteAsync(address, cancellationToken).ConfigureAwait(false);

          readAddressSet.Add(address);
        }
        catch (I2cReadException ex) when (ex.Address.Equals(address)) {
          // expected exception
        }
      }

      return (writeAddressSet, readAddressSet);
    }

    public (IReadOnlyI2cAddressSet WriteAddressSet, IReadOnlyI2cAddressSet ReadAddressSet) ScanBus(
      I2cAddress addressRangeMin = default,
      I2cAddress addressRangeMax = default,
      IProgress<I2cScanBusProgress>? progress = null,
      CancellationToken cancellationToken = default
    )
    {
      if ((int)addressRangeMax < (int)addressRangeMin)
        throw new ArgumentException($"{nameof(addressRangeMax)}({addressRangeMax}) must be greater than or equals to {nameof(addressRangeMin)}({addressRangeMin})", nameof(addressRangeMax));

      if (addressRangeMin.Equals(I2cAddress.Zero))
        addressRangeMin = I2cAddress.DeviceMinValue;
      if (addressRangeMax.Equals(I2cAddress.Zero))
        addressRangeMax = I2cAddress.DeviceMaxValue;

      var writeAddressSet = new SortedSet<I2cAddress>();
      var readAddressSet = new SortedSet<I2cAddress>();

      for (var addr = (int)addressRangeMin; addr <= (int)addressRangeMax; addr++) {
        var address = new I2cAddress(addr);

        progress?.Report(new I2cScanBusProgress(address, addressRangeMin, addressRangeMax));

        try {
          Write(address, default, cancellationToken);

          writeAddressSet.Add(address);
        }
        catch (I2cNackException ex) when (ex.Address.Equals(address)) {
          // expected exception
        }

        try {
          ReadByte(address, cancellationToken);

          readAddressSet.Add(address);
        }
        catch (I2cReadException ex) when (ex.Address.Equals(address)) {
          // expected exception
        }
      }

      return (writeAddressSet, readAddressSet);
    }
  }
}
