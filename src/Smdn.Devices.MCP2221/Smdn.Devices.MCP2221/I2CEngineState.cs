// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT

using System;
using System.Device.Gpio;
using System.Linq;

namespace Smdn.Devices.MCP2221;

internal
#if NET5_0_OR_GREATER
readonly
#endif
struct I2CEngineState {
  public enum TransferStatus : byte {
    NoSpecialOperation = 0x00, // No special operation
    MarkedForCancellation = 0x10, // The current I2C/SMBus transfer was marked for cancellation
    AlreadyInIdleMode = 0x11, // The I2C engine was already in idle mode
  }

  public bool IsInitialState => StateMachineStateValue == 0 && Address == 0 && CommunicationSpeedDividerValue == 0;

  public TransferStatus BusStatus {
    get;
#if NET5_0_OR_GREATER
    init;
#else
    private set;
#endif
  }

  public byte StateMachineStateValue {
    get;
#if NET5_0_OR_GREATER
    init;
#else
    private set;
#endif
  }

  public int RequestedTransferLength {
    get;
#if NET5_0_OR_GREATER
    init;
#else
    private set;
#endif
  }

  public int AlreadyTransferredLength {
    get;
#if NET5_0_OR_GREATER
    init;
#else
    private set;
#endif
  }

  public int DataBufferCounter {
    get;
#if NET5_0_OR_GREATER
    init;
#else
    private set;
#endif
  }

  public ushort Address {
    get;
#if NET5_0_OR_GREATER
    init;
#else
    private set;
#endif
  }

  public int ReadPendingValue {
    get;
#if NET5_0_OR_GREATER
    init;
#else
    private set;
#endif
  }

  public byte CommunicationSpeedDividerValue {
    get;
#if NET5_0_OR_GREATER
    init;
#else
    private set;
#endif
  }

  public byte TimeoutValue {
    get;
#if NET5_0_OR_GREATER
    init;
#else
    private set;
#endif
  }

  public PinValue LineValueSCL {
    get;
#if NET5_0_OR_GREATER
    init;
#else
    private set;
#endif
  }

  public PinValue LineValueSDA {
    get;
#if NET5_0_OR_GREATER
    init;
#else
    private set;
#endif
  }

  public static I2CEngineState Parse(ReadOnlySpan<byte> resp)
    => new() {
#pragma warning disable IDE0055 // Fix formatting
      // [MCP2221A] 3.1.1 STATUS/SET PARAMATERS
      BusStatus                       = (TransferStatus)resp[2],
      StateMachineStateValue          = resp[8], // Internal I2C state machine state value
      RequestedTransferLength         = resp[9] | (resp[10] << 8), // Lower/Higher byte of the requested I2C transfer length
      AlreadyTransferredLength        = resp[11] | (resp[12] << 8), // Lower/Higher byte of the already transferred number of bytes
      DataBufferCounter               = resp[13], // Internal I2C data buffer counter
      CommunicationSpeedDividerValue  = resp[14], // Current I2C communication speed divider value
      TimeoutValue                    = resp[15], // Current I2C time-out value
      Address                         = resp[16] /* | (resp[17] << 8) */, // Lower/Higher byte of the I2C address being used
      LineValueSCL                    = resp[22], // SCL line value as read from the pin
      LineValueSDA                    = resp[23], // SDA line value as read from the pin
      ReadPendingValue                = resp[25], // I2C Read pending value
#pragma warning restore IDE0055 // Fix formatting
    };

  public override string ToString()
    => string.Concat(
      "{",
      nameof(I2CEngineState), ": ",
      string.Join(
        ", ",
        new (string Name, object Value)[] {
          (nameof(StateMachineStateValue), $"0x{StateMachineStateValue:X2}"),
          (nameof(Address), $"0x{Address >> 1:X2}"),
          ("R/W", (Address & 0b1) == 0b1 ? "READ" : "WRITE"),
          (nameof(BusStatus), BusStatus),
          (nameof(RequestedTransferLength), RequestedTransferLength),
          (nameof(AlreadyTransferredLength), AlreadyTransferredLength),
          (nameof(ReadPendingValue), ReadPendingValue),
          (nameof(DataBufferCounter), DataBufferCounter),
          (nameof(CommunicationSpeedDividerValue), $"0x{CommunicationSpeedDividerValue:X2}"),
          (nameof(TimeoutValue), TimeoutValue),
          (nameof(LineValueSCL), LineValueSCL),
          (nameof(LineValueSDA), LineValueSDA),
        }.Select(pair => string.Concat(pair.Name, "=", pair.Value))
      ),
      "}"
    );
}
