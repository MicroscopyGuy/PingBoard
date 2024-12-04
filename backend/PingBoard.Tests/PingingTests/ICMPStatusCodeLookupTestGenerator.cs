using System.Net.NetworkInformation;

namespace PingBoard.Tests.PingingTests;

public class ICMPStatusCodeLookupTestGenerator
{
    public static IEnumerable<object[]> GetIPStatuses(){
        foreach (var status in Enum.GetValues<IPStatus>())
        {
            yield return new object[] { status };
        }
    }
    
    public static IEnumerable<object[]> GetIPStatii()
    {
        var ipStatuses = new List<IPStatus>()
        {
            IPStatus.Unknown,
            IPStatus.Success,
            IPStatus.DestinationNetworkUnreachable,
            IPStatus.DestinationHostUnreachable,
            IPStatus.DestinationProhibited,
            IPStatus.DestinationProtocolUnreachable,
            IPStatus.DestinationPortUnreachable,
            IPStatus.NoResources,
            IPStatus.BadOption,
            IPStatus.HardwareError,
            IPStatus.PacketTooBig,
            IPStatus.TimedOut,
            IPStatus.BadRoute,
            IPStatus.TtlExpired,
            IPStatus.TtlReassemblyTimeExceeded,
            IPStatus.ParameterProblem,
            IPStatus.SourceQuench,
            IPStatus.BadDestination,
            IPStatus.DestinationUnreachable,
            IPStatus.TimeExceeded,
            IPStatus.BadHeader,
            IPStatus.UnrecognizedNextHeader,
            IPStatus.IcmpError,
            IPStatus.DestinationScopeMismatch
        };
        
        foreach (var status in ipStatuses)
        {
            yield return new object[] { status, $"{status.ToString()}_{(int)status}" };
        }
    }
}