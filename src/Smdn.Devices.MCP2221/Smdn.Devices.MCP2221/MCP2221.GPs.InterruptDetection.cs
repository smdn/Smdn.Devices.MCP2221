// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT

using System.Threading;
using System.Threading.Tasks;

namespace Smdn.Devices.MCP2221;

#pragma warning disable IDE0040
partial class MCP2221 {
#pragma warning restore IDE0040
  internal interface IInterruptDetectionFunctionality {
    ValueTask ConfigureAsInterruptDetectionAsync(
      CancellationToken cancellationToken = default
    );
    void ConfigureAsInterruptDetection(
      CancellationToken cancellationToken = default
    );
#if __FUTURE_VERSION
#endif
  }
}
