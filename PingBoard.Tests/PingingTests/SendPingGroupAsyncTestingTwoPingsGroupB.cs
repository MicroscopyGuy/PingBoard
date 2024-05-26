namespace PingBoard.Tests.PingingTests;
using Microsoft.Extensions.Logging.Abstractions;
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
/// | 1) {Success,Success}                                                                                              |
/// | 2) {Success,Unsuccessful}                                                                                         |
/// | 3) {Success,Pause}                                                                                                |
/// | 4) {Success,Halt}                                                                                                 |
/// | 5) {Success,PacketLossCaution}                                                                                    |
/// | 6) {Unsuccessful,Success}                ←- Start of Group B  ^                                                   | 
/// | 7) {Unsuccessful,Unsuccessful}                                |                                                   |
/// | 8) {Unsuccessful,Pause}                                       |                                                   |
/// | 9) {Unsuccessful,Halt}                                        |                                                   |
/// | 10) {Unsuccessful,PacketLossCaution}     ←- End of Group B    v                                                   |
/// | 11) {Pause,Success}                                                                                               |
/// | 12) {Pause,Unsuccessful}                                                                                          |
/// | 13) {Pause,Pause}                                                                                                 |
/// | 14) {Pause,Halt}                                                                                                  |
/// | 15) {Pause,PacketLossCaution}                                                                                     |
/// | 16) {Halt,Success}                                                                                                |
/// | 17) {Halt,Unsuccessful}                                                                                           |
/// | 18) {Halt,Pause}                                                                                                  |
/// | 19) {Halt,Halt}                                                                                                   |
/// | 20) {Halt,PacketLossCaution}                                                                                      |
/// | 21) {PacketLossCaution,Success}                                                                                   |
/// | 22) {PacketLossCaution,Unsuccessful}                                                                              |
/// | 23) {PacketLossCaution,Pause}                                                                                     |
/// | 24) {PacketLossCaution,Halt}                                                                                      |
/// | 25) {PacketLossCaution,PacketLossCaution}                                                                         |
/// |******************************************************************************************************************|
/// </summary>

public partial class SendPingGroupAsyncTestingTwoPings {
    // 6) {Unsuccessful,Success}
    [Fact]
    public async void SendPingGroupAsync_ReturnsProperPingGroupSummary_OnUnsuccessfulSuccess() {
        IndividualPingerStub pingerStub = new IndividualPingerStub();
        //The target we are going to pretend to ping
        var target = IPAddress.Parse("8.8.8.8");
        
        // Values for the fake PingReply objects that SendPingIndividualAsync will send, every PingReply is one index
        List<long> rtts = [0, 4];
        List<IPStatus> statuses = [IPStatus.BadRoute, IPStatus.Success];
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
        Assert.Equal(4, summary.AveragePing!.Value);
        Assert.Equal(4, summary.MinimumPing!.Value);
        Assert.Equal(4, summary.MaximumPing!.Value);
        Assert.Equal(0, summary.Jitter);
        Assert.Equal(0, summary.ConsecutiveTimeouts!.Value);
        Assert.Equal(PingQualification.ThresholdExceededFlags.NotExceeded, summary.PingQualityFlags);
        Assert.Equal(0, summary.PacketLoss);
        Assert.Equal(2, summary.PacketsSent!.Value);
        Assert.Equal(0, summary.PacketsLost!.Value);
        Assert.Equal(1, summary.ExcludedPings!.Value);
        Assert.Equal(IPStatus.BadRoute, summary.LastAbnormalStatus);
        Assert.Null(summary.TerminatingIPStatus);
    }
    
    // 7) {Unsuccessful,Unsuccessful} 
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
    
    //8) {Unsuccessful,Pause} 
    [Fact]
    public async void SendPingGroupAsync_ReturnsProperPingGroupSummary_OnUnsuccessfulPause() {
        IndividualPingerStub pingerStub = new IndividualPingerStub();
        //The target we are going to pretend to ping
        var target = IPAddress.Parse("8.8.8.8");
        
        // Values for the fake PingReply objects that SendPingIndividualAsync will send, every PingReply is one index
        List<long> rtts = [0, 0];
        List<IPStatus> statuses = [IPStatus.BadRoute, IPStatus.SourceQuench];
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
        Assert.Equal(2, summary.ExcludedPings!.Value);
        Assert.Equal(IPStatus.SourceQuench, summary.LastAbnormalStatus);
        Assert.Null(summary.TerminatingIPStatus);
    }
    
    // 9) {Unsuccessful,Halt}
    [Fact]
    public async void SendPingGroupAsync_ReturnsProperPingGroupSummary_OnUnsuccessfulHalt() {
        IndividualPingerStub pingerStub = new IndividualPingerStub();
        //The target we are going to pretend to ping
        var target = IPAddress.Parse("8.8.8.8");
        
        // Values for the fake PingReply objects that SendPingIndividualAsync will send, every PingReply is one index
        List<long> rtts = [0, 0];
        List<IPStatus> statuses = [IPStatus.BadRoute, IPStatus.BadOption];
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
        Assert.Equal(2, summary.ExcludedPings!.Value);
        Assert.Equal(IPStatus.BadRoute, summary.LastAbnormalStatus);
        Assert.Equal(IPStatus.BadOption, summary.TerminatingIPStatus);
    }
    
    // 10) {Unsuccessful,PacketLossCaution}
    [Fact]
    public async void SendPingGroupAsync_ReturnsProperPingGroupSummary_OnUnsuccessfulPacketLossCaution() {
        IndividualPingerStub pingerStub = new IndividualPingerStub();
        //The target we are going to pretend to ping
        var target = IPAddress.Parse("8.8.8.8");
        
        // Values for the fake PingReply objects that SendPingIndividualAsync will send, every PingReply is one index
        List<long> rtts = [0, 0];
        List<IPStatus> statuses = [IPStatus.BadRoute, IPStatus.TimedOut];
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
        Assert.Equal(1, summary.ConsecutiveTimeouts!.Value);
        Assert.Equal(PingQualification.ThresholdExceededFlags.HighPacketLoss, summary.PingQualityFlags);
        Assert.Equal(50, summary.PacketLoss);
        Assert.Equal(2, summary.PacketsSent!.Value);
        Assert.Equal(1, summary.PacketsLost!.Value);
        Assert.Equal(2, summary.ExcludedPings!.Value);
        Assert.Equal(IPStatus.TimedOut, summary.LastAbnormalStatus);
        Assert.Null(summary.TerminatingIPStatus);
    }
}