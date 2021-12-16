// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Smdn.Devices.MCP2221;

partial class MCP2221 {
  internal interface IADCFunctionality {
    ValueTask ConfigureAsADCAsync(
      CancellationToken cancellationToken = default
    );
    void ConfigureAsADC(
      CancellationToken cancellationToken = default
    );
#if __FUTURE_VERSION
    int ADCValue { get; }
#endif
  }
}
