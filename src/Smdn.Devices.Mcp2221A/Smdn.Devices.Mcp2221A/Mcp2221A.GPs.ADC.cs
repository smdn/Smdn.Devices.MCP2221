// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT

using System.Threading;
using System.Threading.Tasks;

namespace Smdn.Devices.Mcp2221A;

#pragma warning disable IDE0040
partial class Mcp2221A {
#pragma warning restore IDE0040
  internal interface IAdcFunctionality {
    ValueTask ConfigureAsAdcAsync(
      CancellationToken cancellationToken = default
    );
    void ConfigureAsAdc(
      CancellationToken cancellationToken = default
    );
#if __FUTURE_VERSION
    int AdcValue { get; }
#endif
  }
}
