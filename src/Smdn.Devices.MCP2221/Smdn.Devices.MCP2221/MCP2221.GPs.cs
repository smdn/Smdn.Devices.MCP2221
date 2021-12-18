// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT

using System;
using System.Buffers;
using System.Collections.Generic;
using System.Device.Gpio;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace Smdn.Devices.MCP2221;

#pragma warning disable IDE0040
partial class MCP2221 {
#pragma warning restore IDE0040

#if __FUTURE_VERSION
  public interface IReadOnlyGPFunctionalityList : IReadOnlyList<GPFunctionality> {
    void SetDirection(
      PinMode directionAllGPs
    );
    void SetDirection(
      PinMode directionGP0,
      PinMode directionGP1,
      PinMode directionGP2,
      PinMode directionGP3
    );
    void GetDirection(
      out PinMode directionGP0,
      out PinMode directionGP1,
      out PinMode directionGP2,
      out PinMode directionGP3
    );
    void SetValue(
      PinValue valueAllGPs
    );
    void SetValue(
      PinValue valueGP0,
      PinValue valueGP1,
      PinValue valueGP2,
      PinValue valueGP3
    );
    void GetValue(
      out PinValue valueGP0,
      out PinValue valueGP1,
      out PinValue valueGP2,
      out PinValue valueGP3
    );
  }
#endif

  public IReadOnlyList<GPFunctionality> GPs { get; }

  public GP0Functionality GP0 { get; }
  public GP1Functionality GP1 { get; }
  public GP2Functionality GP2 { get; }
  public GP3Functionality GP3 { get; }

  // [MCP2221A] 3.1.13 SET SRAM SETTINGS
  // Byte Index 8-11 GP0-3 Settings
  // Bit 2-0: GP<n> Designation
  internal enum GPDesignation : byte {
    AlternateFunction2          = 0b_000_0_0_100,
    AlternateFunction1          = 0b_000_0_0_011,
    AlternateFunction0          = 0b_000_0_0_010,
    DedicatedFunctionOperation  = 0b_000_0_0_001,
    GPIOOperation               = 0b_000_0_0_000,
  }

  public sealed class GP0Functionality : GPFunctionality {
    private protected override int GPIndex => 0;

    internal GP0Functionality(MCP2221 device) : base(device) { }

    public ValueTask ConfigureAsLEDURXAsync(CancellationToken cancellationToken = default)
      => ConfigureGPDesignationAsync(
        pinDesignation: "LED_URX",
        gpDesignation: GPDesignation.AlternateFunction0,
        cancellationToken: cancellationToken
      );
    public void ConfigureAsLEDURX(CancellationToken cancellationToken = default)
      => ConfigureGPDesignation(
        pinDesignation: "LED_URX",
        gpDesignation: GPDesignation.AlternateFunction0,
        cancellationToken: cancellationToken
      );

    public ValueTask ConfigureAsSSPNDAsync(CancellationToken cancellationToken = default)
      => ConfigureGPDesignationAsync(
        pinDesignation: "SSPND",
        gpDesignation: GPDesignation.DedicatedFunctionOperation,
        cancellationToken: cancellationToken
      );
    public void ConfigureAsSSPND(CancellationToken cancellationToken = default)
      => ConfigureGPDesignation(
        pinDesignation: "SSPND",
        gpDesignation: GPDesignation.DedicatedFunctionOperation,
        cancellationToken: cancellationToken
      );
  }

  public sealed class GP1Functionality : GPFunctionality, IInterruptDetectionFunctionality, IADCFunctionality, IClockOutputFunctionality {
    private protected override int GPIndex => 1;

    internal GP1Functionality(MCP2221 device) : base(device) { }

    public ValueTask ConfigureAsInterruptDetectionAsync(CancellationToken cancellationToken = default)
      => ConfigureGPDesignationAsync(
        pinDesignation: "Interrupt Detection",
        gpDesignation: GPDesignation.AlternateFunction2,
        cancellationToken: cancellationToken
      );
    public void ConfigureAsInterruptDetection(CancellationToken cancellationToken = default)
      => ConfigureGPDesignation(
        pinDesignation: "Interrupt Detection",
        gpDesignation: GPDesignation.AlternateFunction2,
        cancellationToken: cancellationToken
      );

    public ValueTask ConfigureAsLEDUTXAsync(CancellationToken cancellationToken = default)
      => ConfigureGPDesignationAsync(
        pinDesignation: "LED_UTX",
        gpDesignation: GPDesignation.AlternateFunction1,
        cancellationToken: cancellationToken
      );
    public void ConfigureAsLEDUTX(CancellationToken cancellationToken = default)
      => ConfigureGPDesignation(
        pinDesignation: "LED_UTX",
        gpDesignation: GPDesignation.AlternateFunction1,
        cancellationToken: cancellationToken
      );

    public ValueTask ConfigureAsADCAsync(CancellationToken cancellationToken = default)
      => ConfigureGPDesignationAsync(
        pinDesignation: "ADC1",
        gpDesignation: GPDesignation.AlternateFunction0,
        cancellationToken: cancellationToken
      );
    public void ConfigureAsADC(CancellationToken cancellationToken = default)
      => ConfigureGPDesignation(
        pinDesignation: "ADC1",
        gpDesignation: GPDesignation.AlternateFunction0,
        cancellationToken: cancellationToken
      );

    public ValueTask ConfigureAsClockOutputAsync(CancellationToken cancellationToken = default)
      => ConfigureGPDesignationAsync(
        pinDesignation: "Clock Output",
        gpDesignation: GPDesignation.DedicatedFunctionOperation,
        cancellationToken: cancellationToken
      );
    public void ConfigureAsClockOutput(CancellationToken cancellationToken = default)
      => ConfigureGPDesignation(
        pinDesignation: "Clock Output",
        gpDesignation: GPDesignation.DedicatedFunctionOperation,
        cancellationToken: cancellationToken
      );
  }

  public sealed class GP2Functionality : GPFunctionality, IADCFunctionality, IDACFunctionality {
    private protected override int GPIndex => 2;

    internal GP2Functionality(MCP2221 device) : base(device) { }

    public ValueTask ConfigureAsDACAsync(CancellationToken cancellationToken = default)
      => ConfigureGPDesignationAsync(
        pinDesignation: "DAC1",
        gpDesignation: GPDesignation.AlternateFunction1,
        cancellationToken: cancellationToken
      );
    public void ConfigureAsDAC(CancellationToken cancellationToken = default)
      => ConfigureGPDesignation(
        pinDesignation: "DAC1",
        gpDesignation: GPDesignation.AlternateFunction1,
        cancellationToken: cancellationToken
      );

    public ValueTask ConfigureAsADCAsync(CancellationToken cancellationToken = default)
      => ConfigureGPDesignationAsync(
        pinDesignation: "ADC2",
        gpDesignation: GPDesignation.AlternateFunction0,
        cancellationToken: cancellationToken
      );
    public void ConfigureAsADC(CancellationToken cancellationToken = default)
      => ConfigureGPDesignation(
        pinDesignation: "ADC2",
        gpDesignation: GPDesignation.AlternateFunction0,
        cancellationToken: cancellationToken
      );

    public ValueTask ConfigureAsUSBCFGAsync(CancellationToken cancellationToken = default)
      => ConfigureGPDesignationAsync(
        pinDesignation: "USBCFG",
        gpDesignation: GPDesignation.DedicatedFunctionOperation,
        cancellationToken: cancellationToken
      );
    public void ConfigureAsUSBCFG(CancellationToken cancellationToken = default)
      => ConfigureGPDesignation(
        pinDesignation: "USBCFG",
        gpDesignation: GPDesignation.DedicatedFunctionOperation,
        cancellationToken: cancellationToken
      );
  }

  public sealed class GP3Functionality : GPFunctionality, IADCFunctionality, IDACFunctionality {
    private protected override int GPIndex => 3;

    internal GP3Functionality(MCP2221 device) : base(device) { }

    public ValueTask ConfigureAsDACAsync(CancellationToken cancellationToken = default)
      => ConfigureGPDesignationAsync(
        pinDesignation: "DAC2",
        gpDesignation: GPDesignation.AlternateFunction1,
        cancellationToken: cancellationToken
      );
    public void ConfigureAsDAC(CancellationToken cancellationToken = default)
      => ConfigureGPDesignation(
        pinDesignation: "DAC2",
        gpDesignation: GPDesignation.AlternateFunction1,
        cancellationToken: cancellationToken
      );

    public ValueTask ConfigureAsADCAsync(CancellationToken cancellationToken = default)
      => ConfigureGPDesignationAsync(
        pinDesignation: "ADC3",
        gpDesignation: GPDesignation.AlternateFunction0,
        cancellationToken: cancellationToken
      );
    public void ConfigureAsADC(CancellationToken cancellationToken = default)
      => ConfigureGPDesignation(
        pinDesignation: "ADC3",
        gpDesignation: GPDesignation.AlternateFunction0,
        cancellationToken: cancellationToken
      );

    public ValueTask ConfigureAsLEDI2CAsync(CancellationToken cancellationToken = default)
      => ConfigureGPDesignationAsync(
        pinDesignation: "LED_I2C",
        gpDesignation: GPDesignation.DedicatedFunctionOperation,
        cancellationToken: cancellationToken
      );
    public void ConfigureAsLEDI2C(CancellationToken cancellationToken = default)
      => ConfigureGPDesignation(
        pinDesignation: "LED_I2C",
        gpDesignation: GPDesignation.DedicatedFunctionOperation,
        cancellationToken: cancellationToken
      );
  }

  public abstract partial class GPFunctionality {
    private const int NumberOfGPs = 4;

    private readonly MCP2221 device;
    private protected abstract int GPIndex { get; }
    public string PinName => $"GP{GPIndex}";
    public string PinDesignation { get; private set; }

    private protected GPFunctionality(MCP2221 device)
    {
      this.device = device;
    }

    private static class GetGPSettingsCommand {
      public static void ConstructCommand(Span<byte> comm, ReadOnlySpan<byte> userData, Memory<byte> gpSettings)
      {
        // [MCP2221A] 3.1.14 GET SRAM SETTINGS
        comm[0] = 0x61; // Get SRAM Settings
      }

      public static bool ParseResponse(ReadOnlySpan<byte> resp, Memory<byte> gpSettings)
      {
        resp.Slice(22, 4).CopyTo(gpSettings.Span); // GP0-3 Settings

        return true;
      }
    }

    private static class SetGPSettingsCommand {
      [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1316:TupleElementNamesShouldUseCorrectCasing", Justification = "Not a publicly-exposed type or member.")]
      public static void ConstructCommand(
        Span<byte> comm,
        ReadOnlySpan<byte> userData,
        (
          ReadOnlyMemory<byte> gpSettings,
          int gpIndex,
          GPDesignation gpDesignation,
          PinMode gpioDirection,
          PinValue gpioValue
        ) args
      )
      {
        // [MCP2221A] 3.1.13 SET SRAM SETTINGS
        comm[0] = 0x60; // Set SRAM settings
#if false
        comm[1] = 0x00; // Don't care
        comm[2] = 0b00000000; // Clock Output Driver Value = remain unaltered (0b0_______)
        comm[3] = 0b00000000; // DAC Voltage Reference = remain unaltered (0b0_______)
        comm[4] = 0b00000000; // Set DAC Output Value = remain unaltered (0b0_______)
        comm[5] = 0b00000000; // ADC Voltage Reference = remain unaltered (0b0_______)
        comm[6] = 0b00000000; // Setup the interrupt detection mechanism and clear the detection flag = remain unaltered (0b0_______)
#endif
        comm[7] = 0b10000000; // Alter GPIO configuration = Alter the GP designation (1)

        const int firstIndexOfGPSettings = 8; // GP0 Settings

        // copy current GP0-GP3 settings
        args.gpSettings.Span.CopyTo(comm.Slice(firstIndexOfGPSettings, NumberOfGPs));

        // construct new GP<n> settings
        var bitsGpioOutputValue = (bool)args.gpioValue
          ? 0b_000_1_0_000
          : 0b_000_0_0_000;
        var bitsGpioDirection = args.gpioDirection switch {
          PinMode.Input => 0b_000_0_1_000,
          PinMode.Output => 0b_000_0_0_000,
          _ => throw new ArgumentOutOfRangeException(nameof(args.gpioDirection), args.gpioDirection, $"must be {nameof(PinMode.Input)} or {nameof(PinMode.Output)}"),
        };
        var bitsGPnDesignation = (byte)args.gpDesignation & 0b_000_0_0_111;

        comm[firstIndexOfGPSettings + args.gpIndex] = (byte)(
          // Byte Index 8-11 GP0-3 Settings
          0b_000_0_0_000 | // Bit 7-5: Don't care
          bitsGpioOutputValue | // Bit 4: GPIO Output value
          bitsGpioDirection | // Bit 3: GPIO Direction
          bitsGPnDesignation // Bit 2-0: GP<n> Designation
        );
      }

      [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1316:TupleElementNamesShouldUseCorrectCasing", Justification = "Not a publicly-exposed type or member.")]
      [SuppressMessage("StyleCop.CSharp.MaintainAbilityRules", "SA1414:TupleTypesInSignaturesShouldHaveElementNames", Justification = "Not a publicly-exposed type or member.")]
#pragma warning disable IDE0060, SA1313 // [IDE0060] Remove unused parameter [SA1313] SA1313ParameterNamesMustBeginWithLowerCaseLetter
      public static bool ParseResponse(
        ReadOnlySpan<byte> resp,
        (
          ReadOnlyMemory<byte>,
          int,
          GPDesignation,
          PinMode,
          PinValue
        ) _
      )
#pragma warning restore IDE0060, SA1313
      {
        return resp[1] switch {
          0x00 => true, // Command completed successfully
          _ => throw new CommandException($"unexpected command response ({resp[1]:X2})"),
        };
      }
    }

    private protected async ValueTask ConfigureGPDesignationAsync(
      string pinDesignation,
      GPDesignation gpDesignation,
      PinMode gpioInitialDirection = default,
      PinValue gpioInitialValue = default,
      CancellationToken cancellationToken = default
    )
    {
      var gpSettings = ArrayPool<byte>.Shared.Rent(NumberOfGPs);

      try {
        // retrieve current GP0-GP3 settings
        _ = await device.CommandAsync(
          userData: default,
          arg: gpSettings.AsMemory(0, 4),
          cancellationToken: cancellationToken,
          constructCommand: GetGPSettingsCommand.ConstructCommand,
          parseResponse: GetGPSettingsCommand.ParseResponse
        ).ConfigureAwait(false);

        // overwrite GPn settings and set GP0-GP3 settings
        _ = await device.CommandAsync(
          userData: default,
          arg: ((ReadOnlyMemory<byte>)gpSettings.AsMemory(0, 4), GPIndex, gpDesignation, gpioInitialDirection, gpioInitialValue),
          cancellationToken: cancellationToken,
          constructCommand: SetGPSettingsCommand.ConstructCommand,
          parseResponse: SetGPSettingsCommand.ParseResponse
        ).ConfigureAwait(false);

        PinDesignation = pinDesignation;
      }
      finally {
        ArrayPool<byte>.Shared.Return(gpSettings);
      }
    }

    private protected void ConfigureGPDesignation(
      string pinDesignation,
      GPDesignation gpDesignation,
      PinMode gpioInitialDirection = default,
      PinValue gpioInitialValue = default,
      CancellationToken cancellationToken = default
    )
    {
      var gpSettings = ArrayPool<byte>.Shared.Rent(4);

      try {
        // retrieve current GP0-GP3 settings
        device.Command(
          userData: default,
          arg: gpSettings.AsMemory(0, 4),
          cancellationToken: cancellationToken,
          constructCommand: GetGPSettingsCommand.ConstructCommand,
          parseResponse: GetGPSettingsCommand.ParseResponse
        );

        // overwrite GPn settings and set GP0-GP3 settings
        device.Command(
          userData: default,
          arg: ((ReadOnlyMemory<byte>)gpSettings.AsMemory(0, 4), GPIndex, gpDesignation, gpioInitialDirection, gpioInitialValue),
          cancellationToken: cancellationToken,
          constructCommand: SetGPSettingsCommand.ConstructCommand,
          parseResponse: SetGPSettingsCommand.ParseResponse
        );

        PinDesignation = pinDesignation;
      }
      finally {
        ArrayPool<byte>.Shared.Return(gpSettings);
      }
    }
  }
}
