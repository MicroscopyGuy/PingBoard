using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using PingBoard.Database.Models;

namespace PingBoard.Tests.PingingTests;
using PingBoard.Pinging.Configuration;
using PingBoard.Pinging;
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
        Assert.Equal(2, testSummary.MinimumPing);
        Assert.Equal(4, testSummary.MaximumPing);
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
        Assert.Equal(2, testSummary.MinimumPing);
        Assert.Equal(4, testSummary.MaximumPing);
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
        Assert.Equal(3, testSummary.MinimumPing);
        Assert.Equal(197, testSummary.MaximumPing);
        Assert.Equal(197, rtts[rtts.Count-1]);
    }

    [Fact]
    public void ProcessContinue_UpdatesMinimumAndMaxPing_WhenFirstPingInGroup(){
        PingGroupSummary testSummary = PingGroupSummary.Empty();
        List<long> rtts = new List<long>{ 3 };

        PingReply fakeReply = MakePingReplyStub(3, IPStatus.Success, new byte[]{});
        GroupPinger.ProcessContinue(testSummary, fakeReply, rtts);

        Assert.Equal(3, testSummary.AveragePing);
        Assert.Equal(3, testSummary.MinimumPing);
        Assert.Equal(3, testSummary.MaximumPing);
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
        Assert.Equal(3, testSummary.MinimumPing);
        Assert.Equal(12, testSummary.MaximumPing);
        Assert.Equal(4, rtts[rtts.Count-1]);
    }
    
    [Fact]
    public void ProcessContinue_DoesntSaveUnsuccessfulPings(){
        PingGroupSummary testSummary = PingGroupSummary.Empty();
        List<long> rtts = new List<long>{};

        PingReply fakeReply = MakePingReplyStub(0, IPStatus.DestinationNetworkUnreachable, new byte[]{});
        GroupPinger.ProcessContinue(testSummary, fakeReply, rtts);

        Assert.Equal(0, testSummary.AveragePing);
        Assert.Equal(short.MaxValue, testSummary.MinimumPing);
        Assert.Equal(short.MinValue, testSummary.MaximumPing);
        Assert.Empty(rtts);
    }
    
    

    [Fact]
    public void ProcessHalt_SavesTerminatingIPStatus_OnHaltingIPStatus(){
        PingGroupSummary testSummary = PingGroupSummary.Empty();
        PingReply fakeReply = MakePingReplyStub(4, IPStatus.HardwareError, new byte[]{});

        GroupPinger.ProcessHalt(testSummary, fakeReply);
        Assert.Equal(IPStatus.HardwareError, testSummary.TerminatingIPStatus);
    }

    [Fact]
    public void ProcessHalt_SavesLastAbnormalIPStatus_OnPausingIPStatus(){
        PingGroupSummary testSummary = PingGroupSummary.Empty();
        PingReply fakeReply = MakePingReplyStub(7, IPStatus.SourceQuench, new byte[]{});

        GroupPinger.ProcessPause(testSummary, fakeReply);

        Assert.Equal(IPStatus.SourceQuench, testSummary.LastAbnormalStatus);
    }

    [Fact]
    public void ProcessPacketLossCaution_UpdatesPacketInfo_OnPacketLoss(){
        PingGroupSummary testSummary = PingGroupSummary.Empty();
        PingReply fakeReply = MakePingReplyStub(1504, IPStatus.TimedOut, new byte[]{}); // don't actually know if rtt returned on TimedOut

        GroupPinger.ProcessPacketLossCaution(testSummary, fakeReply);
        
        Assert.Equal(1, testSummary.PacketsLost);
        Assert.Equal(1, testSummary.ConsecutiveTimeouts);
    }
    
}