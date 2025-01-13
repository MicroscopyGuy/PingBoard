namespace PingBoard.Tests.PingingTests;

using System.Net;
using System.Net.NetworkInformation;

public class ICMPStatusCodeLookupTestGenerator
{
    public static IEnumerable<object[]> GetTestData()
    {
        var ipStatusMappings = new (IPStatus, IPAddress)[]
        {
            (IPStatus.Unknown, IPAddress.Parse("2001:db8::9")),
            (IPStatus.Success, IPAddress.Parse("192.168.1.1")),
            (IPStatus.DestinationNetworkUnreachable, IPAddress.Parse("2001:db8::1")),
            (IPStatus.DestinationHostUnreachable, IPAddress.Parse("192.168.2.1")),
            (IPStatus.DestinationProhibited, IPAddress.Parse("fef0::7")), // must be Ipv6
            (IPStatus.DestinationProtocolUnreachable, IPAddress.Parse("203.0.113.1")), // must be Ipv4
            (IPStatus.DestinationPortUnreachable, IPAddress.Parse("fe80::2")),
            (IPStatus.NoResources, IPAddress.Parse("198.51.100.1")),
            (IPStatus.BadOption, IPAddress.Parse("2001:db8::2")),
            (IPStatus.HardwareError, IPAddress.Parse("192.0.2.1")),
            (IPStatus.PacketTooBig, IPAddress.Parse("2001:db8::3")),
            (IPStatus.TimedOut, IPAddress.Parse("203.0.113.2")),
            (IPStatus.BadRoute, IPAddress.Parse("2001:db8::4")),
            (IPStatus.TtlExpired, IPAddress.Parse("192.168.0.1")),
            (IPStatus.TtlReassemblyTimeExceeded, IPAddress.Parse("2001:db8::5")),
            (IPStatus.ParameterProblem, IPAddress.Parse("198.51.100.2")),
            (IPStatus.SourceQuench, IPAddress.Parse("2001:db8::6")),
            (IPStatus.BadDestination, IPAddress.Parse("2001:db8::5")),
            (IPStatus.DestinationUnreachable, IPAddress.Parse("2001:0db8:85a3::8a2e:0370:7334")),
            (IPStatus.TimeExceeded, IPAddress.Parse("192.0.2.2")),
            (IPStatus.BadHeader, IPAddress.Parse("2001:db8::7")),
            (IPStatus.UnrecognizedNextHeader, IPAddress.Parse("192.168.1.2")),
            (IPStatus.IcmpError, IPAddress.Parse("2001:db8::8")),
            (IPStatus.DestinationScopeMismatch, IPAddress.Parse("192.168.3.1")),
        };

        foreach (var testData in ipStatusMappings)
        {
            yield return new object[] { testData.Item1, testData.Item2 };
        }
    }
}
