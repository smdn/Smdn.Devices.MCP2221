// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Smdn.Devices.MCP2221;

partial class MCP2221 {
  internal interface IDACFunctionality {
    ValueTask ConfigureAsDACAsync(
      CancellationToken cancellationToken = default
    );
    void ConfigureAsDAC(
      CancellationToken cancellationToken = default
    );
#if __FUTURE_VERSION
    int DACValue { set; }
#endif
  }
}
