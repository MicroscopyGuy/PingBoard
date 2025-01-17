namespace PingBoard.Probes.NetworkProbes.Ping;

using System.Net;
using System.Net.NetworkInformation;

/// <summary>
/// Provides extensions to the IPStatus type in the System.Net.NetworkInformation library
/// </summary>
public static class IpStatusExtensions
{
    /// <summary>
    /// Both the DestinationProhibited and DestinationProtocolUnreachable IPStatus enums have the same ordinal value (11004).
    /// This causes a System.TypeInitializationException when deserializing this data into StatusCodes (IImmutableDictionary)
    /// from the ICMPStatusCodes.json file since the each IPStatus' ordinal value is checked for uniqueness.
    ///
    /// The DisambiguatedIpStatus enums below are a direct 1:1 translation of the IPStatus enums, with the only exception
    /// being that DestinationProtocolUnreachable and DestinationProhibited and have been remapped to ordinal values 4 and 6,
    /// respectively.
    /// </summary>
    public enum DisambiguatedIpStatus
    {
        Unknown = -1,
        Success = 0,
        DestinationProtocolUnreachable = 4,
        DestinationProhibited = 6,
        DestinationNetworkUnreachable = 11002,
        DestinationHostUnreachable = 11003,
        DestinationPortUnreachable = 11005,
        NoResources = 11006,
        BadOption = 11007,
        HardwareError = 11008,
        PacketTooBig = 11009,
        TimedOut = 11010,
        BadRoute = 11012,
        TtlExpired = 11013,
        TtlReassemblyTimeExceeded = 11014,
        ParameterProblem = 11015,
        SourceQuench = 11016,
        BadDestination = 11018,
        DestinationUnreachable = 11040,
        TimeExceeded = 11041,
        BadHeader = 11042,
        UnrecognizedNextHeader = 11043,
        IcmpError = 11044,
        DestinationScopeMismatch = 11045,
    };

    /// <summary>
    /// Translates an IPStatus into one of the remapped DisambiguatedIpStatus enums.
    /// Both IPStatus.DestinationProtocolUnreacahble and IPStatus.DestinationProhibited are mapped to 11004.
    /// </summary>
    /// <param name="ambiguousStatus">The (inherently) ambiguous IPStatus enum to disambiguate</param>
    /// <param name="ipv4"> Whether or not the IPStatus resulted from an ipv4 pinging operation </param>
    /// <returns></returns>
    public static DisambiguatedIpStatus Disambiguate(this IPStatus ambiguousStatus, bool ipv4)
    {
        var ordinalValue = (int)ambiguousStatus;
        if (ordinalValue != 11004)
        {
            return (DisambiguatedIpStatus)ambiguousStatus;
        }

        if (ipv4)
        {
            return DisambiguatedIpStatus.DestinationProtocolUnreachable;
        }

        return DisambiguatedIpStatus.DestinationProhibited;
    }

    public static IcmpStatusCodeEntry GetInfo(this IPStatus ipStatus, IPAddress ip)
    {
        return IcmpStatusCodeLookup.Lookup(ipStatus, ip);
    }
}
