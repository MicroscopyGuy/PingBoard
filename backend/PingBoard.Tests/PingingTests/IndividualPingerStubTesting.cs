namespace PingBoard.Tests.PingingTests;
using PingBoard.TestUtilities.PingingTestingUtilities;
using System.Net.NetworkInformation;

public class IndividualPingerStubTesting{
    /****************************************************** MakePingOptionsStub ***************************************/
    #region "MakePingOptionsStub"

    [Fact]
    public void MakePingOptionsStub_ConstructsValidPingOptions_WhenMakeNullIsFalse() {
        PingOptions pingOptionsStub = IndividualPingerStub.MakePingOptions(64, true);
        Assert.NotNull(pingOptionsStub);
        Assert.Equal(64, pingOptionsStub.Ttl);
        Assert.True(pingOptionsStub.DontFragment);
    }

    [Fact]
    public void MakePingOptionsStub_ConstructsNullPingOptions_WhenMakeNullIsTrue() {
        PingOptions pingOptionsStub = IndividualPingerStub.MakePingOptions(0, true, true);
        Assert.Null(pingOptionsStub);
    }
    
    #endregion
    /*************************************************** End MakePingOptionsStub **************************************/
    
    /****************************************************** MakePingReplyStub *****************************************/
    #region "MakePingReplyStub"
    [Fact]
    public void MakePingReplyStub_ConstructsValidPingReply_WithoutNullOptions() {
        PingReply stub = IndividualPingerStub.MakePingReplyStub(
            5, IPStatus.Success, new byte[] { }, "", 64);
        Assert.Equal(5, stub.RoundtripTime);
        Assert.Equal(IPStatus.Success, stub.Status);
        Assert.Empty(stub.Buffer);
        Assert.NotNull(stub.Options);
        Assert.Equal(64, stub.Options!.Ttl);
        Assert.True(stub.Options!.DontFragment);
    }

    [Fact]
    public void MakePingReplyStub_ConstructsValidPingReply_WithNullOptions() {
        PingReply stub = IndividualPingerStub.MakePingReplyStub(
            5, IPStatus.DestinationHostUnreachable, new byte[] { }, "", 64);
        Assert.Equal(0, stub.RoundtripTime);
        Assert.Equal(IPStatus.DestinationHostUnreachable, stub.Status);
        Assert.Empty(stub.Buffer);
        Assert.Null(stub.Options);
    }
    
    
    #endregion
    /*************************************************** End MakePingReplyStub ****************************************/
    
    /*************************************************** PreparePingReplyStubs ****************************************/
    #region "PreparePingReplyStubs"

    [Fact]
    public async Task PreparePingReplyStubs_CreatesPingReplyStubs_OnTwoIPStatusSuccessStubs()
    {
        IndividualPingerStub pingerStub = new IndividualPingerStub();
        string target = "8.8.8.8";
        
        List<long> rtts = [5, 4];
        List<IPStatus> statuses = [IPStatus.Success, IPStatus.Success];
        List<byte[]> buffers = [[], []];
        List<string> addresses = [target, target];
        List<int> ttls = [64, 64];
        pingerStub.PrepareStubbedPingReplies(rtts, statuses, buffers, addresses, ttls);

        for (int i = 0; i < rtts.Count; i++){
            PingReply latestReply = await pingerStub.SendPingIndividualAsync(addresses[i]);
            Assert.NotNull(latestReply);
            Assert.Equal(rtts[i], latestReply.RoundtripTime);
            Assert.Equal(statuses[i], latestReply.Status);
            Assert.Equal(buffers[i], latestReply.Buffer);
            Assert.Equal(addresses[i], latestReply.Address.ToString());
            Assert.Equal(ttls[i], latestReply.Options!.Ttl);
        }
    }
    
    [Fact]
    public async Task PreparePingReplyStubs_CreatesTwoPingReplyStubs_OnTwoUnsuccessfulPingReplies()
    {
        IndividualPingerStub pingerStub = new IndividualPingerStub();
        string target = "0.0.0.0";
        
        List<long> rtts = [0, 0];
        List<IPStatus> statuses = [IPStatus.BadRoute, IPStatus.BadRoute];
        List<byte[]> buffers = [[], []];
        List<string> addresses = [target, target];
        List<int> ttls = [64, 64];
        

        Assert.Equal(2, rtts.Count);
        Assert.Equal(2, statuses.Count);
        Assert.Equal(2, buffers.Count);
        Assert.Equal(2, addresses.Count);

        
        pingerStub.PrepareStubbedPingReplies(rtts, statuses, buffers, addresses, ttls);
    
        for (int i = 0; i < rtts.Count; i++){
            PingReply latestReply = await pingerStub.SendPingIndividualAsync(addresses[i]);
            Assert.NotNull(latestReply);
            Assert.Null(latestReply.Options);
            Assert.Equal(rtts[i], latestReply.RoundtripTime);
            Assert.Equal(statuses[i], latestReply.Status);
            Assert.Equal(buffers[i], latestReply.Buffer);
            Assert.Equal(addresses[i], latestReply.Address.ToString());
        }
    }
    
    #endregion
    
    
    
}