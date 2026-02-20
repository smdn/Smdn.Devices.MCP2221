// SPDX-FileCopyrightText: 2026 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Smdn.Devices.UsbHid;

/// <summary>
/// Provides a mechanism to abstract reading from and writing to USB-HID report endpoints.
/// </summary>
public interface IUsbHidEndPoint : IDisposable, IAsyncDisposable {
  /// <summary>
  /// Gets the <see cref="IUsbHidDevice"/> that opened this <see cref="IUsbHidEndPoint"/>.
  /// </summary>
  IUsbHidDevice Device { get; }

  /// <summary>
  /// Gets a value indicating whether this endpoint can read reports.
  /// </summary>
  bool CanRead { get; }

  /// <summary>
  /// Gets a value indicating whether this endpoint can write reports.
  /// </summary>
  bool CanWrite { get; }

  /// <summary>
  /// Reads a report from the endpoint.
  /// </summary>
  /// <param name="buffer">
  /// A <see cref="Span{T}"/> to receive the report payload.
  /// The first byte will contain the Report ID, or will be zero if the
  /// underlying implementation does not use Report IDs.
  /// </param>
  /// <param name="cancellationToken">
  /// The <see cref="CancellationToken"/> to monitor for cancellation requests.
  /// </param>
  /// <returns>
  /// The number of bytes read from the report.
  /// </returns>
  /// <exception cref="ArgumentException">
  /// The length of <paramref name="buffer"/> is too short or too long.
  /// </exception>
  /// <exception cref="InvalidOperationException">
  /// The endpoint does not support reading.
  /// </exception>
  /// <exception cref="ObjectDisposedException">
  /// The endpoint has been disposed.
  /// </exception>
  /// <exception cref="OperationCanceledException">
  /// The <paramref name="cancellationToken"/> has had cancellation requested.
  /// </exception>
  int Read(Span<byte> buffer, CancellationToken cancellationToken);

  /// <summary>
  /// Asynchronously reads a report from the endpoint.
  /// </summary>
  /// <param name="buffer">
  /// A <see cref="Memory{T}"/> to receive the report payload.
  /// The first byte will contain the Report ID, or will be zero if the
  /// underlying implementation does not use Report IDs.
  /// </param>
  /// <param name="cancellationToken">
  /// The <see cref="CancellationToken"/> to monitor for cancellation requests.
  /// </param>
  /// <returns>
  /// A <see cref="ValueTask{TResult}"/> that represents the asynchronous operation.
  /// The result of the task is the number of bytes read from the report.
  /// </returns>
  /// <exception cref="ArgumentException">
  /// The length of <paramref name="buffer"/> is too short or too long.
  /// </exception>
  /// <exception cref="InvalidOperationException">
  /// The endpoint does not support reading.
  /// </exception>
  /// <exception cref="ObjectDisposedException">
  /// The endpoint has been disposed.
  /// </exception>
  /// <exception cref="OperationCanceledException">
  /// The <paramref name="cancellationToken"/> has had cancellation requested.
  /// </exception>
  /// <remarks>
  /// If the underlying implementation does not support asynchronous reading,
  /// this method will perform a synchronous read instead.
  /// </remarks>
  ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken);

  /// <summary>
  /// Writes a report to the endpoint.
  /// </summary>
  /// <param name="buffer">
  /// A <see cref="ReadOnlySpan{T}"/> that contains the report payload to send.
  /// The first byte of the buffer must be the Report ID, or zero if the
  /// underlying implementation does not use Report IDs.
  /// </param>
  /// <param name="cancellationToken">
  /// The <see cref="CancellationToken"/> to monitor for cancellation requests.
  /// </param>
  /// <exception cref="ArgumentException">
  /// The length of <paramref name="buffer"/> is too short or too long.
  /// </exception>
  /// <exception cref="InvalidOperationException">
  /// The endpoint does not support writing.
  /// </exception>
  /// <exception cref="ObjectDisposedException">
  /// The endpoint has been disposed.
  /// </exception>
  /// <exception cref="OperationCanceledException">
  /// The <paramref name="cancellationToken"/> has had cancellation requested.
  /// </exception>
  void Write(ReadOnlySpan<byte> buffer, CancellationToken cancellationToken);

  /// <summary>
  /// Asynchronously writes a report to the endpoint.
  /// </summary>
  /// <param name="buffer">
  /// A <see cref="ReadOnlyMemory{T}"/> that contains the report payload to send.
  /// The first byte of the buffer must be the Report ID, or zero if the
  /// underlying implementation does not use Report IDs.
  /// </param>
  /// <param name="cancellationToken">
  /// The <see cref="CancellationToken"/> to monitor for cancellation requests.
  /// </param>
  /// <returns>
  /// A <see cref="ValueTask"/> that represents the asynchronous operation.
  /// </returns>
  /// <exception cref="ArgumentException">
  /// The length of <paramref name="buffer"/> is too short or too long.
  /// </exception>
  /// <exception cref="InvalidOperationException">
  /// The endpoint does not support writing.
  /// </exception>
  /// <exception cref="ObjectDisposedException">
  /// The endpoint has been disposed.
  /// </exception>
  /// <exception cref="OperationCanceledException">
  /// The <paramref name="cancellationToken"/> has had cancellation requested.
  /// </exception>
  /// <remarks>
  /// If the underlying implementation does not support asynchronous writing,
  /// this method will perform a synchronous write instead.
  /// </remarks>
  ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken);
}
