// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using IReadOnlyI2CAddressSet =
#if NET5_0_OR_GREATER
  System.Collections.Generic.IReadOnlySet
#else
  System.Collections.Generic.IReadOnlyCollection
#endif
  <Smdn.Devices.MCP2221.I2CAddress>;

namespace Smdn.Devices.MCP2221 {
  partial class MCP2221 {
    partial class I2CFunctionality {
      public async ValueTask<(IReadOnlyI2CAddressSet writeAddressSet, IReadOnlyI2CAddressSet readAddressSet)> ScanBusAsync(
        I2CAddress addressRangeMin = default,
        I2CAddress addressRangeMax = default,
        IProgress<I2CScanBusProgress> progress = null,
        CancellationToken cancellationToken = default
      )
      {
        if ((int)addressRangeMax < (int)addressRangeMin)
          throw new ArgumentException($"{nameof(addressRangeMax)}({addressRangeMax}) must be greater than or equals to {nameof(addressRangeMin)}({addressRangeMin})", nameof(addressRangeMax));

        if (addressRangeMin.Equals(I2CAddress.Zero))
          addressRangeMin = I2CAddress.DeviceMinValue;
        if (addressRangeMax.Equals(I2CAddress.Zero))
          addressRangeMax = I2CAddress.DeviceMaxValue;

        var writeAddressSet = new SortedSet<I2CAddress>();
        var readAddressSet = new SortedSet<I2CAddress>();

        for (var addr = (int)addressRangeMin; addr <= (int)addressRangeMax; addr++) {
          var address = new I2CAddress(addr);

          progress?.Report(new I2CScanBusProgress(address, addressRangeMin, addressRangeMax));

          try {
            await WriteAsync(address, default, cancellationToken).ConfigureAwait(false);

            writeAddressSet.Add(address);
          }
          catch (I2CNAckException ex) when (ex.Address.Equals(address)) {
            // expected exception
          }

          try {
            await ReadByteAsync(address, cancellationToken).ConfigureAwait(false);

            readAddressSet.Add(address);
          }
          catch (I2CReadException ex) when (ex.Address.Equals(address)) {
            // expected exception
          }
        }

        return (writeAddressSet, readAddressSet);
      }

      public (IReadOnlyI2CAddressSet writeAddressSet, IReadOnlyI2CAddressSet readAddressSet) ScanBus(
        I2CAddress addressRangeMin = default,
        I2CAddress addressRangeMax = default,
        IProgress<I2CScanBusProgress> progress = null,
        CancellationToken cancellationToken = default
      )
      {
        if ((int)addressRangeMax < (int)addressRangeMin)
          throw new ArgumentException($"{nameof(addressRangeMax)}({addressRangeMax}) must be greater than or equals to {nameof(addressRangeMin)}({addressRangeMin})", nameof(addressRangeMax));

        if (addressRangeMin.Equals(I2CAddress.Zero))
          addressRangeMin = I2CAddress.DeviceMinValue;
        if (addressRangeMax.Equals(I2CAddress.Zero))
          addressRangeMax = I2CAddress.DeviceMaxValue;

        var writeAddressSet = new SortedSet<I2CAddress>();
        var readAddressSet = new SortedSet<I2CAddress>();

        for (var addr = (int)addressRangeMin; addr <= (int)addressRangeMax; addr++) {
          var address = new I2CAddress(addr);

          progress?.Report(new I2CScanBusProgress(address, addressRangeMin, addressRangeMax));

          try {
            Write(address, default, cancellationToken);

            writeAddressSet.Add(address);
          }
          catch (I2CNAckException ex) when (ex.Address.Equals(address)) {
            // expected exception
          }

          try {
            ReadByte(address, cancellationToken);

            readAddressSet.Add(address);
          }
          catch (I2CReadException ex) when (ex.Address.Equals(address)) {
            // expected exception
          }
        }

        return (writeAddressSet, readAddressSet);
      }
    }
  }
}
