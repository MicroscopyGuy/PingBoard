using System.Net.NetworkInformation;
using PingBoard.Pinging;

namespace PingBoard.Tests.PingingTests;

using System.Net;
using Newtonsoft.Json;

public class ICMPStatusCodeLookupTesting
{
    [Theory]
    [MemberData(
        nameof(ICMPStatusCodeLookupTestGenerator.GetTestData),
        MemberType = typeof(ICMPStatusCodeLookupTestGenerator)
    )]
    public void AllIPStatusEnums_CanBeLookedUp(IPStatus status, IPAddress response)
    {
        var lookupInfo = status.GetInfo(response);

        var expectedStrEnum =
            status == (IPStatus)11004
                ? response.IsIPv4()
                    ? nameof(IPStatus.DestinationProtocolUnreachable)
                    : nameof(IPStatus.DestinationProhibited)
                : status.ToString();

        Assert.Equal(expectedStrEnum, lookupInfo.IcmpStatusCode.ToString());
        Assert.IsType<PingingStates.PingState>(lookupInfo.State);
        Assert.NotEqual("", lookupInfo.BriefDescription);
        Assert.NotEqual("", lookupInfo.ExtendedDescription);
    }
}
