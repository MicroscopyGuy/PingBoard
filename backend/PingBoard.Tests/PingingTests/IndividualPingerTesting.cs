namespace PingBoard.Tests.PingingTests;

using System.Net.NetworkInformation;
using Microsoft.Extensions.Logging.Abstractions;
using Probes.NetworkProbes.Ping;
using Probes.Services;
using Probes.Utilities;

public class IndividualPingerTesting
{
    [Fact]
    public async Task Can_Ping8_8_8_8_AndGetProperResult()
    {
        var target = new IpAddressTarget("8.8.8.8");
        var behavior = new PingProbeBehavior(target, 64, 500, "This is a test");
        var pinger = new IndividualPinger(new Ping(), NullLogger<IndividualPinger>.Instance);
        var result = await pinger.SendPingIndividualAsync(behavior, CancellationToken.None);

        Assert.NotNull(result.Address);
        Assert.NotNull(result.Status);
        Assert.NotEmpty(result.Address.ToString());
        Assert.NotEmpty(result.Status.ToString());
    }

    [Fact]
    public void Can_Get_String_Ip_From_IpAddressTarget_Type()
    {
        var target = new IpAddressTarget("8.8.8.8");
        Assert.Equal("8.8.8.8", target.ToString());
    }

    [Fact]
    public void Can_Get_String_Ip_From_HostnameTarget_Type()
    {
        var target = new HostnameTarget("8.8.8.8");
        Assert.Equal("8.8.8.8", target.ToString());
    }
}
