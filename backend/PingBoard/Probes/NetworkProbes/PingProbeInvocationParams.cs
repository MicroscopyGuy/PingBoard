namespace PingBoard.Probes.NetworkProbes;

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
