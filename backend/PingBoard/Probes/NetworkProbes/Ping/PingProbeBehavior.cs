namespace PingBoard.Probes.NetworkProbes.Ping;

using System.Text.Json.Serialization;

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
