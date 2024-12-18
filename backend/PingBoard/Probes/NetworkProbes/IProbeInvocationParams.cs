namespace PingBoard.Probes.NetworkProbes;

public interface IProbeInvocationParams
{
    public INetworkProbeTarget Target { get; }
    public string GetTarget();
}
