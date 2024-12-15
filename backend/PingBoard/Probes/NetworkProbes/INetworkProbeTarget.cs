namespace PingBoard.Probes.NetworkProbes;
/// <summary>
/// Represents the desired target of a Network Probe.
/// </summary>
public interface INetworkProbeTarget<T>
{
    string Name { get; }
    public Type TargetType { get; }
    public object Target { get; }
    public static abstract bool TryParse(string stringifiedTarget, out T? target);
    //byte[] ToProtobufBytes();
    //static abstract bool Validate();
}