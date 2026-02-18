// SPDX-FileCopyrightText: 2026 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;

using Microsoft.Extensions.DependencyInjection;

namespace Smdn.Devices.UsbHid.DependencyInjection;

/// <summary>
/// An abstract class that provides a builder pattern for configuring the USB-HID services.
/// This class works as an extension point for adding extension methods for configuring
/// services using the builder pattern.
/// </summary>
[CLSCompliant(false)] // IServiceCollection is not CLS compliant
public abstract class UsbHidServiceBuilder<TServiceKey>(
  IServiceCollection services,
  TServiceKey serviceKey,
  Func<TServiceKey, string?> selectOptionsNameForServiceKey
) {
  /// <summary>
  /// Gets the <see cref="IServiceCollection"/> where the USB-HID services are configured.
  /// </summary>
  public IServiceCollection Services { get; } = services ?? throw new ArgumentNullException(nameof(services));

  /// <summary>
  /// Gets the <typeparamref name="TServiceKey"/> key for configured USB-HID services.
  /// </summary>
  /// <remarks>
  /// If a key is not explicitly specified, type of <typeparamref name="TServiceKey"/>
  /// will be <see cref="object"/><c>?</c> and the value will be <see langword="null"/>.
  /// </remarks>
  public TServiceKey ServiceKey { get; } = serviceKey;

  public abstract IUsbHidService Build(IServiceProvider serviceProvider);

#pragma warning disable CS1574 // cannot resolve cref Microsoft.Extensions.Options.IOptionsMonitor
  /// <summary>
  /// Gets the name of the configured options to be passed to the <c>name</c> parameter of the
  /// <see cref="Microsoft.Extensions.Options.IOptionsMonitor{TOptions}.Get"/> method.
  /// </summary>
  /// <remarks>
  /// This method calls <see cref="selectOptionsNameForServiceKey"/> to get the name
  /// of the configured options associated with <see cref="ServiceKey"/>.
  /// If <see cref="selectOptionsNameForServiceKey"/> is null, an exception is thrown.
  /// </remarks>
  /// <exception cref="InvalidOperationException">
  /// <see cref="selectOptionsNameForServiceKey"/> is null.
  /// </exception>
  public string? GetOptionsName()
  {
    if (selectOptionsNameForServiceKey is null)
      throw new InvalidOperationException($"The name of the configured options cannot be selected from {nameof(ServiceKey)}.");

    return selectOptionsNameForServiceKey(ServiceKey);
  }
#pragma warning restore CS1574
}
