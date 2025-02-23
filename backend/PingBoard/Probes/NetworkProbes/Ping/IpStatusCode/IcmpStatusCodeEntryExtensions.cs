namespace PingBoard.Probes.NetworkProbes.Ping.IpStatusCode;

public static class IcmpStatusCodeEntryExtensions
{
    public static IcmpStatusCodeEntryPublic ToIcmpStatusCodeEntryPublic(
        this IcmpStatusCodeEntry statusEntry
    )
    {
        return new IcmpStatusCodeEntryPublic() with
        {
            IcmpStatusCode = statusEntry.IcmpStatusCode,
            BriefDescription = statusEntry.BriefDescription,
            ExtendedDescription = statusEntry.ExtendedDescription,
        };
    }
}
