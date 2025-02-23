namespace PingBoard.Probes.NetworkProbes.Common;

using System.Text.Json.Serialization;
using Ping;

[JsonPolymorphic]
[JsonDerivedType(typeof(PingProbeBehavior))]
public interface IProbeBehavior
{
    public INetworkProbeTarget Target { get; }
    public string GetTarget();
}
