namespace PingBoard.Probes.NetworkProbes;

public interface IProbeBehavior
{
    public INetworkProbeTarget Target { get; }

    public string GetTarget();
}
