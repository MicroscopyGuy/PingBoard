using System.Net.NetworkInformation;
using PingBoard.Pinging;

namespace PingBoard.Tests.PingingTests;

public class ICMPStatusCodeLookupTesting
{
    
    [Fact]
    public void NoDuplicateNames_In_IpStatusDisambiguatedEnums()
    {
        HashSet<string> checkedEnums = new HashSet<string>();
        var newEnumNames = Enum.GetNames(typeof(IpStatusExtensions.DisambiguatedIpStatus));
        foreach (var name in newEnumNames)
        {
            Assert.False(checkedEnums.Contains(name));
            checkedEnums.Add(name);
        }
    }
    
    /// <summary>
    /// Why not use a generator function, something like:
    /// [MemberData(nameof(ICMPStatusCodeLookupTestGenerator.GetIPStatuses), MemberType = typeof(ICMPStatusCodeLookupTestGenerator))]
    /// 
    /// Using either of the generator functions in IcmpStatusCodeLookupTestGenerator.cs results in Xunit mistaking
    /// DestinationPortUnreachable for a second DestinationProtocolUnreachable, since Xunit hashes input data
    /// to determine its uniqueness, and since both these IPStatus enums map to ordinal value 11004.
    ///
    /// Consequently, instead of properly utilizing the generator functions I wrote, the test data are supplied inline.
    /// I am leaving this comment and the generator functions in place until such time that Xunit addresses the issue of
    /// determining the uniqueness of Enums based on possibly non-unique ordinal values.
    /// </summary>
    [Theory]
    [InlineData(IPStatus.Unknown, "Unknown_-1")]
    [InlineData(IPStatus.Success, "Success_0")]
    [InlineData(IPStatus.DestinationNetworkUnreachable, "DestinationNetworkUnreachable_11002")]
    [InlineData(IPStatus.DestinationHostUnreachable, "DestinationHostUnreachable_11003")]
    [InlineData(IPStatus.DestinationProtocolUnreachable, "DestinationProtocolUnreachable_11004")]
    [InlineData(IPStatus.DestinationPortUnreachable, "DestinationPortUnreachable_11004")]
    [InlineData(IPStatus.DestinationProhibited, "DestinationProhibited_11005")]
    [InlineData(IPStatus.NoResources, "NoResources_11006")]
    [InlineData(IPStatus.BadOption, "BadOption_11007")]
    [InlineData(IPStatus.HardwareError, "HardwareError_11008")]
    [InlineData(IPStatus.PacketTooBig, "PacketTooBig_11009")]
    [InlineData(IPStatus.TimedOut, "TimedOut_11010")]
    [InlineData(IPStatus.BadRoute, "BadRoute_11011")]
    [InlineData(IPStatus.TtlExpired, "TtlExpired_11012")]
    [InlineData(IPStatus.TtlReassemblyTimeExceeded, "TtlReassemblyTimeExceeded_11013")]
    [InlineData(IPStatus.ParameterProblem, "ParameterProblem_11014")]
    [InlineData(IPStatus.SourceQuench, "SourceQuench_11015")]
    [InlineData(IPStatus.BadDestination, "BadDestination_11016")]
    [InlineData(IPStatus.DestinationUnreachable, "DestinationUnreachable_11017")]
    [InlineData(IPStatus.TimeExceeded, "TimeExceeded_11018")]
    [InlineData(IPStatus.BadHeader, "BadHeader_11019")]
    [InlineData(IPStatus.UnrecognizedNextHeader, "UnrecognizedNextHeader_11020")]
    [InlineData(IPStatus.IcmpError, "IcmpError_11021")]
    [InlineData(IPStatus.DestinationScopeMismatch, "DestinationScopeMismatch_11022")]
    public void LookupIPStatus(IPStatus status, string statusName)
    {
        var lookupInfo = status.GetInfo();
        Assert.Equal(status.ToString(), lookupInfo.IcmpStatusCode.ToString());
        Assert.IsType<PingingStates.PingState>(lookupInfo.State);
        Assert.NotEqual("", lookupInfo.BriefDescription);
        Assert.NotEqual("", lookupInfo.ExtendedDescription);
    }
}