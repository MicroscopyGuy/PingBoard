namespace PingBoard.Probes.NetworkProbes;

using System.Text.Json.Serialization;
using Database.Models;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "Discriminator")]
[JsonDerivedType(typeof(PingProbeResult), typeDiscriminator: "PingProbeResult")]
[JsonDerivedType(typeof(DnsProbeResult), typeDiscriminator: "DnsProbeResult")]
//[JsonDerivedType(typeof(TracerouteProbeResult), typeDiscriminator: "PingProbeResult")]
public class ProbeResult
{
    /// <summary>
    /// Identifier, and primary key for ProbeResult records in the database.
    /// </summary>
    [JsonIgnore]
    public Guid Id { get; set; }

    /// <summary>
    /// The time the attempt to send the group of pings either started, or attempted to start
    /// </summary>
    [JsonIgnore]
    public DateTime Start { get; set; }

    /// <summary>
    /// The time the attempt to receive the group of pings ended
    /// </summary>
    [JsonIgnore]
    public DateTime End { get; set; }

    /// <summary>
    /// Wherever the user said to ping, could be either a domain or an IP address
    /// </summary>
    [JsonIgnore]
    public string Target { get; set; }

    /// <summary>
    /// Indicates whether the operation was successful.
    /// </summary>
    [JsonIgnore]
    public bool Success { get; set; }

    /// <summary>
    /// If the record indicates an anomaly based on the thresholds supplied by the user over the interval
    /// in which this networking operation was conducted.
    /// </summary>
    [JsonIgnore]
    public bool Anomaly { get; set; }

    /// <summary>
    /// The data retrieved from a particular probe operation which is not common to all probes. Ie, ping related
    /// information, or dns, or traceroute related info, etc.
    /// </summary>
    public string ProbeSubtypeData { get; set; }

    public ProbeResult()
    {
        Id = Guid.CreateVersion7();
        Start = DateTime.MinValue;
        End = DateTime.MaxValue;
        Target = null;
        Success = false;
        Anomaly = false;
        ProbeSubtypeData = null;
    }
}
