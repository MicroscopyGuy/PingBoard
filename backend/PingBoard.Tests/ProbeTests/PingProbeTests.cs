namespace PingBoard.Tests.ProbeTests;

using System.Net;
using System.Net.NetworkInformation;
using Database.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Probes.NetworkProbes;
using Probes.NetworkProbes.Ping;
using Probes.Services;
using Probes.Utilities;
using static TestUtilities.PingingTestingUtilities.IndividualPingerStub;

public class PingProbeTests
{
    private static PingProbe _probe;
    private PingProbeBehavior _behaviorParam = new PingProbeBehavior(
        new HostnameTarget("127.0.0.1"),
        64,
        500,
        "This is a test"
    );

    public PingProbeTests()
    {
        var tokenSource = new CancellationTokenSource();
        var token = tokenSource.Token; // Renamed to 'token' to avoid conflicts
        var pinger = new IndividualPinger(new Ping(), new NullLogger<IndividualPinger>());
        var probe = new PingProbe(pinger, NullLogger<PingProbe>.Instance);
        _probe = probe;
    }

    [Fact]
    public async Task CanPingWithPingProbe_WhenGivenIpAddressTarget()
    {
        var tokenSource = new CancellationTokenSource();
        var token = tokenSource.Token; // Renamed to 'token' to avoid conflicts
        var pinger = new IndividualPinger(new Ping(), new NullLogger<IndividualPinger>());

        var probe = new PingProbe(pinger, NullLogger<PingProbe>.Instance);
        var result = await probe.ProbeAsync(_behaviorParam, token);
        Assert.NotNull(result);
    }

    public async Task CanPingWithPingProbe_WhenGivenHostnameTarget()
    {
        var tokenSource = new CancellationTokenSource();
        var token = tokenSource.Token; // Renamed to 'token' to avoid conflicts
        var pinger = new IndividualPinger(new Ping(), new NullLogger<IndividualPinger>());

        var probe = new PingProbe(pinger, NullLogger<PingProbe>.Instance);
        var result = await probe.ProbeAsync(_behaviorParam, token);
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

    [Fact]
    public async Task PingProbe_CanPingGoogle_AndRetrieveProperResult()
    {
        var tokenSource = new CancellationTokenSource();
        var token = tokenSource.Token; // Renamed to 'token' to avoid conflicts
        var pinger = new IndividualPinger(new Ping(), new NullLogger<IndividualPinger>());

        var probe = new PingProbe(pinger, NullLogger<PingProbe>.Instance);
        var behaviorParam = new PingProbeBehavior(
            new HostnameTarget("google.com"),
            64,
            1000,
            "https://github.com/MicroscopyGuy/PingBoard"
        );
        var result = await probe.ProbeAsync(_behaviorParam, token);
        Assert.NotNull(result);
        Assert.NotNull(result.ReplyAddress);
        Assert.NotNull(result.IpStatus);
        Assert.NotNull(result.Rtt);
        Assert.NotNull(result.Id);
        Assert.NotNull(result.Anomaly);
        Assert.NotNull(result.End);
        Assert.NotNull(result.Start);
        Assert.NotNull(result.Success);
        Assert.NotNull(result.Target);
        Assert.NotNull(result.ProbeType);
    }
}
