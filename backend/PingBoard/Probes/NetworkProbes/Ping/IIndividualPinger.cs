namespace PingBoard.Probes.NetworkProbes.Ping;

using System.Net;
using System.Net.NetworkInformation;
using Probes.NetworkProbes;

public interface IIndividualPinger
{
    public Task<PingReply> SendPingIndividualAsync(
        PingProbeInvocationParams pingParams,
        CancellationToken stoppingToken = default(CancellationToken)
    );
    // public int GetTtl();
    // public void SetTtl(int newTtl);
    // public int GetTimeout();
    // public void SetTimeout(int newTimeoutMs);
}
