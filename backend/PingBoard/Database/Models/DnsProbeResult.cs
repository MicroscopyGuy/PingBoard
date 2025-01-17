using PingBoard.Probes.NetworkProbes;

namespace PingBoard.Database.Models;

using System.Net;
using System.Text.Json;
using PingBoard.Services;
using Probes.NetworkProbes.Common;

public record DnsProbeResult : ProbeResult
{
    /// <summary>
    ///  List of IPAddresses resolved by DNS
    /// </summary>
    public IPAddress[] IpAddresses { get; set; }

    /// <summary>
    /// List of Aliases associated with the DNS-resolved host, if any
    /// </summary>
    public string[] Aliases { get; set; }

    /// <summary>
    /// The name of the primary host resolved by DNS
    /// </summary>
    public string PrimaryHostName { get; set; }

    public string ToJson()
    {
        return JsonSerializer.Serialize(this);
    }
}
