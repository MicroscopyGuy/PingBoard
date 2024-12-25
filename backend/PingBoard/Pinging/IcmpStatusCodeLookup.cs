namespace PingBoard.Pinging;

using System.Collections.Immutable;
using System.Net;
using System.Net.NetworkInformation;
using System.Text.Json;
using static IpAddressExtensions;

public static class IcmpStatusCodeLookup
{
    /// <summary>
    /// Immutable data container allowing each IPStatus enum to reference its associated info
    /// <seealso>
    ///                                                          **Important:**
    ///           Note that the DestinationProhibited and DestinationProtocolUnreachable enums
    ///           both have the same ordinal value (11004). This causes a System.TypeInitializationException when
    ///           deserializing this data into StatusCodes (IImmutableDictionary) since each IPStatus' ordinal value
    ///           is checked for uniqueness.
    ///
    ///           The DestinationProhibited enum describes a prohibited contact with a target computer, and is ONLY for use with IPv6.
    ///           The DestinationProtocolUnreachable is similar, but uses IPv4. Since this application is only using IPv4 -- for now --
    ///           I have removed the DestinationProhibited entry from ICMPStatusCodes.json file.
    ///
    ///           If in the future this enum becomes relevant, ie, if IPv6 support is added, then I will perhaps create my own set of enums
    ///           and map these IPStatus enums to them, to avoid this duplication.
    ///   </seealso>
    /// </summary>
    private static readonly IImmutableDictionary<
        IpStatusExtensions.DisambiguatedIpStatus,
        IcmpStatusCodeEntry
    > StatusCodes;

    /// <summary>
    /// Static constructor, populates StatusCodes with the IPStatus information from ICMPStatusCodes.json
    /// </summary>
    static IcmpStatusCodeLookup()
    {
        string statusCodeInfoFromJson = ReadInIpStatusInfo("ICMPStatusCodes.json");
        List<IcmpStatusCodeEntry> icmpStatusList = JsonSerializer.Deserialize<
            List<IcmpStatusCodeEntry>
        >(statusCodeInfoFromJson)!;
        StatusCodes = icmpStatusList.ToImmutableDictionary(
            (icmpStatusEntry) => icmpStatusEntry.IcmpStatusCode
        );
    }

    /// <summary>
    /// Reads in the IPStatus information from the ICMPStatusCodes.json folder
    /// </summary>
    /// <param name="filename"></param>
    /// <returns> the file contents as a string </returns>
    private static string ReadInIpStatusInfo(string filename)
    {
        var type = typeof(IcmpStatusCodeLookup); // to get namespace and assembly name

        // use ErrorCodeLookup (can choose any class in this namespace) to get the namespace
        var fullEmbeddedFileName = $"{type.Namespace}.{filename}";

        // fullEmbeddedFileName requires namespace for access, and now use the type to get the assembly
        // then pass fullEmbeddedFileName to GetManifestResourceStream to access the file
        using var stream = type.Assembly.GetManifestResourceStream(fullEmbeddedFileName);
        using var reader = new StreamReader(stream!);
        return reader.ReadToEnd();
    }

    public static IcmpStatusCodeEntry Lookup(IPStatus ipStatus, IPAddress respondingIPAddress)
    {
        var ipv4 = respondingIPAddress.IsIPv4();
        return StatusCodes[ipStatus.Disambiguate(ipv4)];
    }
}
