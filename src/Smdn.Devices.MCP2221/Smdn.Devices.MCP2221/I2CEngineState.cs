// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT

using System;
using System.Linq;

namespace Smdn.Devices.MCP2221 {
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
    public GPIOValue LineValueSCL {
      get;
#if NET5_0_OR_GREATER
      init;
#else
      private set;
#endif
    }
    public GPIOValue LineValueSDA {
      get;
#if NET5_0_OR_GREATER
      init;
#else
      private set;
#endif
    }
    public static I2CEngineState Parse(ReadOnlySpan<byte> resp)
      => new I2CEngineState() {
        // [MCP2221A] 3.1.1 STATUS/SET PARAMATERS
        BusStatus                       = (TransferStatus)resp[2],
        StateMachineStateValue          = resp[8], // Internal I2C state machine state value
        RequestedTransferLength         = (int)(resp[ 9] | (resp[10] << 8)), // Lower/Higher byte of the requested I2C transfer length
        AlreadyTransferredLength        = (int)(resp[11] | (resp[12] << 8)), // Lower/Higher byte of the already transferred number of bytes
        DataBufferCounter               = resp[13], // Internal I2C data buffer counter
        CommunicationSpeedDividerValue  = resp[14], // Current I2C communication speed divider value
        TimeoutValue                    = resp[15], // Current I2C time-out value
        Address                         = (ushort)(resp[16] /* | (resp[17] << 8) */), // Lower/Higher byte of the I2C address being used
        LineValueSCL                    = (GPIOValue)resp[22], // SCL line value as read from the pin
        LineValueSDA                    = (GPIOValue)resp[23], // SDA line value as read from the pin
        ReadPendingValue                = (int)resp[25], // I2C Read pending value
      };

    public override string ToString()
      => string.Concat(
        "{",
        nameof(I2CEngineState), ": ",
        string.Join(", ",
          new[] {
            (nameof(StateMachineStateValue), (object)$"0x{StateMachineStateValue:X2}"),
            (nameof(Address), (object)$"0x{Address >> 1:X2}"),
            ("R/W", (object)((Address & 0b1) == 0b1 ? "READ" : "WRITE")),
            (nameof(BusStatus), (object)BusStatus),
            (nameof(RequestedTransferLength), (object)RequestedTransferLength),
            (nameof(AlreadyTransferredLength), (object)AlreadyTransferredLength),
            (nameof(ReadPendingValue), (object)ReadPendingValue),
            (nameof(DataBufferCounter), (object)DataBufferCounter),
            (nameof(CommunicationSpeedDividerValue), (object)$"0x{CommunicationSpeedDividerValue:X2}"),
            (nameof(TimeoutValue), (object)TimeoutValue),
            (nameof(LineValueSCL), (object)LineValueSCL),
            (nameof(LineValueSDA), (object)LineValueSDA),
          }.Select(pair => string.Concat(pair.Item1, "=", pair.Item2))
        ),
        "}"
      );
  }
}
