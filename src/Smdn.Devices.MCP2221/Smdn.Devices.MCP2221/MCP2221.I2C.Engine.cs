// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT

using System;
using System.Collections.Generic;

using Microsoft.Extensions.Logging;

namespace Smdn.Devices.MCP2221;

#pragma warning disable IDE0040
partial class MCP2221 {
  partial class I2CFunctionality {
#pragma warning restore IDE0040
    private static Exception CreateUnexpectedResponseException(I2CAddress? address, byte response)
      => address == null
        ? new CommandException($"unexpected response (0x{response:X2})")
        : new I2CCommandException(address.Value, $"unexpected response (0x{response:X2})");
    private static Exception CreateI2CErrorException(I2CAddress address, byte? stateValue, string message, string i2cEngineState = null)
      => new I2CCommandException(address, $"{message} (0x{stateValue?.ToString("X2") ?? "??"}, {i2cEngineState ?? "(details not available)"})");
    private static Exception CreateUnknownEngineStateException(I2CAddress address, byte? stateValue, string i2cEngineState = null)
      => new I2CCommandException(address, $"unknown I2C engine state (0x{stateValue?.ToString("X2") ?? "??"}, {i2cEngineState ?? "(details not available)"})");

    private enum OperationState {
      Initial,
      CancelAndRetry,
      Continue,
      AdvanceToNextStep,
    }

    private class OperationContext {
      public OperationContext(ILogger logger, I2CBusSpeed busSpeed)
      {
        this.logger = logger;
        this.busSpeed = busSpeed;
      }

      private readonly ILogger logger;
      private readonly I2CBusSpeed busSpeed;
      private OperationState operationState;
      private I2CEngineState lastEngineState;
      public int ReadLength { get; private set; } = -1;

#pragma warning disable 0164
      public IEnumerable<(
        ConstructCommandAction<(I2CAddress, Memory<byte>)> constructCommand,
        ParseResponseFunc<(I2CAddress, Memory<byte>), bool> parseResponse
      )>
      IterateWriteCommands()
      {
        operationState = OperationState.Initial;

      WRITE_INIT:
        logger?.LogDebug(I2CFunctionality.eventIdI2CCommand, "WRITE_INIT");

        yield return (
          StatusConstructCommand,
          StatusParseResponse
        );

        if (operationState == OperationState.CancelAndRetry)
          goto WRITE_INIT;

      WRITE_DO:
        logger?.LogDebug(I2CFunctionality.eventIdI2CCommand, "WRITE_DO");

        yield return (
          WriteConstructCommand,
          WriteParseResponse
        );

      WRITE_STATUS:
        logger?.LogDebug(I2CFunctionality.eventIdI2CCommand, "WRITE_STATUS");

        yield return (
          StatusConstructCommand,
          StatusParseResponse
        );

        if (operationState == OperationState.Continue)
          goto WRITE_STATUS;
        if (lastEngineState.RequestedTransferLength == 0)
          yield break;
      }

      public IEnumerable<(
        ConstructCommandAction<(I2CAddress, Memory<byte>)> constructCommand,
        ParseResponseFunc<(I2CAddress, Memory<byte>), bool> parseResponse
      )>
      IterateReadCommands()
      {
        operationState = OperationState.Initial;
        ReadLength = -1;

      READ_INIT:
        logger?.LogDebug(I2CFunctionality.eventIdI2CCommand, "READ_INIT");

        yield return (
          StatusConstructCommand,
          StatusParseResponse
        );

        if (operationState == OperationState.CancelAndRetry)
          goto READ_INIT;

      READ_DO:
        logger?.LogDebug(I2CFunctionality.eventIdI2CCommand, "READ_DO");

        yield return (
          ReadConstructCommand,
          ReadParseResponse
        );

#if false
        if (lastEngineState.RequestedTransferLength == 0)
          yield break; // no need to do READ_GET
        if (canAdvanceToNextStep)
          goto READ_GET;
#endif

      READ_GET:
        yield return (
          GetConstructCommand,
          GetParseResponse
        );

        if (operationState == OperationState.Continue)
          goto READ_GET;

        yield break;
      }
#pragma warning restore 0164

      private static OperationState TransitStateOrThrowIfEngineStateInvalid(OperationState currentState, I2CAddress address, I2CEngineState engineState)
      {
        if (currentState == OperationState.Initial && (engineState.LineValueSCL.IsLow() || engineState.LineValueSDA.IsLow()))
          throw CreateI2CErrorException(address, engineState.StateMachineStateValue, "The line level of SDA and/or SCL is invalid. Try pull-up the bus lines. It may need to be reset or powered off.", engineState.ToString());

        if (engineState.BusStatus == I2CEngineState.TransferStatus.MarkedForCancellation)
          throw CreateI2CErrorException(address, engineState.StateMachineStateValue, "I2C engine has been marked for cancellation unexpectedly. It may need to be reset or powered off.", engineState.ToString());

        return engineState.StateMachineStateValue switch {
          /*
           * success / can advance
           */
          0x00 => OperationState.AdvanceToNextStep, // completed successfully?
          // 0x10: ACK? transferring?

          0x55 => OperationState.AdvanceToNextStep, // ACK? transferring?
          0x60 => OperationState.AdvanceToNextStep, // all buffer transferred?

          /*
           * still in progress / NACK reply
           */
          // 0x25: write operation still in progress?
          0x25 when currentState == OperationState.Initial => OperationState.CancelAndRetry, // remains previous operation state(?)
          0x25 when 0 < engineState.TimeoutValue => OperationState.Continue, // current operation in progress
          0x25 => throw new I2CNAckException(address), // time out

          // 0x61: read operation still in progress?
          // 0x61 when (currentState == OperationState.Initial) => OperationState.CancelAndRetry, // issuing cancellation in this state will trasit state to 0x62, and will be in state which cannot reset with command
          0x61 when 0 < engineState.TimeoutValue => OperationState.Continue, // current operation in progress
          // 0x62: has been marked for cancellation?
          0x62 => throw CreateI2CErrorException(address, engineState.StateMachineStateValue, "I2C engine has been in invalid state. It may need to be reset or powered off.", engineState.ToString()),

          /*
           * exceptional / unknown states
           */
          _ => throw CreateUnknownEngineStateException(address, engineState.StateMachineStateValue, engineState.ToString()),
        };
      }

      private void StatusConstructCommand(Span<byte> comm, ReadOnlySpan<byte> userData, (I2CAddress address, Memory<byte> _) args)
      {
        // [MCP2221A] 3.1.1 STATUS/SET PARAMATERS
        comm[0] = 0x10; // Status/Set Parameters
        comm[1] = 0x00; // Don't care
        comm[2] = 0x00; // Cancel current I2C/SMBus transfer (0x00: No effect)

        if (operationState == OperationState.Initial) {
          comm[3] = 0x20; // Set I2C/SMBus communication speed
          comm[4] = busSpeed switch {
            I2CBusSpeed.Speed10kBitsPerSec => 0xAD,
            I2CBusSpeed.Speed100kBitsPerSec => 0x75,
            I2CBusSpeed.Speed400kBitsPerSec => 0x1B,
            _ => 0x75, // as default (or should throw InvalidEnumValueException?)
          };
        }
        else if (operationState == OperationState.CancelAndRetry) {
          comm[2] = 0x10; // Cancel current I2C/SMBus transfer (0x10: Cancel transfer)
        }
      }

      private bool StatusParseResponse(ReadOnlySpan<byte> resp, (I2CAddress address, Memory<byte> _) args)
      {
        // [MCP2221A] 3.1.1 STATUS/SET PARAMATERS
        _ = resp[1] switch {
          0x00 => true, // Command completed successfully
          _ => throw CreateUnexpectedResponseException(args.address, resp[1]),
        };

        lastEngineState = I2CEngineState.Parse(resp);

        logger?.LogInformation(eventIdI2CEngineState, $"STATUS/SET PARAMATERS: {lastEngineState}");

        if (operationState == OperationState.Initial) {
          var isSpeedConsidered = resp[3] switch {
            0x00 => false, // No Set I2C/SMBus communication speed was issued
            0x20 => true, // The new I2C/SMBus communication speed is now considered
            // 0x21 => throw; // I2C transfer in progress
            _ => false, // throw
          };

          if (!isSpeedConsidered)
            logger?.LogWarning(eventIdI2CEngineState, $"STATUS/SET PARAMATERS: new I2C/SMBus communication speed is not considered");
        }

        operationState = TransitStateOrThrowIfEngineStateInvalid(
          operationState,
          args.address,
          lastEngineState
        );

        return operationState == OperationState.AdvanceToNextStep;
      }

      private void WriteConstructCommand(Span<byte> comm, ReadOnlySpan<byte> userData, (I2CAddress address, Memory<byte> _) args)
      {
        // [MCP2221A] 3.1.5 I2C WRITE DATA
        comm[0] = 0x90; // I2C Write Data
        comm[1] = (byte)(userData.Length & 0x00FF); // Requested I2C transfer length - low byte
        comm[2] = (byte)(userData.Length >> 8); // Requested I2C transfer length - high byte
        comm[3] = args.address.GetWriteAddress(); // I2C slave address to communicate with
        userData.CopyTo(comm.Slice(4));
      }

      private bool WriteParseResponse(ReadOnlySpan<byte> resp, (I2CAddress address, Memory<byte> _) args)
      {
        // [MCP2221A] 3.1.5 I2C WRITE DATA
        operationState = resp[1] switch {
          0x00 => OperationState.AdvanceToNextStep, // Command completed successfully
          0x01 => OperationState.Continue, // Command not completed (I2C engine is busy)
          _ => throw CreateUnexpectedResponseException(args.address, resp[1]),
        };

        return operationState == OperationState.AdvanceToNextStep;
      }

      private void ReadConstructCommand(Span<byte> comm, ReadOnlySpan<byte> userData, (I2CAddress address, Memory<byte> _) args)
      {
        // [MCP2221A] 3.1.8 I2C READ DATA
        comm[0] = 0x91; // I2C Read Data
        comm[1] = (byte)(userData.Length & 0x00FF); // Requested I2C transfer length - low byte
        comm[2] = (byte)(userData.Length >> 8); // Requested I2C transfer length - high byte
        comm[3] = args.address.GetReadAddress(); // I2C slave address to communicate with
      }

      private bool ReadParseResponse(ReadOnlySpan<byte> resp, (I2CAddress address, Memory<byte> _) args)
      {
        // [MCP2221A] 3.1.8 I2C READ DATA
        operationState = resp[1] switch {
          0x00 => OperationState.AdvanceToNextStep, // Command completed successfully
          0x01 => OperationState.Continue, // Command not completed (I2C engine is busy)
          _ => throw CreateUnexpectedResponseException(args.address, resp[1]),
        };

        return operationState == OperationState.AdvanceToNextStep;
      }

      private void GetConstructCommand(Span<byte> comm, ReadOnlySpan<byte> userData, (I2CAddress address, Memory<byte> _) args)
      {
        // [MCP2221A] 3.1.10 I2C READ DATA - GET I2C DATA
        comm[0] = 0x40; // I2C Read Data - Get I2C Data
        comm[1] = (byte)(userData.Length & 0x00FF); // [??] Requested I2C transfer length - low byte
        comm[2] = (byte)(userData.Length >> 8); // [??] Requested I2C transfer length - high byte
      }

      private bool GetParseResponse(ReadOnlySpan<byte> resp, (I2CAddress address, Memory<byte> buffer) args)
      {
        // [MCP2221A] 3.1.10 I2C READ DATA - GET I2C DATA
        operationState = resp[1] switch {
          0x00 => OperationState.AdvanceToNextStep, // Command completed successfully
          0x01 => OperationState.Continue, // Command not completed (I2C engine is busy)
          0x41 => throw new I2CReadException(args.address, "can not read from I2C slave"), // Error reading the I2C slave data
          _ => throw CreateUnexpectedResponseException(args.address, resp[1]),
        };

        if (operationState == OperationState.AdvanceToNextStep) {
          ReadLength = resp[3] switch {
            _ when resp[3] is >= 0 and <= 60 => resp[3],
            127 => throw new I2CCommandException("error has orccurred on reading"),
            _ => throw new I2CCommandException(args.address, $"unexpected data length ({resp[3]})"),
          };

          resp.Slice(4, ReadLength).CopyTo(args.buffer.Span);
        }

        return operationState == OperationState.AdvanceToNextStep;
      }
    }
  }
}
