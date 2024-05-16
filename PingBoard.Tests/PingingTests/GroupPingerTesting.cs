namespace PingBoard.Tests.PingingTests;
using PingBoard.Pinging.Configuration;
using PingBoard.Pinging;
using Limits = PingBoard.Pinging.Configuration.PingingBehaviorConfigLimits;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Net;

public class GroupPingerTesting{

    public PingReply MakePingReplyStub(long rtt, IPStatus status, byte[] buffer){
        BindingFlags bindingFlags = BindingFlags.Default | BindingFlags.CreateInstance | 
                                    BindingFlags.NonPublic | BindingFlags.Instance; 
     
        PingReply stub = (PingReply) Activator.CreateInstance(
            typeof(PingReply),
            bindingFlags,
            null,
            new object?[] { IPAddress.Any, null, status, rtt, buffer},
            null)!;

        return stub;
    }

    [Fact]
    public void ProcessContinueUpdatesAveragePing(){
        PingGroupSummary testSummary = PingGroupSummary.Empty();
        testSummary.MinimumPing = 2;
        testSummary.AveragePing = 6; // since not yet divided by / of pings sent
        testSummary.MaximumPing = 4;
        long[] rtts = new long[8];
        rtts.Append(2); rtts.Append(4);
        int pingCounter = 2;
        PingReply fakeReply = MakePingReplyStub(3, IPStatus.Success, new byte[]{});
        GroupPinger.ProcessContinue(testSummary, fakeReply, rtts, pingCounter);

        Assert.Equal(9, testSummary.AveragePing);
        Assert.Equal(2, testSummary.MinimumPing.Value);
        Assert.Equal(4, testSummary.MaximumPing.Value);
        Assert.Equal(3, rtts[pingCounter]);
    }

    [Fact]
    public void ProcessContinueUpdatesMinimumPingWhenNewMinimumPing(){
        PingGroupSummary testSummary = PingGroupSummary.Empty();
        testSummary.MinimumPing = 3;
        testSummary.AveragePing = 7; // since not yet divided by / of pings sent
        testSummary.MaximumPing = 4;
        long[] rtts = new long[8];
        rtts.Append(3); rtts.Append(4);
        int pingCounter = 2;
        PingReply fakeReply = MakePingReplyStub(2, IPStatus.Success, new byte[]{});
        GroupPinger.ProcessContinue(testSummary, fakeReply, rtts, pingCounter);

        Assert.Equal(9, testSummary.AveragePing);
        Assert.Equal(2, testSummary.MinimumPing.Value);
        Assert.Equal(4, testSummary.MaximumPing.Value);
        Assert.Equal(2, rtts[pingCounter]);
    }

    [Fact]
    public void ProcessContinueUpdatesMaximumPingWhenNewMaximumPing(){
        PingGroupSummary testSummary = PingGroupSummary.Empty();
        testSummary.MinimumPing = 3;
        testSummary.AveragePing = 15; // since not yet divided by / of pings sent
        testSummary.MaximumPing = 12;
        long[] rtts = new long[8];
        rtts.Append(3); rtts.Append(12);
        int pingCounter = 2;
        PingReply fakeReply = MakePingReplyStub(197, IPStatus.Success, new byte[]{});
        GroupPinger.ProcessContinue(testSummary, fakeReply, rtts, pingCounter);

        Assert.Equal(212, testSummary.AveragePing);
        Assert.Equal(3, testSummary.MinimumPing.Value);
        Assert.Equal(197, testSummary.MaximumPing.Value);
        Assert.Equal(197, rtts[pingCounter]);
    }

    [Fact]
    public void ProcessContinueUpdatesMinimumAndMaxPingWhenFirstPingInGroup(){
        PingGroupSummary testSummary = PingGroupSummary.Empty();
        long[] rtts = new long[8];
        rtts.Append(3);
        int pingCounter = 1;
        PingReply fakeReply = MakePingReplyStub(3, IPStatus.Success, new byte[]{});
        GroupPinger.ProcessContinue(testSummary, fakeReply, rtts, pingCounter);

        Assert.Equal(3, testSummary.AveragePing);
        Assert.Equal(3, testSummary.MinimumPing!.Value);
        Assert.Equal(3, testSummary.MaximumPing!.Value);
        Assert.Equal(3, rtts[pingCounter]);
    }

    [Fact]
    public void ProcessContinueOnlyUpdatesAveragePingStoresRttWhenUremarkablePing(){
        PingGroupSummary testSummary = PingGroupSummary.Empty();
        testSummary.MinimumPing = 3;
        testSummary.AveragePing = 15; // since not yet divided by / of pings sent
        testSummary.MaximumPing = 12;
        long[] rtts = new long[8];
        rtts.Append(4);
        int pingCounter = 1;
        PingReply fakeReply = MakePingReplyStub(4, IPStatus.Success, new byte[]{});
        GroupPinger.ProcessContinue(testSummary, fakeReply, rtts, pingCounter);

        Assert.Equal(19, testSummary.AveragePing);
        Assert.Equal(3, testSummary.MinimumPing!.Value);
        Assert.Equal(12, testSummary.MaximumPing!.Value);
        Assert.Equal(4, rtts[pingCounter]);
    }

    [Fact]
    public void ProcessHaltSavesTerminatingIPStatusOnHaltingIPStatus(){
        PingGroupSummary testSummary = PingGroupSummary.Empty();
        PingReply fakeReply = MakePingReplyStub(4, IPStatus.HardwareError, new byte[]{});
        GroupPinger.ProcessHalt(testSummary, fakeReply);

        Assert.Equal(IPStatus.HardwareError, testSummary.TerminatingIPStatus);
    }

    [Fact]
    public void ProcessHaltSavesTerminatingIPStatusOnPausingIPStatus(){
        PingGroupSummary testSummary = PingGroupSummary.Empty();
        PingReply fakeReply = MakePingReplyStub(4, IPStatus.SourceQuench, new byte[]{});
        GroupPinger.ProcessPause(testSummary, fakeReply);

        Assert.Equal(IPStatus.SourceQuench, testSummary.TerminatingIPStatus);
    }







}