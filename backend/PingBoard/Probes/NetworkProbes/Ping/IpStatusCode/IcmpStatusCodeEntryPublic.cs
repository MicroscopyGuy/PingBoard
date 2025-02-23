namespace PingBoard.Probes.NetworkProbes.Ping.IpStatusCode;

using System.Net.NetworkInformation;
using System.Text.Json.Serialization;

/// <summary>
/// A struct to store information related to a single IcmpStatusCode, to be used for later lookup and translation.
/// </summary>
public struct IcmpStatusCodeEntryPublic
{
    /// <summary>
    /// A remapping of an IPStatus enum to a new enum type for the purpose of separating two IpStatus enums
    /// which both had the same ordinal value.
    /// <seealso> IpStatusExtensions.cs </seealso>
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public IpStatusExtensions.DisambiguatedIpStatus IcmpStatusCode { get; set; }

    /// <summary>
    /// A brief description of the ICMP StatusCode, display in UI
    /// </summary>
    public string BriefDescription { get; set; } // brief, intended for DB/UI

    /// <summary>
    /// A more extensive description of the code, acquired from:
    /// <seealso href="https://learn.microsoft.com/en-us/dotnet/api/system.net.networkinformation.ipstatus?view=net-8.0">
    ///     System.Net.NetworkInformation.IPStatus documentation
    /// </seealso>
    /// </summary>
    public string ExtendedDescription { get; set; }
}
