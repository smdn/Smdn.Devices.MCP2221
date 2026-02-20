// SPDX-FileCopyrightText: 2026 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;

using Microsoft.Extensions.DependencyInjection;

namespace Smdn.Devices.UsbHid.DependencyInjection;

/// <summary>
/// Provides a builder for configuring USB-HID services.
/// </summary>
[CLSCompliant(false)] // IServiceCollection is not CLS compliant
public abstract class UsbHidServiceBuilder<TServiceKey>(
  IServiceCollection services,
  TServiceKey serviceKey,
  Func<TServiceKey, string?>? selectOptionsNameForServiceKey
) {
  /// <summary>
  /// Gets the <see cref="IServiceCollection"/> where the USB-HID services are configured.
  /// </summary>
  public IServiceCollection Services { get; } = services ?? throw new ArgumentNullException(nameof(services));

  /// <summary>
  /// Gets the <typeparamref name="TServiceKey"/> key for configured USB-HID services.
  /// </summary>
  /// <remarks>
  /// If a key is not explicitly specified, the type of <typeparamref name="TServiceKey"/>
  /// will be <see cref="object"/><c>?</c> and the value will be <see langword="null"/>.
  /// </remarks>
  public TServiceKey ServiceKey { get; } = serviceKey;

  /// <summary>
  /// Constructs an instance of <see cref="IUsbHidService"/> using the current configuration
  /// and the services provided by <paramref name="serviceProvider"/>.
  /// </summary>
  /// <param name="serviceProvider">
  /// An <see cref="IServiceProvider"/> that provides the services required
  /// to construct the <see cref="IUsbHidService"/>.
  /// </param>
  /// <returns>
  /// The constructed <see cref="IUsbHidService"/>.
  /// </returns>
  /// <exception cref="InvalidOperationException">
  /// A service required to construct the <see cref="IUsbHidService"/> was not
  /// provided by <paramref name="serviceProvider"/>.
  /// </exception>
  public abstract IUsbHidService Build(IServiceProvider serviceProvider);

#pragma warning disable CS1574 // cannot resolve cref Microsoft.Extensions.Options.IOptionsMonitor
  /// <summary>
  /// Gets the name of the configured options to be passed to the <c>name</c> parameter of the
  /// <see cref="Microsoft.Extensions.Options.IOptionsMonitor{TOptions}.Get"/> method.
  /// </summary>
  /// <remarks>
  /// This method calls the <see cref="Func{T, TResult}"/> specified in the constructor
  /// to get the name of the configured options associated with <see cref="ServiceKey"/>.
  /// If the function is not provided, an exception is thrown.
  /// </remarks>
  /// <exception cref="InvalidOperationException">
  /// The function to select the options name was not provided to the constructor.
  /// </exception>
  public string? GetOptionsName()
  {
    if (selectOptionsNameForServiceKey is null)
      throw new InvalidOperationException($"The name of the configured options cannot be selected from {nameof(ServiceKey)}.");

    return selectOptionsNameForServiceKey(ServiceKey);
  }
#pragma warning restore CS1574
}
