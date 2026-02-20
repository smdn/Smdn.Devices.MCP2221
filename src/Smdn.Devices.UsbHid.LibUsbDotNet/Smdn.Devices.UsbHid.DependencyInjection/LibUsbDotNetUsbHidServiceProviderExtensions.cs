// SPDX-FileCopyrightText: 2026 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;

using Polly;
using Polly.Registry;

namespace Smdn.Devices.UsbHid.DependencyInjection;

public static class LibUsbDotNetUsbHidServiceProviderExtensions {
  [CLSCompliant(false)] // ResiliencePipelineProvider is CLS incompliant
  public static ResiliencePipelineProvider<string>? GetResiliencePipelineProviderForLibUsbDotNetUsbHidService(
    this IServiceProvider serviceProvider,
    object? serviceKey
  )
    => (serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider))).GetKeyedResiliencePipelineProvider<string>(
      serviceKey: serviceKey,
      typeOfKeyPair: typeof(LibUsbDotNetResiliencePipelineKeyPair<>)
    );
}
