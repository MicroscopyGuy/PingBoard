namespace PingBoard.Tests.ProbeTests;

using Microsoft.Extensions.DependencyInjection;
using Pinging;
using Probes.NetworkProbes;
using Probes.Services;

public class PingProbeTests
{
    [Fact]
    public async Task CanPingWithPingProbe()
    {
        var tokenSource = new CancellationTokenSource();
        var token = tokenSource.Token; // Renamed to 'token' to avoid conflicts
        var pinger = new IndividualPinger();

        PingProbeInvocationParams pingParams = new PingProbeInvocationParams(
            new HostnameTarget("127.0.0.1"),
            64,
            500,
            "This is a test"
        );

        var result = await probe.ProbeAsync(pingParams, token);
        Assert.NotNull(result);
    }
}
