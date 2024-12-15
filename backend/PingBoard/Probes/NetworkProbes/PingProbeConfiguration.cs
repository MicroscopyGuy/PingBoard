namespace PingBoard.Probes.NetworkProbes;

public record class PingProbeConfiguration(INetworkProbeTarget<IpOrHostnameTarget> Target,
    int Ttl, TimeSpan TimeoutMs)
{
    private readonly INetworkProbeTarget<IpOrHostnameTarget> _target = Target;
    private readonly int _ttl = Ttl;
    private readonly TimeSpan _timeoutMs = TimeoutMs;

    public INetworkProbeTarget<IpOrHostnameTarget> Target
    {
        get => _target;
        init => _target = value;
    }

    public int Ttl
    {
        get => _ttl;
        init => _ttl = value;
    }

    public TimeSpan TimeoutMs
    {
        get => _timeoutMs;
        init => _timeoutMs = value;
    }
}
