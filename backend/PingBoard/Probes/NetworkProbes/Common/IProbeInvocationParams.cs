namespace PingBoard.Probes.NetworkProbes;

public interface IProbeInvocationParams
{
    public INetworkProbeTarget Target { get; }

    public IProbeInvocationThresholds Thresholds { get; }
    public string GetTarget();
}
