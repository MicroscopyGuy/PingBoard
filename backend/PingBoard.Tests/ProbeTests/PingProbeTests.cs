namespace PingBoard.Tests.ProbeTests;

using System.Net;
using System.Net.NetworkInformation;
using Database.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Pinging;
using Probes.NetworkProbes;
using Probes.Services;
using static TestUtilities.PingingTestingUtilities.IndividualPingerStub;

public class PingProbeTests
{
    private static PingProbe _probe;

    public PingProbeTests()
    {
        var tokenSource = new CancellationTokenSource();
        var token = tokenSource.Token; // Renamed to 'token' to avoid conflicts
        var pinger = new IndividualPinger(new Ping(), new NullLogger<IndividualPinger>());
        var probe = new PingProbe(pinger);
        _probe = probe;
    }

    [Fact]
    public async Task CanPingWithPingProbe_WhenGivenIpAddressTarget()
    {
        var tokenSource = new CancellationTokenSource();
        var token = tokenSource.Token; // Renamed to 'token' to avoid conflicts
        var pinger = new IndividualPinger(new Ping(), new NullLogger<IndividualPinger>());

        PingProbeInvocationParams pingParams = new PingProbeInvocationParams(
            new HostnameTarget("127.0.0.1"),
            64,
            500,
            "This is a test"
        );

        var probe = new PingProbe(pinger);
        var result = await probe.ProbeAsync(pingParams, token);
        Assert.NotNull(result);
    }

    public async Task CanPingWithPingProbe_WhenGivenHostnameTarget()
    {
        var tokenSource = new CancellationTokenSource();
        var token = tokenSource.Token; // Renamed to 'token' to avoid conflicts
        var pinger = new IndividualPinger(new Ping(), new NullLogger<IndividualPinger>());

        PingProbeInvocationParams pingParams = new PingProbeInvocationParams(
            new HostnameTarget("localhost"),
            64,
            500,
            "This is a test"
        );

        var probe = new PingProbe(pinger);
        var result = await probe.ProbeAsync(pingParams, token);
        Assert.NotNull(result);
    }

    [Fact]
    public void ShouldContinue_ReturnsFalse_OnResultWithHaltingIpStatus()
    {
        var pingProbeResult = new PingProbeResult();
        pingProbeResult.ReplyAddress = "127.0.0.1";

        pingProbeResult.IpStatus = IPStatus.Unknown;
        Assert.False(_probe.ShouldContinue(pingProbeResult));
    }

    [Fact]
    public void ShouldContinue_ReturnsTrue_OnResultWithNonHaltingIpStatus()
    {
        var pingProbeResult = new PingProbeResult();
        pingProbeResult.ReplyAddress = "127.0.0.1";
        pingProbeResult.IpStatus = IPStatus.Success;
        Assert.True(_probe.ShouldContinue(pingProbeResult));
    }
}
