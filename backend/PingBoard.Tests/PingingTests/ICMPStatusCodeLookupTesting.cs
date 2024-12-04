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
            Assert.DoesNotContain(name, checkedEnums);
            checkedEnums.Add(name);
        }
    }
    

    [Theory]
    [MemberData(nameof(ICMPStatusCodeLookupTestGenerator.GetIPStatuses), MemberType = typeof(ICMPStatusCodeLookupTestGenerator))]
    public void LookupIPStatus(IPStatus status)
    {
        var lookupInfo = status.GetInfo();
        Assert.Equal(status.ToString(), lookupInfo.IcmpStatusCode.ToString());
        Assert.IsType<PingingStates.PingState>(lookupInfo.State);
        Assert.NotEqual("", lookupInfo.BriefDescription);
        Assert.NotEqual("", lookupInfo.ExtendedDescription);
    }
}