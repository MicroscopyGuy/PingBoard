namespace PingBoard.Services;
/// <summary>
/// Represents the desired target of a Network Probe.
/// At the time of writing, it could be an IpAddress (v4 or v6) or a Hostname.
/// </summary>
public interface INetworkProbeTarget
{
    string Name { get; }
    public Type TargetType { get; }
    public object Target { get; }
    //byte[] ToProtobufBytes();
    //static abstract bool Validate();
}