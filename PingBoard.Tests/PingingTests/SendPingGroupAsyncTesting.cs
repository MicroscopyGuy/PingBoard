using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace PingBoard.Tests.PingingTests;
using PingBoard.Pinging.Configuration;
using PingBoard.Pinging;
using System.Net.NetworkInformation;
using System.Net;

/// <summary>
/// Ideal for testing proper PingGroupSummary return values for every combination of two states/behaviors.
/// Every "Continue" state most likely is because the ping responded with IPStatus.Success, but there (as of writing this)
/// other IPStatus codes also mapped to PingingStates.Continue, simply because these are possibly transient
/// problems.
///
/// As a result of this possibility, however, the Continue state can happen from IPStatus.Success (successful), or
/// one of a few other IPStatuses also mapped to the Continue state, but which were not the product of a successful ping.
/// For this reason there is an additional term (Unsuccessful) included below in the list of combinations to test.
///
/// This 
/// |*************************************************Combinations:****************************************************|
/// |1) {Success,Success}                     ←-------                                                                 |
/// |2) {Success,Unsuccessful}                                                                                         |
/// |3) {Success,Pause}                                                                                                |
/// |4) {Success,Halt}                                                                                                 |
/// |5) {Success,PacketLossCaution}                                                                                    |
/// |6) {Unsuccessful,Success}                                                                                         | 
/// |7) {Unsuccessful,Unsuccessful}                                                                                    |
/// |8) {Unsuccessful,Pause}                                                                                           |
/// |9) {Unsuccessful,Halt}                                                                                            |
/// |10) {Unsuccessful,PacketLossCaution}                                                                              |
/// |11) {Pause,Success}                                                                                               |
/// |12) {Pause,Unsuccessful}                                                                                          |
/// |13) {Pause,Pause}                                                                                                 |
/// |14) {Pause,Halt}                                                                                                  |
/// |15) {Pause,PacketLossCaution}                                                                                     |
/// |16) {Halt,Success}                                                                                                |
/// |17) {Halt,Unsuccessful}                                                                                           |
/// |18) {Halt,Pause}                                                                                                  |
/// |19) {Halt,Halt}                                                                                                   |
/// |20) {Halt,PacketLossCaution}                                                                                      |
/// |21) {PacketLossCaution,Success}                                                                                   |
/// |22) {PacketLossCaution,Unsuccessful}                                                                              |
/// |23) {PacketLossCaution,Pause}                                                                                     |
/// |24) {PacketLossCaution,Halt}                                                                                      |
/// |25) {PacketLossCaution,PacketLossCaution}                                                                         |
/// |******************************************************************************************************************|
/// </summary>

public partial class SendPingGroupAsyncTestingTwoPings {
    
    private static readonly PingingThresholdsConfig PingThresholds = new PingingThresholdsConfig() {
        MinimumPingMs = 5, 
        AveragePingMs = 10, 
        MaximumPingMs = 25,
        JitterMs = 5,
        PacketLossPercentage = 0
    };
    private static readonly IOptions<PingingThresholdsConfig> PingThresholdsOptions = Options.Create(PingThresholds);
    
    private static readonly PingingBehaviorConfig PingBehavior = new PingingBehaviorConfig(){
        PayloadStr = "this is a payload string", // not crucial here
        PingsPerCall = 2, // crucial for this test
        ReportBackAfterConsecutiveTimeouts = 2, // not crucial here
        TimeoutMs = 1500, // not crucial for this test
        Ttl = 64,
        WaitMs = 25 // technically violates the WaitMs limits, but the wait time isn't relevant here
    };

    private static readonly IOptions<PingingBehaviorConfig> PingBehaviorOptions = Options.Create(PingBehavior);
    
    /********************************************* Beginning With Success *********************************************/
    #region "BeginningWithSuccessfulPing"
    //{Success,Success}
    [Fact]
    public async void SendPingGroupAsync_ReturnsProperPingGroupSummary_OnSuccessSuccess() {
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
        
        // Make PingScheduler object
        var pingScheduler = new PingScheduler(PingBehaviorOptions);
        
        // Make logger (though not implemented as of writing this)
        var logger = new NullLogger<IGroupPinger>();
        
        // Finally, make the GroupPinger
        var groupPinger = new GroupPinger(
            pingerStub,
            new PingQualification(PingThresholdsOptions),
            pingScheduler,
            PingBehaviorOptions,
            PingThresholdsOptions,
            logger
        );
        
        var summary = await groupPinger.SendPingGroupAsync(target, PingBehavior.PingsPerCall);
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
    
    // {Success,Unsuccessful}
    [Fact]
    public async void SendPingGroupAsync_ReturnsProperPingGroupSummary_OnSuccessUnsuccessful() {
        IndividualPingerStub pingerStub = new IndividualPingerStub();
        //The target we are going to pretend to ping
        var target = IPAddress.Parse("8.8.8.8");
        
        // Values for the fake PingReply objects that SendPingIndividualAsync will send, every PingReply is one index
        List<long> rtts = [5, 0];
        List<IPStatus> statuses = [IPStatus.Success, IPStatus.DestinationPortUnreachable];
        List<byte[]> buffers = [[], []];
        List<IPAddress> addresses = [target, target];
        List<int> ttls = [64, 64];
        
        // Make sure the Lists of PingReply values are the same length, didn't mess up the test setup
        PingingTestingUtilities.AssertAllProperLength(rtts, statuses, buffers, addresses, ttls, 2);
        
        // Prepare the PingReply stubs which are stored on the pingerStub object, one of which will be returned
        // from each SendPingIndividualAsync function call that SendPingGroupAsync makes
        pingerStub.PrepareStubbedPingReplies(rtts, statuses, buffers, addresses, ttls);
        
        var optionsPingBehavior = Options.Create(PingBehaviorOptions);
        
        // Make PingScheduler object
        var pingScheduler = new PingScheduler(PingBehaviorOptions);
        
        // Make logger (though not implemented as of writing this)
        var logger = new NullLogger<IGroupPinger>();
        
        // Finally, make the GroupPinger
        var groupPinger = new GroupPinger(
            pingerStub,
            new PingQualification(PingThresholdsOptions),
            pingScheduler,
            PingBehaviorOptions,
            PingThresholdsOptions,
            logger
        );
        
        var summary = await groupPinger.SendPingGroupAsync(target, PingBehavior.PingsPerCall);
        Assert.Equal(5, summary.AveragePing!.Value);
        Assert.Equal(5, summary.MinimumPing!.Value);
        Assert.Equal(5, summary.MaximumPing!.Value);
        Assert.Equal(0, summary.Jitter);
        Assert.Equal(0, summary.ConsecutiveTimeouts!.Value);
        Assert.Equal(PingQualification.ThresholdExceededFlags.NotExceeded, summary.PingQualityFlags);
        Assert.Equal(0, summary.PacketLoss);
        Assert.Equal(2, summary.PacketsSent!.Value);
        Assert.Equal(0, summary.PacketsLost!.Value);
        Assert.Equal(1, summary.ExcludedPings!.Value);
        Assert.Equal(IPStatus.DestinationPortUnreachable, summary.LastAbnormalStatus);
        Assert.Null(summary.TerminatingIPStatus);
    }
    
    //3) {Success,Pause}
    [Fact]
    public async void SendPingGroupAsync_ReturnsProperPingGroupSummary_OnSuccessPause() {
        IndividualPingerStub pingerStub = new IndividualPingerStub();
        //The target we are going to pretend to ping
        var target = IPAddress.Parse("8.8.8.8");
        
        // Values for the fake PingReply objects that SendPingIndividualAsync will send, every PingReply is one index
        List<long> rtts = [5, 0];
        List<IPStatus> statuses = [IPStatus.Success, IPStatus.SourceQuench];
        List<byte[]> buffers = [[], []];
        List<IPAddress> addresses = [target, target];
        List<int> ttls = [64, 64];
        
        // Make sure the Lists of PingReply values are the same length, didn't mess up the test setup
        PingingTestingUtilities.AssertAllProperLength(rtts, statuses, buffers, addresses, ttls, 2);
        
        // Prepare the PingReply stubs which are stored on the pingerStub object, one of which will be returned
        // from each SendPingIndividualAsync function call that SendPingGroupAsync makes
        pingerStub.PrepareStubbedPingReplies(rtts, statuses, buffers, addresses, ttls);
        
        // Make PingScheduler object
        var pingScheduler = new PingScheduler(PingBehaviorOptions);
        
        // Make logger (though not implemented as of writing this)
        var logger = new NullLogger<IGroupPinger>();
        
        // Finally, make the GroupPinger
        var groupPinger = new GroupPinger(
            pingerStub,
            new PingQualification(PingThresholdsOptions),
            pingScheduler,
            PingBehaviorOptions,
            PingThresholdsOptions,
            logger
        );
        
        var summary = await groupPinger.SendPingGroupAsync(target, PingBehavior.PingsPerCall);
        Assert.Equal(5, summary.AveragePing!.Value);
        Assert.Equal(5, summary.MinimumPing!.Value);
        Assert.Equal(5, summary.MaximumPing!.Value);
        Assert.Equal(0, summary.Jitter);
        Assert.Equal(0, summary.ConsecutiveTimeouts!.Value);
        Assert.Equal(PingQualification.ThresholdExceededFlags.NotExceeded, summary.PingQualityFlags);
        Assert.Equal(0, summary.PacketLoss);
        Assert.Equal(2, summary.PacketsSent!.Value);
        Assert.Equal(0, summary.PacketsLost!.Value);
        Assert.Equal(1, summary.ExcludedPings!.Value);
        Assert.Equal(IPStatus.SourceQuench, summary.LastAbnormalStatus);
        Assert.Null(summary.TerminatingIPStatus);
    }
    
    //4) {Success, Halt}
    [Fact]
    public async void SendPingGroupAsync_ReturnsProperPingGroupSummary_OnSuccessHalt() {
        IndividualPingerStub pingerStub = new IndividualPingerStub();
        //The target we are going to pretend to ping
        var target = IPAddress.Parse("8.8.8.8");
        
        // Values for the fake PingReply objects that SendPingIndividualAsync will send, every PingReply is one index
        List<long> rtts = [5, 0];
        List<IPStatus> statuses = [IPStatus.Success, IPStatus.BadHeader];
        List<byte[]> buffers = [[], []];
        List<IPAddress> addresses = [target, target];
        List<int> ttls = [64, 64];
        
        // Make sure the Lists of PingReply values are the same length, didn't mess up the test setup
        PingingTestingUtilities.AssertAllProperLength(rtts, statuses, buffers, addresses, ttls, 2);
        
        // Prepare the PingReply stubs which are stored on the pingerStub object, one of which will be returned
        // from each SendPingIndividualAsync function call that SendPingGroupAsync makes
        pingerStub.PrepareStubbedPingReplies(rtts, statuses, buffers, addresses, ttls);
        
        // Make PingScheduler object
        var pingScheduler = new PingScheduler(PingBehaviorOptions);
        
        // Make logger (though not implemented as of writing this)
        var logger = new NullLogger<IGroupPinger>();
        
        // Finally, make the GroupPinger
        var groupPinger = new GroupPinger(
            pingerStub,
            new PingQualification(PingThresholdsOptions),
            pingScheduler,
            PingBehaviorOptions,
            PingThresholdsOptions,
            logger
        );
        
        var summary = await groupPinger.SendPingGroupAsync(target, PingBehavior.PingsPerCall);
        Assert.Equal(5, summary.AveragePing!.Value);
        Assert.Equal(5, summary.MinimumPing!.Value);
        Assert.Equal(5, summary.MaximumPing!.Value);
        Assert.Equal(0, summary.Jitter);
        Assert.Equal(0, summary.ConsecutiveTimeouts!.Value);
        Assert.Equal(PingQualification.ThresholdExceededFlags.NotExceeded, summary.PingQualityFlags);
        Assert.Equal(0, summary.PacketLoss);
        Assert.Equal(2, summary.PacketsSent!.Value);
        Assert.Equal(0, summary.PacketsLost!.Value);
        Assert.Equal(1, summary.ExcludedPings!.Value);
        Assert.Null(summary.LastAbnormalStatus);
        Assert.Equal(IPStatus.BadHeader, summary.TerminatingIPStatus);
    }
    
    // 5) {Success,PacketLossCaution}
    [Fact]
    public async void SendPingGroupAsync_ReturnsProperPingGroupSummary_OnSuccessPacketLossCaution() {
        IndividualPingerStub pingerStub = new IndividualPingerStub();
        //The target we are going to pretend to ping
        var target = IPAddress.Parse("8.8.8.8");
        
        // Values for the fake PingReply objects that SendPingIndividualAsync will send, every PingReply is one index
        List<long> rtts = [5, 0];
        List<IPStatus> statuses = [IPStatus.Success, IPStatus.TimedOut];
        List<byte[]> buffers = [[], []];
        List<IPAddress> addresses = [target, target];
        List<int> ttls = [64, 64];
        
        // Make sure the Lists of PingReply values are the same length, didn't mess up the test setup
        PingingTestingUtilities.AssertAllProperLength(rtts, statuses, buffers, addresses, ttls, 2);
        
        // Prepare the PingReply stubs which are stored on the pingerStub object, one of which will be returned
        // from each SendPingIndividualAsync function call that SendPingGroupAsync makes
        pingerStub.PrepareStubbedPingReplies(rtts, statuses, buffers, addresses, ttls);
        
        // Make PingScheduler object
        var pingScheduler = new PingScheduler(PingBehaviorOptions);
        
        // Make logger (though not implemented as of writing this)
        var logger = new NullLogger<IGroupPinger>();
        
        // Finally, make the GroupPinger
        var groupPinger = new GroupPinger(
            pingerStub,
            new PingQualification(PingThresholdsOptions),
            pingScheduler,
            PingBehaviorOptions,
            PingThresholdsOptions,
            logger
        );
        
        var summary = await groupPinger.SendPingGroupAsync(target, PingBehavior.PingsPerCall);
        Assert.Equal(5, summary.AveragePing!.Value);
        Assert.Equal(5, summary.MinimumPing!.Value);
        Assert.Equal(5, summary.MaximumPing!.Value);
        Assert.Equal(0, summary.Jitter);
        Assert.Equal(1, summary.ConsecutiveTimeouts!.Value);
        Assert.Equal(PingQualification.ThresholdExceededFlags.HighPacketLoss, summary.PingQualityFlags);
        Assert.Equal(50, summary.PacketLoss);
        Assert.Equal(2, summary.PacketsSent!.Value);
        Assert.Equal(1, summary.PacketsLost!.Value);
        Assert.Equal(0, summary.ExcludedPings!.Value);
        Assert.Equal(IPStatus.TimedOut, summary.LastAbnormalStatus);
        Assert.Null(summary.TerminatingIPStatus);
    }
    
    #endregion 
    /********************************************* Beginning With Success *********************************************/
    
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
        


        // Make PingScheduler object
        var pingScheduler = new PingScheduler(PingBehaviorOptions);
        
        // Make logger (though not implemented as of writing this)
        var logger = new NullLogger<IGroupPinger>();
        
        // Finally, make the GroupPinger
        var groupPinger = new GroupPinger(
            pingerStub,
            new PingQualification(PingThresholdsOptions),
            pingScheduler,
            PingBehaviorOptions,
            PingThresholdsOptions,
            logger
        );
        
        var summary = await groupPinger.SendPingGroupAsync(target, PingBehavior.PingsPerCall);
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
        
        // Make PingScheduler object
        var pingScheduler = new PingScheduler(PingBehaviorOptions);
        
        // Make logger (though not implemented as of writing this)
        var logger = new NullLogger<IGroupPinger>();
        
        // Finally, make the GroupPinger
        var groupPinger = new GroupPinger(
            pingerStub,
            new PingQualification(PingThresholdsOptions),
            pingScheduler,
            PingBehaviorOptions,
            PingThresholdsOptions,
            logger
        );
        
        var summary = await groupPinger.SendPingGroupAsync(target, PingBehavior.PingsPerCall);
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
        
        // Make PingScheduler object
        var pingScheduler = new PingScheduler(PingBehaviorOptions);
        
        // Make logger (though not implemented as of writing this)
        var logger = new NullLogger<IGroupPinger>();
        
        // Finally, make the GroupPinger
        var groupPinger = new GroupPinger(
            pingerStub,
            new PingQualification(PingThresholdsOptions),
            pingScheduler,
            PingBehaviorOptions,
            PingThresholdsOptions,
            logger
        );
        
        var summary = await groupPinger.SendPingGroupAsync(target, PingBehavior.PingsPerCall);
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
  
}