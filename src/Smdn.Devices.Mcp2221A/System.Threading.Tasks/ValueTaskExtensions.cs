// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
namespace System.Threading.Tasks;

internal static class ValueTaskExtensions {
  public static async ValueTask AsValueTask<T>(this ValueTask<T> task)
    => await task.ConfigureAwait(false);
}
