// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT

using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using NUnit.Framework;

using Smdn.IO.UsbHid;

namespace Smdn.Devices.MCP2221;

partial class MCP2221Tests {
  [TestFixture]
  public class I2CFunctionality {
    private readonly I2CAddress address = new I2CAddress(0x20);

    private static byte[] ToByteArray(string hexByteSequence)
      => hexByteSequence.Split('-').Select(hex => Convert.ToByte(hex, 16)).ToArray();

    private static void AppendResponse(PseudoUsbHidEndPoint endpoint, params string[] responseSequences)
    {
      if (!endpoint.CanRead)
        throw new InvalidOperationException("endpoint does not support reading");

      var currentPosition = endpoint.ReadStream!.Position;

      foreach (var sequence in responseSequences) {
        endpoint.ReadStream.WriteByte(ReportInput);
        endpoint.ReadStream.Write(ToByteArray(sequence));
      }

      endpoint.ReadStream.Position = currentPosition;
    }

    private static async Task<(MCP2221, PseudoUsbHidEndPoint)> CreatePseudoDeviceWithConfiguredI2C()
    {
      var baseDevice = CreatePseudoDevice();
      var device = await MCP2221.CreateAsync(baseDevice, shouldDisposeUsbHidDevice: true);

      AppendResponse(
        baseDevice.EndPoint,
        "10-00-00-20-75-00-00-00-00-03-00-03-00-03-75-00-00-00-10-28-00-60-01-01-00-00-F1-19-F0-00-00-00-30-30-0B-30-10-23-13-71-05-00-00-26-94-14-41-36-31-32-FB-03-00-00-00-00-F4-02-76-02-00-00-00-00"
      );

      return (device, baseDevice.EndPoint);
    }

    [Test] public async Task Write() => await TestWrite(d => { d.I2C.Write(address, new byte[] {0x00, 0x00, 0x00}); return Task.CompletedTask; });

    [Test] public async Task WriteAsync() => await TestWrite(async d => { await d.I2C.WriteAsync(address, new byte[] {0x00, 0x00, 0x00}); });

    private async Task TestWrite(Func<MCP2221, Task> writeAction)
    {
      var (device, stream) = await CreatePseudoDeviceWithConfiguredI2C();

      AppendResponse(
        stream,
        "10-00-00-20-75-00-00-00-00-03-00-03-00-03-75-00-00-00-10-28-00-60-01-01-00-00-F1-19-F0-00-00-00-30-30-0B-30-10-23-13-71-05-00-00-26-94-14-41-36-31-32-FB-03-00-00-00-00-F4-02-76-02-00-00-00-00",
        "90-00-10-20-75-00-00-00-00-03-00-03-00-03-75-00-00-00-10-28-00-60-01-01-00-00-F1-19-F0-00-00-00-30-30-0B-30-10-23-13-71-05-00-00-26-94-14-41-36-31-32-FB-03-00-00-00-00-F4-02-76-02-00-00-00-00",
        $"10-00-00-00-00-00-00-00-00-01-00-01-00-01-75-00-{address}-00-10-28-00-60-01-01-00-00-F1-79-F0-00-00-00-30-30-0B-30-10-23-13-79-05-00-00-26-94-14-41-36-31-32-FB-03-00-00-00-00-F5-02-59-02-00-00-00-00"
      );

      Assert.That(async () => await writeAction(device), Throws.Nothing);
    }
  }
}
