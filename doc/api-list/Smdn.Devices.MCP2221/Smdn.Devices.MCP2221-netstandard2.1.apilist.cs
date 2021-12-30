// Smdn.Devices.MCP2221.dll (Smdn.Devices.MCP2221)
//   Name: Smdn.Devices.MCP2221
//   AssemblyVersion: 0.9.0.0
//   InformationalVersion: 0.9 (netstandard2.1)
//   TargetFramework: .NETStandard,Version=v2.1
//   Configuration: Release

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Smdn.Devices.MCP2221;
using Smdn.Devices.UsbHid;

namespace Smdn.Devices.MCP2221 {
  public enum GPIODirection : byte {
    Input = 1,
    Output = 0,
  }

  public enum GPIOLevel : byte {
    High = 1,
    Low = 0,
  }

  public enum I2CBusSpeed : int {
    Default = 0,
    FastMode = 2,
    LowSpeedMode = 1,
    Speed100kBitsPerSec = 0,
    Speed10kBitsPerSec = 1,
    Speed400kBitsPerSec = 2,
    StandardMode = 0,
  }

  public class CommandException : InvalidOperationException {
    public CommandException(string message) {}
    public CommandException(string message, Exception innerException) {}
  }

  public class DeviceNotFoundException : InvalidOperationException {
    public DeviceNotFoundException() {}
    public DeviceNotFoundException(string message) {}
  }

  public class I2CCommandException : CommandException {
    public I2CCommandException(I2CAddress address, string message) {}
    public I2CCommandException(I2CAddress address, string message, Exception innerException) {}
    public I2CCommandException(string message) {}
    public I2CCommandException(string message, Exception innerException) {}

    public I2CAddress Address { get; }
  }

  public class I2CNAckException : I2CCommandException {
    public I2CNAckException(I2CAddress address) {}
    public I2CNAckException(I2CAddress address, Exception innerException) {}
    public I2CNAckException(string message) {}
    public I2CNAckException(string message, Exception innerException) {}
  }

  public class I2CReadException : I2CCommandException {
    public I2CReadException(I2CAddress address, string message) {}
    public I2CReadException(I2CAddress address, string message, Exception innerException) {}
    public I2CReadException(string message) {}
    public I2CReadException(string message, Exception innerException) {}
  }

  public class MCP2221 :
    IAsyncDisposable,
    IDisposable
  {
    public sealed class GP0Functionality : GPFunctionality {
      public void ConfigureAsLEDURX(CancellationToken cancellationToken = default) {}
      public ValueTask ConfigureAsLEDURXAsync(CancellationToken cancellationToken = default) {}
      public void ConfigureAsSSPND(CancellationToken cancellationToken = default) {}
      public ValueTask ConfigureAsSSPNDAsync(CancellationToken cancellationToken = default) {}
    }

    public sealed class GP1Functionality : GPFunctionality {
      public void ConfigureAsADC(CancellationToken cancellationToken = default) {}
      public ValueTask ConfigureAsADCAsync(CancellationToken cancellationToken = default) {}
      public void ConfigureAsClockOutput(CancellationToken cancellationToken = default) {}
      public ValueTask ConfigureAsClockOutputAsync(CancellationToken cancellationToken = default) {}
      public void ConfigureAsInterruptDetection(CancellationToken cancellationToken = default) {}
      public ValueTask ConfigureAsInterruptDetectionAsync(CancellationToken cancellationToken = default) {}
      public void ConfigureAsLEDUTX(CancellationToken cancellationToken = default) {}
      public ValueTask ConfigureAsLEDUTXAsync(CancellationToken cancellationToken = default) {}
    }

    public sealed class GP2Functionality : GPFunctionality {
      public void ConfigureAsADC(CancellationToken cancellationToken = default) {}
      public ValueTask ConfigureAsADCAsync(CancellationToken cancellationToken = default) {}
      public void ConfigureAsDAC(CancellationToken cancellationToken = default) {}
      public ValueTask ConfigureAsDACAsync(CancellationToken cancellationToken = default) {}
      public void ConfigureAsUSBCFG(CancellationToken cancellationToken = default) {}
      public ValueTask ConfigureAsUSBCFGAsync(CancellationToken cancellationToken = default) {}
    }

    public sealed class GP3Functionality : GPFunctionality {
      public void ConfigureAsADC(CancellationToken cancellationToken = default) {}
      public ValueTask ConfigureAsADCAsync(CancellationToken cancellationToken = default) {}
      public void ConfigureAsDAC(CancellationToken cancellationToken = default) {}
      public ValueTask ConfigureAsDACAsync(CancellationToken cancellationToken = default) {}
      public void ConfigureAsLEDI2C(CancellationToken cancellationToken = default) {}
      public ValueTask ConfigureAsLEDI2CAsync(CancellationToken cancellationToken = default) {}
    }

    public abstract class GPFunctionality {
      public string PinDesignation { get; }
      public string PinName { get; }

      public void ConfigureAsGPIO(GPIODirection initialDirection = GPIODirection.Output, GPIOValue initialValue = default, CancellationToken cancellationToken = default) {}
      public ValueTask ConfigureAsGPIOAsync(GPIODirection initialDirection = GPIODirection.Output, GPIOValue initialValue = default, CancellationToken cancellationToken = default) {}
      public GPIODirection GetDirection(CancellationToken cancellationToken = default) {}
      public ValueTask<GPIODirection> GetDirectionAsync(CancellationToken cancellationToken = default) {}
      public GPIOValue GetValue(CancellationToken cancellationToken = default) {}
      public ValueTask<GPIOValue> GetValueAsync(CancellationToken cancellationToken = default) {}
      public void SetDirection(GPIODirection newDirection, CancellationToken cancellationToken = default) {}
      public ValueTask SetDirectionAsync(GPIODirection newDirection, CancellationToken cancellationToken = default) {}
      public void SetValue(GPIOValue newValue, CancellationToken cancellationToken = default) {}
      public ValueTask SetValueAsync(GPIOValue newValue, CancellationToken cancellationToken = default) {}
    }

    public sealed class I2CFunctionality {
      public const int MaxBlockLength = 65535;

      public I2CBusSpeed BusSpeed { get; set; }

      public int Read(I2CAddress address, Span<byte> buffer, CancellationToken cancellationToken = default) {}
      public int Read(I2CAddress address, byte[] buffer, int offset, int count, CancellationToken cancellationToken = default) {}
      [AsyncStateMachine]
      public ValueTask<int> ReadAsync(I2CAddress address, Memory<byte> buffer, CancellationToken cancellationToken = default) {}
      public ValueTask<int> ReadAsync(I2CAddress address, byte[] buffer, int offset, int count, CancellationToken cancellationToken = default) {}
      public int ReadByte(I2CAddress address, CancellationToken cancellationToken = default) {}
      [AsyncStateMachine]
      public ValueTask<int> ReadByteAsync(I2CAddress address, CancellationToken cancellationToken = default) {}
      public (IReadOnlyCollection<I2CAddress> writeAddressSet, IReadOnlyCollection<I2CAddress> readAddressSet) ScanBus(I2CAddress addressRangeMin = default, I2CAddress addressRangeMax = default, IProgress<I2CScanBusProgress> progress = null, CancellationToken cancellationToken = default) {}
      [AsyncStateMachine]
      public ValueTask<(IReadOnlyCollection<I2CAddress> writeAddressSet, IReadOnlyCollection<I2CAddress> readAddressSet)> ScanBusAsync(I2CAddress addressRangeMin = default, I2CAddress addressRangeMax = default, IProgress<I2CScanBusProgress> progress = null, CancellationToken cancellationToken = default) {}
      public void Write(I2CAddress address, ReadOnlySpan<byte> buffer, CancellationToken cancellationToken = default) {}
      public void Write(I2CAddress address, byte[] buffer, int offset, int count, CancellationToken cancellationToken = default) {}
      [AsyncStateMachine]
      public ValueTask WriteAsync(I2CAddress address, ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default) {}
      public ValueTask WriteAsync(I2CAddress address, byte[] buffer, int offset, int count, CancellationToken cancellationToken = default) {}
      public void WriteByte(I2CAddress address, byte @value, CancellationToken cancellationToken = default) {}
      [AsyncStateMachine]
      public ValueTask WriteByteAsync(I2CAddress address, byte @value, CancellationToken cancellationToken = default) {}
    }

    public const int DeviceProductID = 221;
    public const int DeviceVendorID = 1240;
    [Nullable(1)]
    public const string FirmwareRevisionMCP2221 = "1.1";
    [Nullable(1)]
    public const string FirmwareRevisionMCP2221A = "1.2";
    [Nullable(1)]
    public const string HardwareRevisionMCP2221 = "A.6";
    [Nullable(1)]
    public const string HardwareRevisionMCP2221A = "A.6";

    [Nullable(1)]
    public string ChipFactorySerialNumber { get; }
    [Nullable(1)]
    public string FirmwareRevision { get; }
    public MCP2221.GP0Functionality GP0 { get; }
    public MCP2221.GP1Functionality GP1 { get; }
    public MCP2221.GP2Functionality GP2 { get; }
    public MCP2221.GP3Functionality GP3 { get; }
    public IReadOnlyList<MCP2221.GPFunctionality> GPs { get; }
    [Nullable(1)]
    public string HardwareRevision { get; }
    [Nullable(1)]
    public IUsbHidDevice HidDevice { get; }
    public MCP2221.I2CFunctionality I2C { get; }

    public void Dispose() {}
    [AsyncStateMachine]
    public ValueTask DisposeAsync() {}
    [NullableContext(1)]
    public static MCP2221 Open(Func<IUsbHidDevice> createHidDevice, [Nullable(2)] IServiceProvider serviceProvider = null) {}
    [NullableContext(1)]
    public static MCP2221 Open([Nullable(2)] IServiceProvider serviceProvider = null) {}
    [NullableContext(1)]
    public static MCP2221 Open([Nullable] Predicate<IUsbHidDevice> findDevicePredicate, [Nullable(2)] IServiceProvider serviceProvider = null) {}
    [AsyncStateMachine]
    [NullableContext(1)]
    [return: Nullable] public static ValueTask<MCP2221> OpenAsync(Func<IUsbHidDevice> createHidDevice, [Nullable(2)] IServiceProvider serviceProvider = null) {}
    [NullableContext(2)]
    [return: Nullable] public static ValueTask<MCP2221> OpenAsync(IServiceProvider serviceProvider = null) {}
    [NullableContext(2)]
    [return: Nullable] public static ValueTask<MCP2221> OpenAsync([Nullable] Predicate<IUsbHidDevice> findDevicePredicate, IServiceProvider serviceProvider = null) {}
  }

  public readonly struct GPIOValue {
    public static readonly GPIOValue Default; // = "Low"
    public static readonly GPIOValue High; // = "High"
    public static readonly GPIOValue Low; // = "Low"

    public GPIOValue(GPIOLevel state) {}
    public GPIOValue(bool @value) {}
    public GPIOValue(byte @value) {}
    public GPIOValue(int @value) {}

    public bool IsHigh { get; }
    public bool IsLow { get; }

    public override string ToString() {}
    public static explicit operator GPIOLevel(GPIOValue @value) {}
    public static explicit operator bool(GPIOValue @value) {}
    public static explicit operator byte(GPIOValue @value) {}
    public static explicit operator int(GPIOValue @value) {}
    public static implicit operator GPIOValue(GPIOLevel @value) {}
    public static implicit operator GPIOValue(bool @value) {}
    public static implicit operator GPIOValue(byte @value) {}
    public static implicit operator GPIOValue(int @value) {}
  }

  public readonly struct I2CAddress :
    IComparable<I2CAddress>,
    IEquatable<I2CAddress>,
    IEquatable<byte>,
    IEquatable<int>
  {
    public static readonly I2CAddress DeviceMaxValue; // = "77"
    public static readonly I2CAddress DeviceMinValue; // = "08"
    public static readonly I2CAddress Zero; // = "00"

    public I2CAddress(int address) {}
    public I2CAddress(int deviceAddressBits, int hardwareAddressBits) {}

    public int CompareTo(I2CAddress other) {}
    public bool Equals(I2CAddress other) {}
    public bool Equals(byte other) {}
    public bool Equals(int other) {}
    public override bool Equals(object obj) {}
    public override int GetHashCode() {}
    public override string ToString() {}
    public static bool operator == (I2CAddress x, I2CAddress y) {}
    public static explicit operator byte(I2CAddress address) {}
    public static explicit operator int(I2CAddress address) {}
    public static implicit operator I2CAddress(byte address) {}
    public static bool operator != (I2CAddress x, I2CAddress y) {}
  }

  public readonly struct I2CScanBusProgress {
    public I2CAddress AddressRangeMax { get; }
    public I2CAddress AddressRangeMin { get; }
    public int ProgressInPercent { get; }
    public I2CAddress ScanningAddress { get; }
  }
}

namespace Smdn.Devices.UsbHid {
  public interface IUsbHidDevice :
    IAsyncDisposable,
    IDisposable
  {
    string DevicePath { get; }
    string FileSystemName { get; }
    string Manufacturer { get; }
    int ProductID { get; }
    string ProductName { get; }
    Version ReleaseNumber { get; }
    string SerialNumber { get; }
    int VendorID { get; }

    IUsbHidStream OpenStream();
    ValueTask<IUsbHidStream> OpenStreamAsync();
  }

  public interface IUsbHidStream :
    IAsyncDisposable,
    IDisposable
  {
    bool RequiresPacketOnly { get; }

    int Read(Span<byte> buffer);
    ValueTask<int> ReadAsync(Memory<byte> buffer);
    void Write(ReadOnlySpan<byte> buffer);
    ValueTask WriteAsync(ReadOnlyMemory<byte> buffer);
  }

  public static class Log {
    public static LogLevel NativeLibraryLogLevel { get; set; }
  }

  public class UsbHidException : InvalidOperationException {
    public UsbHidException() {}
    public UsbHidException(string message) {}
  }
}

