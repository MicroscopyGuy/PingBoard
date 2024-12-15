namespace PingBoard.Probes.NetworkProbes;

public record PingProbeConfiguration(IpOrHostnameTarget Target, int Ttl, TimeSpan TimeoutMs)
    : IProbeConfiguration
{
    public string GetTarget()
    {
        return Target.ToString()!;
    }
}
