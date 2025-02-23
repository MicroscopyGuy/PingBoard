namespace PingBoard.Probes.NetworkProbes.Common;

using System.Text.Json.Serialization;
using Ping;

[JsonPolymorphic]
[JsonDerivedType(typeof(PingProbeThresholds))]
public interface IProbeThresholds { }
