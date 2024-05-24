using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace PingBoard.Tests.PingingTests;
using PingBoard.Pinging.Configuration;
using PingBoard.Pinging;
using System.Net.NetworkInformation;
using System.Net;

public class SendPingGroupAsyncTesting
{
    /************************************************** SendPingGroupSync *********************************************/
    #region "SendPingGroupAsync"
    
    [Fact]
    public async void SendPingGroupAsync_ReturnsProperPingGroupSummary_OnTwoSuccessfulPings() {
        IndividualPingerStub pingerStub = new IndividualPingerStub();
        //The target we are going to pretend to ping
        var target = IPAddress.Parse("8.8.8.8");
        
        // Values for the fake PingReply objects that SendPingIndividualAsync will send, every PingReply is one index
        List<long> rtts = [5, 4];
        List<IPStatus> statuses = [IPStatus.Success, IPStatus.Success];
        List<byte[]> buffers = [[], []];
        List<IPAddress> addresses = [target, target];
        List<int> ttls = [64, 64];
        
        // Make sure the Lists of PingReply values are the same length, didn't mess up the test setup
        PingingTestingUtilities.AssertAllProperLength(rtts, statuses, buffers, addresses, ttls, 2);
        
        // Prepare the PingReply stubs which are stored on the pingerStub object, one of which will be returned
        // from each SendPingIndividualAsync function call that SendPingGroupAsync makes
        pingerStub.PrepareStubbedPingReplies(rtts, statuses, buffers, addresses, ttls);
        
        // Make Pinging Thresholds Config object (and wrapping in Options object,
        // since accepts IOptions<PingingThresholdsConfig>
        var pingThresholds = new PingingThresholdsConfig() {
            MinimumPingMs = 5, 
            AveragePingMs = 10, 
            MaximumPingMs = 25,
            JitterMs = 5,
            PacketLossPercentage = 0
        };
        var optionsPingThresholds = Options.Create(pingThresholds);

        // Make PingingBehaviorConfig object (and wrapping in Options object, 
        // since accepts IOptions<PingingBehaviorConfig>
        var pingBehavior = new PingingBehaviorConfig() {
            PayloadStr = "this is a payload string", // not crucial here
            PingsPerCall = 2, // crucial for this test
            ReportBackAfterConsecutiveTimeouts = 2, // not crucial here
            TimeoutMs = 1500, // not crucial for this test
            Ttl = 64,
            WaitMs = 25 // technically violates the WaitMs limits, but the wait time isn't relevant here
            // but limited to 500ms in PingingBehaviorConfigLimits.cs, so dont want to test improper behavior
        };
        var optionsPingBehavior = Options.Create(pingBehavior);
        
        // Make PingScheduler object
        var pingScheduler = new PingScheduler(optionsPingBehavior);
        
        // Make logger (though not implemented as of writing this)
        var logger = new NullLogger<IGroupPinger>();
        
        // Finally, make the GroupPinger
        var groupPinger = new GroupPinger(
            pingerStub,
            new PingQualification(optionsPingThresholds),
            pingScheduler,
            optionsPingBehavior,
            optionsPingThresholds,
            logger
        );
        
        var summary = await groupPinger.SendPingGroupAsync(target, pingBehavior.PingsPerCall);
        Assert.Equal(4.5, summary.AveragePing!.Value);
        Assert.Equal(4, summary.MinimumPing!.Value);
        Assert.Equal(5, summary.MaximumPing!.Value);
        Assert.Equal(1, summary.Jitter);
        Assert.Equal(0, summary.ConsecutiveTimeouts!.Value);
        Assert.Equal(PingQualification.ThresholdExceededFlags.NotExceeded, summary.PingQualityFlags);
        Assert.Equal(0, summary.PacketLoss);
        Assert.Equal(2, summary.PacketsSent!.Value);
        Assert.Equal(0, summary.PacketsLost!.Value);
        Assert.Null(summary.LastAbnormalStatus);
        Assert.Null(summary.TerminatingIPStatus);
    }
    
    [Fact]
    public async void SendPingGroupAsync_ReturnsProperPingGroupSummary_OnTwoUnSuccessfulPings() {
        IndividualPingerStub pingerStub = new IndividualPingerStub();
        //The target we are going to pretend to ping
        var target = IPAddress.Parse("8.8.8.8");
        
        // Values for the fake PingReply objects that SendPingIndividualAsync will send, every PingReply is one index
        List<long> rtts = [0, 0];
        List<IPStatus> statuses = [IPStatus.BadRoute, IPStatus.BadRoute];
        List<byte[]> buffers = [[], []];
        List<IPAddress> addresses = [target, target];
        List<int> ttls = [64, 64];
        
        // Make sure the Lists of PingReply values are the same length, didn't mess up the test setup
        PingingTestingUtilities.AssertAllProperLength(rtts, statuses, buffers, addresses, ttls, 2);
        
        // Prepare the PingReply stubs which are stored on the pingerStub object, one of which will be returned
        // from each SendPingIndividualAsync function call that SendPingGroupAsync makes
        pingerStub.PrepareStubbedPingReplies(rtts, statuses, buffers, addresses, ttls);
        
        // Make Pinging Thresholds Config object (and wrapping in Options object,
        // since accepts IOptions<PingingThresholdsConfig>
        var pingThresholds = new PingingThresholdsConfig() {
            MinimumPingMs = 5, 
            AveragePingMs = 10, 
            MaximumPingMs = 25,
            JitterMs = 5,
            PacketLossPercentage = 0
        };
        var optionsPingThresholds = Options.Create(pingThresholds);

        // Make PingingBehaviorConfig object (and wrapping in Options object, 
        // since accepts IOptions<PingingBehaviorConfig>
        var pingBehavior = new PingingBehaviorConfig() {
            PayloadStr = "this is a payload string", // not crucial here
            PingsPerCall = 2, // crucial for this test
            ReportBackAfterConsecutiveTimeouts = 2, // not crucial here
            TimeoutMs = 1500, // not crucial for this test
            Ttl = 64,
            WaitMs = 25 // technically violates the WaitMs limits, but the wait time isn't relevant here
        };
        var optionsPingBehavior = Options.Create(pingBehavior);
        
        // Make PingScheduler object
        var pingScheduler = new PingScheduler(optionsPingBehavior);
        
        // Make logger (though not implemented as of writing this)
        var logger = new NullLogger<IGroupPinger>();
        
        // Finally, make the GroupPinger
        var groupPinger = new GroupPinger(
            pingerStub,
            new PingQualification(optionsPingThresholds),
            pingScheduler,
            optionsPingBehavior,
            optionsPingThresholds,
            logger
        );
        
        var summary = await groupPinger.SendPingGroupAsync(target, pingBehavior.PingsPerCall);
        Assert.Equal(0, summary.AveragePing!.Value);
        Assert.Equal(0, summary.MinimumPing!.Value);
        Assert.Equal(0, summary.MaximumPing!.Value);
        Assert.Equal(0, summary.Jitter);
        Assert.Equal(0, summary.ConsecutiveTimeouts!.Value);
        Assert.Equal(PingQualification.ThresholdExceededFlags.NotExceeded, summary.PingQualityFlags);
        Assert.Equal(0, summary.PacketLoss);
        Assert.Equal(2, summary.PacketsSent!.Value);
        Assert.Equal(0, summary.PacketsLost!.Value);
        Assert.Equal(IPStatus.BadRoute, summary.LastAbnormalStatus);
        Assert.Null(summary.TerminatingIPStatus);
    }
    
    [Fact]
    public async void SendPingGroupAsync_ReturnsProperPingGroupSummary_OnOnePausePing_AndOneUnsuccessfulPing() {
        IndividualPingerStub pingerStub = new IndividualPingerStub();
        //The target we are going to pretend to ping
        var target = IPAddress.Parse("8.8.8.8");
        
        // Values for the fake PingReply objects that SendPingIndividualAsync will send, every PingReply is one index
        List<long> rtts = [0, 0];
        List<IPStatus> statuses = [IPStatus.SourceQuench, IPStatus.BadRoute];
        List<byte[]> buffers = [[], []];
        List<IPAddress> addresses = [target, target];
        List<int> ttls = [64, 64];
        
        // Make sure the Lists of PingReply values are the same length, didn't mess up the test setup
        PingingTestingUtilities.AssertAllProperLength(rtts, statuses, buffers, addresses, ttls, 2);
        
        // Prepare the PingReply stubs which are stored on the pingerStub object, one of which will be returned
        // from each SendPingIndividualAsync function call that SendPingGroupAsync makes
        pingerStub.PrepareStubbedPingReplies(rtts, statuses, buffers, addresses, ttls);
        
        // Make Pinging Thresholds Config object (and wrapping in Options object,
        // since accepts IOptions<PingingThresholdsConfig>
        var pingThresholds = new PingingThresholdsConfig() {
            MinimumPingMs = 5, 
            AveragePingMs = 10, 
            MaximumPingMs = 25,
            JitterMs = 5,
            PacketLossPercentage = 0
        };
        var optionsPingThresholds = Options.Create(pingThresholds);

        // Make PingingBehaviorConfig object (and wrapping in Options object, 
        // since accepts IOptions<PingingBehaviorConfig>
        var pingBehavior = new PingingBehaviorConfig() {
            PayloadStr = "this is a payload string", // not crucial here
            PingsPerCall = 2, // crucial for this test
            ReportBackAfterConsecutiveTimeouts = 2, // not crucial here
            TimeoutMs = 1500, // not crucial for this test
            Ttl = 64,
            WaitMs = 25 // technically violates the WaitMs limits, but the wait time isn't relevant here,
                        // just want to keep it low for testing
        };
        var optionsPingBehavior = Options.Create(pingBehavior);
        
        // Make PingScheduler object
        var pingScheduler = new PingScheduler(optionsPingBehavior);
        
        // Make logger (though not implemented as of writing this)
        var logger = new NullLogger<IGroupPinger>();
        
        // Finally, make the GroupPinger
        var groupPinger = new GroupPinger(
            pingerStub,
            new PingQualification(optionsPingThresholds),
            pingScheduler,
            optionsPingBehavior,
            optionsPingThresholds,
            logger
        );
        
        var summary = await groupPinger.SendPingGroupAsync(target, pingBehavior.PingsPerCall);
        Assert.Equal(0, summary.AveragePing!.Value);
        Assert.Equal(0, summary.MinimumPing!.Value);
        Assert.Equal(0, summary.MaximumPing!.Value);
        Assert.Equal(0, summary.Jitter);
        Assert.Equal(0, summary.ConsecutiveTimeouts!.Value);
        Assert.Equal(PingQualification.ThresholdExceededFlags.NotExceeded, summary.PingQualityFlags);
        Assert.Equal(0, summary.PacketLoss);
        Assert.Equal(2, summary.PacketsSent!.Value);
        Assert.Equal(0, summary.PacketsLost!.Value);
        Assert.Equal(IPStatus.BadRoute, summary.LastAbnormalStatus);
        Assert.Null(summary.TerminatingIPStatus);
    }
    
    /// <summary>
    /// Reverse order of the previous one, instead of Ping0: Pause, Ping1: Unsuccessful, is Unsuccessful -> Pause
    /// </summary>
    [Fact]
    public async void SendPingGroupAsync_ReturnsProperPingGroupSummary_OnOneUnsuccessfulPing_AndOnePausePing() {
        IndividualPingerStub pingerStub = new IndividualPingerStub();
        //The target we are going to pretend to ping
        var target = IPAddress.Parse("8.8.8.8");
        
        // Values for the fake PingReply objects that SendPingIndividualAsync will send, every PingReply is one index
        List<long> rtts = [0, 0];
        List<IPStatus> statuses = [IPStatus.SourceQuench, IPStatus.BadRoute];
        List<byte[]> buffers = [[], []];
        List<IPAddress> addresses = [target, target];
        List<int> ttls = [64, 64];
        
        // Make sure the Lists of PingReply values are the same length, didn't mess up the test setup
        PingingTestingUtilities.AssertAllProperLength(rtts, statuses, buffers, addresses, ttls, 2);
        
        // Prepare the PingReply stubs which are stored on the pingerStub object, one of which will be returned
        // from each SendPingIndividualAsync function call that SendPingGroupAsync makes
        pingerStub.PrepareStubbedPingReplies(rtts, statuses, buffers, addresses, ttls);
        
        // Make Pinging Thresholds Config object (and wrapping in Options object,
        // since accepts IOptions<PingingThresholdsConfig>
        var pingThresholds = new PingingThresholdsConfig() {
            MinimumPingMs = 5, 
            AveragePingMs = 10, 
            MaximumPingMs = 25,
            JitterMs = 5,
            PacketLossPercentage = 0
        };
        var optionsPingThresholds = Options.Create(pingThresholds);

        // Make PingingBehaviorConfig object (and wrapping in Options object, 
        // since accepts IOptions<PingingBehaviorConfig>
        var pingBehavior = new PingingBehaviorConfig() {
            PayloadStr = "this is a payload string", // not crucial here
            PingsPerCall = 2, // crucial for this test
            ReportBackAfterConsecutiveTimeouts = 2, // not crucial here
            TimeoutMs = 1500, // not crucial for this test
            Ttl = 64,
            WaitMs = 25 // technically violates the WaitMs limits, but the wait time isn't relevant here,
                        // just want to keep it low for testing
        };
        var optionsPingBehavior = Options.Create(pingBehavior);
        
        // Make PingScheduler object
        var pingScheduler = new PingScheduler(optionsPingBehavior);
        
        // Make logger (though not implemented as of writing this)
        var logger = new NullLogger<IGroupPinger>();
        
        // Finally, make the GroupPinger
        var groupPinger = new GroupPinger(
            pingerStub,
            new PingQualification(optionsPingThresholds),
            pingScheduler,
            optionsPingBehavior,
            optionsPingThresholds,
            logger
        );
        
        var summary = await groupPinger.SendPingGroupAsync(target, pingBehavior.PingsPerCall);
        Assert.Equal(0, summary.AveragePing!.Value);
        Assert.Equal(0, summary.MinimumPing!.Value);
        Assert.Equal(0, summary.MaximumPing!.Value);
        Assert.Equal(0, summary.Jitter);
        Assert.Equal(0, summary.ConsecutiveTimeouts!.Value);
        Assert.Equal(PingQualification.ThresholdExceededFlags.NotExceeded, summary.PingQualityFlags);
        Assert.Equal(0, summary.PacketLoss);
        Assert.Equal(2, summary.PacketsSent!.Value);
        Assert.Equal(0, summary.PacketsLost!.Value);
        Assert.Equal(IPStatus.BadRoute, summary.LastAbnormalStatus);
        Assert.Null(summary.TerminatingIPStatus);
    }
    /************************************************ End SendPingGroupSync *******************************************/
    #endregion
}