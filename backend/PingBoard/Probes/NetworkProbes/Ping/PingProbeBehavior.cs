namespace PingBoard.Probes.NetworkProbes.Ping;

using PingBoard.Probes.NetworkProbes.Common;

public record PingProbeBehavior(
    INetworkProbeTarget Target,
    int Ttl,
    int TimeoutMs,
    string PacketPayload
) : IProbeBehavior
{
    public string GetTarget()
    {
        return Target.ToString()!;
    }
}
