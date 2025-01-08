namespace PingBoard.Probes.NetworkProbes.Ping;

using System.Text.Json.Serialization;

public record PingProbeInvocationParams(
    INetworkProbeTarget Target,
    IProbeInvocationThresholds Thresholds,
    int Ttl,
    int TimeoutMs,
    string PacketPayload
) : IProbeInvocationParams
{
    public string GetTarget()
    {
        return Target.ToString()!;
    }
}
