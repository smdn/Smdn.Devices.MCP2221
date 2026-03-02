// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT

using System.Threading;
using System.Threading.Tasks;

namespace Smdn.Devices.MCP2221;

#pragma warning disable IDE0040
partial class Mcp2221 {
#pragma warning restore IDE0040
  internal interface IDacFunctionality {
    ValueTask ConfigureAsDacAsync(
      CancellationToken cancellationToken = default
    );
    void ConfigureAsDac(
      CancellationToken cancellationToken = default
    );
#if __FUTURE_VERSION
    int DacValue { set; }
#endif
  }
}
