namespace PingBoard.Probes.NetworkProbes.Common;

using System.Text.Json.Serialization;
using PingBoard.Probes.Utilities;
using Services;

/// <summary>
/// Represents the desired target of a Network Probe.
/// </summary>
[JsonPolymorphic(TypeDiscriminatorPropertyName = "targetType")]
[JsonDerivedType(typeof(IpAddressTarget), typeDiscriminator: "ipAddress")]
[JsonDerivedType(typeof(HostnameTarget), typeDiscriminator: "hostname")]
public interface INetworkProbeTarget { }
