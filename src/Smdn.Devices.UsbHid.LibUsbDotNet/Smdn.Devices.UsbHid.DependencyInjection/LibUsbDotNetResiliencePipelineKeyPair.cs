// SPDX-FileCopyrightText: 2026 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;

using Microsoft.Extensions.DependencyInjection;

using Polly;
using Polly.Registry.KeyedRegistry;

namespace Smdn.Devices.UsbHid.DependencyInjection;

#pragma warning disable IDE0055
public readonly record struct LibUsbDotNetResiliencePipelineKeyPair<TServiceKey> :
  IResiliencePipelineKeyPair<TServiceKey, string>,
  IEquatable<LibUsbDotNetResiliencePipelineKeyPair<TServiceKey>>
#pragma warning restore IDE0055
{
  /// <summary>
  /// Gets a key of <typeparamref name="TServiceKey"/> type specified when the <see cref="ResiliencePipeline"/>
  /// is registered to the <see cref="IServiceCollection"/>.
  /// </summary>
  public TServiceKey ServiceKey { get; }

  /// <summary name="pipelineKey">
  /// Gets a key for <see cref="ResiliencePipeline"/> referenced by <see cref="LibUsbDotNetUsbHidService"/>.
  /// </summary>
  public string PipelineKey { get; }

  public LibUsbDotNetResiliencePipelineKeyPair(TServiceKey serviceKey, string pipelineKey)
  {
    if (pipelineKey is null)
      throw new ArgumentNullException(nameof(pipelineKey));
    if (string.IsNullOrEmpty(pipelineKey))
      throw new ArgumentException(message: "must be non-empty string", paramName: nameof(pipelineKey));

    ServiceKey = serviceKey;
    PipelineKey = pipelineKey;
  }

  public override string ToString()
    => $"{{{ServiceKey}:{PipelineKey}}}";
}
