// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT

using System.Threading.Tasks;

namespace Smdn.Devices.MCP2221;

internal static class ValueTaskExtension {
  public static async ValueTask AsValueTask<T>(this ValueTask<T> task)
    => await task.ConfigureAwait(false);
}
