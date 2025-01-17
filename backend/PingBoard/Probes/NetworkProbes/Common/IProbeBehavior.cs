namespace PingBoard.Probes.NetworkProbes.Common;

public interface IProbeBehavior
{
    public INetworkProbeTarget Target { get; }

    public string GetTarget();
}
