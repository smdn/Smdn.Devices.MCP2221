// SPDX-FileCopyrightText: 2026 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Smdn.Devices.UsbHid;

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
  /// Requests reading of the HID report for the endpoint represented by this object.
  /// </summary>
  /// <param name="buffer">
  /// Specifies the <see cref="Span{T}"/> where the entire report payload will be read.
  /// The first byte contains the Report ID.
  /// </param>
  /// <param name="cancellationToken">
  /// The <see cref="CancellationToken"/> to monitor for cancellation requests.
  /// </param>
  /// <returns>
  /// Returns the length of the transferred payload.
  /// </returns>
  /// <exception cref="ArgumentException">
  /// The length of <paramref name="buffer"/> is too short or too long.
  /// </exception>
  /// <exception cref="ObjectDisposedException">
  /// Attempted to read report after the instance was disposed.
  /// </exception>
  int Read(Span<byte> buffer, CancellationToken cancellationToken);

  /// <summary>
  /// Requests asynchronous reading of HID reports for the endpoint represented by this object.
  /// </summary>
  /// <param name="buffer">
  /// Specifies the <see cref="Span{T}"/> where the entire report payload will be read.
  /// The first byte contains the Report ID.
  /// </param>
  /// <param name="cancellationToken">
  /// The <see cref="CancellationToken"/> to monitor for cancellation requests.
  /// </param>
  /// <returns>
  /// The <see cref="ValueTask{T}"/> that represents the asynchronous operation.
  /// The value of its <see cref="ValueTask{T}.Result"/> property contains length of the transferred payload.
  /// </returns>
  /// <exception cref="ArgumentException">
  /// The length of <paramref name="buffer"/> is too short or too long.
  /// </exception>
  /// <exception cref="ObjectDisposedException">
  /// Attempted to read report after the instance was disposed.
  /// </exception>
  /// <remarks>
  /// If the implementation does not support asynchronous reading,
  /// this method will perform a synchronous read instead.
  /// </remarks>
  ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken);

  /// <summary>
  /// Requests asynchronous writing of the HID report for the endpoint represented by this object.
  /// </summary>
  /// <param name="buffer">
  /// Specifies the <see cref="ReadOnlySpan{T}"/> where the entire report payload to be sent.
  /// The first byte contains the Report ID.
  /// </param>
  /// <param name="cancellationToken">
  /// The <see cref="CancellationToken"/> to monitor for cancellation requests.
  /// </param>
  /// <exception cref="ArgumentException">
  /// The length of <paramref name="buffer"/> is too short or too long.
  /// </exception>
  /// <exception cref="ObjectDisposedException">
  /// Attempted to write report after the instance was disposed.
  /// </exception>
  void Write(ReadOnlySpan<byte> buffer, CancellationToken cancellationToken);

  /// <summary>
  /// Requests writing of the HID report for the endpoint represented by this object.
  /// </summary>
  /// <param name="buffer">
  /// Specifies the <see cref="ReadOnlyMemory{T}"/> where the entire report payload to be sent.
  /// The first byte contains the Report ID.
  /// </param>
  /// <param name="cancellationToken">
  /// The <see cref="CancellationToken"/> to monitor for cancellation requests.
  /// </param>
  /// <returns>
  /// The <see cref="ValueTask"/> that represents the asynchronous operation.
  /// </returns>
  /// <exception cref="ArgumentException">
  /// The length of <paramref name="buffer"/> is too short or too long.
  /// </exception>
  /// <exception cref="ObjectDisposedException">
  /// Attempted to write report after the instance was disposed.
  /// </exception>
  /// <remarks>
  /// If the implementation does not support asynchronous writing,
  /// this method will perform a synchronous write instead.
  /// </remarks>
  ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken);
}
