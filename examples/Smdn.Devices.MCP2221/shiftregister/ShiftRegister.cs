using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Numerics;
using System.Threading.Tasks;

using Smdn.Devices.MCP2221;

enum BitOrder {
  LSBFirst,
  HSBFirst,
}

enum Endianness {
  LittleEndian,
  BigEndian,
}

class ShiftRegister {
  private readonly MCP2221.GPFunctionality /*IGPIOFunctionality*/ gpioLatch;
  private readonly MCP2221.GPFunctionality /*IGPIOFunctionality*/ gpioClock;
  private readonly MCP2221.GPFunctionality /*IGPIOFunctionality*/ gpioData;

  /// <param name="gpioLatch">storage register clock pin (RCLK/ST_CP)</param>
  /// <param name="gpioClock">shift register clock pin (SRCLK/SH_CP)</param>
  /// <param name="gpioData">serial output pin (SER)</param>
  public ShiftRegister(
    MCP2221.GPFunctionality /*IGPIOFunctionality*/ gpioLatch,
    MCP2221.GPFunctionality /*IGPIOFunctionality*/ gpioClock,
    MCP2221.GPFunctionality /*IGPIOFunctionality*/ gpioData
  )
  {
    this.gpioLatch = gpioLatch ?? throw new ArgumentNullException(nameof(gpioLatch));
    this.gpioClock = gpioClock ?? throw new ArgumentNullException(nameof(gpioClock));
    this.gpioData = gpioData ?? throw new ArgumentNullException(nameof(gpioData));
  }

  public async ValueTask WriteAsync(
    ReadOnlyMemory<byte> sequence,
    BitOrder bitOrder = default
  )
  {
    var (firstBitMask, shiftAmount) = bitOrder switch {
      BitOrder.LSBFirst => (0b_00000001u, +1),
      BitOrder.HSBFirst => (0b_10000000u, -1),
      _ => throw new ArgumentException($"undefined bit order ({bitOrder})", nameof(bitOrder)),
    };

    for (var byt = 0; byt < sequence.Length; byt++) {
      for (uint bit = 0u, bitMask = firstBitMask; bit < 8u; bit++) {
        await gpioData.SetValueAsync(0L != (sequence.Span[byt] & bitMask)).ConfigureAwait(false);

        await gpioClock.SetValueAsync(GPIOValue.High).ConfigureAwait(false);
        await gpioClock.SetValueAsync(GPIOValue.Low).ConfigureAwait(false);

        bitMask = BitOperations.RotateLeft(bitMask, shiftAmount);
      }
    }

    await gpioLatch.SetValueAsync(GPIOValue.Low).ConfigureAwait(false);
    await gpioLatch.SetValueAsync(GPIOValue.High).ConfigureAwait(false);
  }

  public void Write(
    ReadOnlySpan<byte> sequence,
    BitOrder bitOrder = default
  )
  {
    var (firstBitMask, shiftAmount) = bitOrder switch {
      BitOrder.LSBFirst => (0b_00000001u, +1),
      BitOrder.HSBFirst => (0b_10000000u, -1),
      _ => throw new ArgumentException($"undefined bit order ({bitOrder})", nameof(bitOrder)),
    };

    for (var byt = 0; byt < sequence.Length; byt++) {
      for (uint bit = 0u, bitMask = firstBitMask; bit < 8u; bit++) {
        gpioData.SetValue(0L != (sequence[byt] & bitMask));

        gpioClock.SetValue(GPIOValue.High);
        gpioClock.SetValue(GPIOValue.Low);

        bitMask = BitOperations.RotateLeft(bitMask, shiftAmount);
      }
    }

    gpioLatch.SetValue(GPIOValue.Low);
    gpioLatch.SetValue(GPIOValue.High);
  }

  public async ValueTask WriteAsync(
    byte value,
    BitOrder bitOrder = default
  )
  {
    var sequence = ArrayPool<byte>.Shared.Rent(1);

    try {
      sequence[0] = value;

      await WriteAsync(sequence.AsMemory(0, 1), bitOrder).ConfigureAwait(false);
    }
    finally {
      ArrayPool<byte>.Shared.Return(sequence);
    }
  }

  public void Write(
    byte value,
    BitOrder bitOrder = default
  )
    => Write(stackalloc byte[1] { value }, bitOrder);

  public async ValueTask WriteAsync(
    uint value,
    Endianness endianness = Endianness.LittleEndian,
    BitOrder bitOrder = default
  )
  {
    var sequence = ArrayPool<byte>.Shared.Rent(4);

    try {
      if (endianness == Endianness.LittleEndian)
        BinaryPrimitives.WriteUInt32LittleEndian(sequence.AsSpan(0, 4), value);
      else if (endianness == Endianness.BigEndian)
        BinaryPrimitives.WriteUInt32BigEndian(sequence.AsSpan(0, 4), value);
      else
        throw new ArgumentException($"undefined endianness ({endianness})", nameof(endianness));

      await WriteAsync(sequence.AsMemory(0, 4), bitOrder).ConfigureAwait(false);
    }
    finally {
      ArrayPool<byte>.Shared.Return(sequence);
    }
  }

  public void Write(
    uint value,
    Endianness endianness = Endianness.LittleEndian,
    BitOrder bitOrder = default
  )
  {
    Span<byte> sequence = stackalloc byte[4];

    if (endianness == Endianness.LittleEndian)
      BinaryPrimitives.WriteUInt32LittleEndian(sequence, value);
    else if (endianness == Endianness.BigEndian)
      BinaryPrimitives.WriteUInt32BigEndian(sequence, value);
    else
      throw new ArgumentException($"undefined endianness ({endianness})", nameof(endianness));

    Write(sequence, bitOrder);
  }
}