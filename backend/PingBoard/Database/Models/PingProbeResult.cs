namespace PingBoard.Database.Models;

using System.Diagnostics.CodeAnalysis;
using System.Net.NetworkInformation;
using System.Text.Json;
using Google.Protobuf.WellKnownTypes;
using PingBoard.Probes.NetworkProbes;
using Probes.NetworkProbes.Common;
using Probes.NetworkProbes.Ping;
using Protos;

/// <summary>
/// Defines a class meant to encapsulate the values returned by the SendPingGroupAsync() function
/// in the <see cref="PingProbe"/> class.
/// </summary>
public record PingProbeResult : ProbeResult
{
    /// <summary>
    /// The return time of the ping
    /// </summary>
    public long Rtt { get; set; }

    /// <summary>
    /// Indicates which IPStatus was returned
    /// </summary>
    public IPStatus? IpStatus { get; set; }

    /// <summary>
    /// The ttl of the ping request
    /// </summary>
    public short Ttl { get; set; }

    /// <summary>
    /// The IP address of the machine which sent the reply
    /// </summary>
    public string ReplyAddress { get; set; }

    /*
    /// <summary>
    /// If the ping required a DNS lookup, the results will be stored here.
    /// </summary>
    public DnsProbeResult dnsProbeResult { get; set; }*/

    /// <summary>
    /// Safely initializes and returns a ProbeResult object
    /// </summary>
    /// <returns>a PingProbeResult object </returns>
    public PingProbeResult()
    {
        IpStatus = null;
    }

    [ExcludeFromCodeCoverage]
    public override string ToString()
    {
        return $"Rtt: {Rtt} IpStatus: {IpStatus.ToString()} "
            + $"EndTime: {End:MM:dd:yyyy:hh:mm:ss.ffff}"
            + $"Target: {Target.ToString()}"
            + $"ReplyAddress: ${(ReplyAddress is null ? "No reply address" : ReplyAddress.ToString())}";
    }

    public static PingResultPublic ToApiModel(PingProbeResult result)
    {
        return new PingResultPublic
        {
            Start = Timestamp.FromDateTime(DateTime.SpecifyKind(result.Start, DateTimeKind.Utc)),
            End = Timestamp.FromDateTime(DateTime.SpecifyKind(result.End, DateTimeKind.Utc)),
            Target = result.Target,
            IpStatus = result.IpStatus.ToString(),
            Ttl = result.Ttl,
            ReplyAddress = result.ReplyAddress,
        };
    }

    public static implicit operator PingResultPublic(PingProbeResult result) =>
        PingProbeResult.ToApiModel(result);

    public string ToJson()
    {
        return JsonSerializer.Serialize(this);
    }
}
