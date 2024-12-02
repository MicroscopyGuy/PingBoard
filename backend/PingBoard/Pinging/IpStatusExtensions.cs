namespace PingBoard.Pinging;
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
    public enum DisambiguatedIpStatus{
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
        DestinationScopeMismatch = 11045
    };
    
    /// <summary>
    /// Translates an IPStatus into one of the remapped DisambiguatedIpStatus enums. 
    /// </summary>
    /// <param name="ambiguousStatus"></param>
    /// <returns></returns>
    public static DisambiguatedIpStatus Disambiguate(this IPStatus ambiguousStatus)
    {
        var stringified = nameof(ambiguousStatus);
        switch (stringified)
        {
            case "DestinationProhibited":
                return DisambiguatedIpStatus.DestinationProhibited;
            case "DestinationProtocolUnreachable":
                return DisambiguatedIpStatus.DestinationProtocolUnreachable;
            default:
                return (DisambiguatedIpStatus) ambiguousStatus;
        }
    }

    public static IcmpStatusCodeEntry GetInfo(this IPStatus ipStatus)
    {
        return IcmpStatusCodeLookup.Lookup(ipStatus);
    }
}