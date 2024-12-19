namespace PingBoard.Probes.NetworkProbes;

using System.Text.Json.Serialization;

public record PingProbeInvocationParams(
    INetworkProbeTarget Target,
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
