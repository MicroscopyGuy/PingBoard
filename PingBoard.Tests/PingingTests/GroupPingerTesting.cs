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
    public void ProcessContinue_UpdatesAveragePing(){
        PingGroupSummary testSummary = PingGroupSummary.Empty();
        testSummary.MinimumPing = 2;
        testSummary.AveragePing = 6; // since not yet divided by / of pings sent
        testSummary.MaximumPing = 4;

        List<long> rtts = new List<long>{ 2, 4};
   
        PingReply fakeReply = MakePingReplyStub(3, IPStatus.Success, new byte[]{});
        GroupPinger.ProcessContinue(testSummary, fakeReply, rtts);

        Assert.Equal(9, testSummary.AveragePing);
        Assert.Equal(2, testSummary.MinimumPing.Value);
        Assert.Equal(4, testSummary.MaximumPing.Value);
        Assert.Equal(3, rtts[rtts.Count-1]);
    }

    [Fact]
    public void ProcessContinue_UpdatesMinimumPingWhenNewMinimumPing(){
        PingGroupSummary testSummary = PingGroupSummary.Empty();
        testSummary.MinimumPing = 3;
        testSummary.AveragePing = 7; // since not yet divided by / of pings sent
        testSummary.MaximumPing = 4;
        List<long> rtts = new List<long>{ 3, 4};
     
        PingReply fakeReply = MakePingReplyStub(2, IPStatus.Success, new byte[]{});
        GroupPinger.ProcessContinue(testSummary, fakeReply, rtts);

        Assert.Equal(9, testSummary.AveragePing);
        Assert.Equal(2, testSummary.MinimumPing.Value);
        Assert.Equal(4, testSummary.MaximumPing.Value);
        Assert.Equal(2, rtts[rtts.Count-1]);
    }

    [Fact]
    public void ProcessContinue_UpdatesMaximumPingWhenNewMaximumPing(){
        PingGroupSummary testSummary = PingGroupSummary.Empty();
        testSummary.MinimumPing = 3;
        testSummary.AveragePing = 15; // since not yet divided by / of pings sent
        testSummary.MaximumPing = 12;
        List<long> rtts = new List<long>{3, 12 };
    
        PingReply fakeReply = MakePingReplyStub(197, IPStatus.Success, new byte[]{});
        GroupPinger.ProcessContinue(testSummary, fakeReply, rtts);

        Assert.Equal(212, testSummary.AveragePing);
        Assert.Equal(3, testSummary.MinimumPing.Value);
        Assert.Equal(197, testSummary.MaximumPing.Value);
        Assert.Equal(197, rtts[rtts.Count-1]);
    }

    [Fact]
    public void ProcessContinue_UpdatesMinimumAndMaxPing_WhenFirstPingInGroup(){
        PingGroupSummary testSummary = PingGroupSummary.Empty();
        List<long> rtts = new List<long>{ 3 };

        PingReply fakeReply = MakePingReplyStub(3, IPStatus.Success, new byte[]{});
        GroupPinger.ProcessContinue(testSummary, fakeReply, rtts);

        Assert.Equal(3, testSummary.AveragePing);
        Assert.Equal(3, testSummary.MinimumPing!.Value);
        Assert.Equal(3, testSummary.MaximumPing!.Value);
        Assert.Equal(3, rtts[rtts.Count-1]);
    }

    [Fact]
    public void ProcessContinue_OnlyUpdatesAveragePingAndStoresRtt_WhenUremarkablePing(){
        PingGroupSummary testSummary = PingGroupSummary.Empty();
        testSummary.MinimumPing = 3;
        testSummary.AveragePing = 15; // since not yet divided by / of pings sent
        testSummary.MaximumPing = 12;
        List<long> rtts = new List<long>{ 4 };

        PingReply fakeReply = MakePingReplyStub(4, IPStatus.Success, new byte[]{});
        GroupPinger.ProcessContinue(testSummary, fakeReply, rtts);

        Assert.Equal(19, testSummary.AveragePing);
        Assert.Equal(3, testSummary.MinimumPing!.Value);
        Assert.Equal(12, testSummary.MaximumPing!.Value);
        Assert.Equal(4, rtts[rtts.Count-1]);
    }

    [Fact]
    public void ProcessHalt_SavesTerminatingIPStatus_OnHaltingIPStatus(){
        PingGroupSummary testSummary = PingGroupSummary.Empty();
        List<long> rtts = new List<long>();
        PingReply fakeReply = MakePingReplyStub(4, IPStatus.HardwareError, new byte[]{});

        GroupPinger.ProcessHalt(testSummary, fakeReply, rtts);
        Assert.Equal(IPStatus.HardwareError, testSummary.TerminatingIPStatus);
        Assert.Equal(4, rtts[rtts.Count-1]);
    }

    [Fact]
    public void ProcessHalt_SavesTerminatingIPStatus_OnPausingIPStatus(){
        PingGroupSummary testSummary = PingGroupSummary.Empty();
        List<long> rtts = new List<long>();
        PingReply fakeReply = MakePingReplyStub(7, IPStatus.SourceQuench, new byte[]{});

        GroupPinger.ProcessPause(testSummary, fakeReply, rtts);

        Assert.Equal(IPStatus.SourceQuench, testSummary.TerminatingIPStatus);
    }

    [Fact]
    public void ProcessPacketLossCaution_UpdatesPacketInfo_OnPacketLoss(){
        PingGroupSummary testSummary = PingGroupSummary.Empty();
        PingReply fakeReply = MakePingReplyStub(1504, IPStatus.TimedOut, new byte[]{}); // don't actually know if rtt returned on TimedOut

        GroupPinger.ProcessPacketLossCaution(testSummary, fakeReply);

        Assert.Equal(1, testSummary.PacketsSent!.Value);
        Assert.Equal(1, testSummary.PacketsLost!.Value);
        Assert.Equal(1, testSummary.ConsecutiveTimeouts!.Value);
    }
}