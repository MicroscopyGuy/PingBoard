namespace PingBoard.Tests.PingingTests;
using Microsoft.Extensions.Logging.Abstractions;
using PingBoard.Pinging.Configuration;
using PingBoard.Pinging;
using System.Net.NetworkInformation;
using System.Net;

/// <summary>
/// The unit tests that begin with "Halts" are a bit funny, since that should immediately cause the
/// SendPingGroupAsync function call to terminate. To that end, all the tests in this file effectively
/// confirm two things. 1) That SendPingGroupAsync stops at the Halt state, but more importantly 2),
/// that all the values left on the PingGroupSummary object are sensible and proper.
///
/// |*************************************************Combinations:****************************************************|
/// | 1) {Success,Success}                                                                                             |
/// | 2) {Success,Unsuccessful}                                                                                        |
/// | 3) {Success,Pause}                                                                                               |
/// | 4) {Success,Halt}                                                                                                |
/// | 5) {Success,PacketLossCaution}                                                                                   |
/// | 6) {Unsuccessful,Success}                                                                                        | 
/// | 7) {Unsuccessful,Unsuccessful}                                                                                   |
/// | 8) {Unsuccessful,Pause}                                                                                          |
/// | 9) {Unsuccessful,Halt}                                                                                           |
/// | 10) {Unsuccessful,PacketLossCaution}                                                                             |
/// | 11) {Pause,Success}                                                                                              |
/// | 12) {Pause,Unsuccessful}                                                                                         |
/// | 13) {Pause,Pause}                                                                                                |
/// | 14) {Pause,Halt}                                                                                                 |
/// | 15) {Pause,PacketLossCaution}                                                                                    |
/// | 16) {Halt,Success}                        ←- Start of Group D ^                                                  |
/// | 17) {Halt,Unsuccessful}                                       |                                                  |
/// | 18) {Halt,Pause}                                              |                                                  |
/// | 19) {Halt,Halt}                                               |                                                  |
/// | 20) {Halt,PacketLossCaution}              ←- End of Group D   v                                                  |
/// | 21) {PacketLossCaution,Success}                                                                                  |
/// | 22) {PacketLossCaution,Unsuccessful}                                                                             |
/// | 23) {PacketLossCaution,Pause}                                                                                    |
/// | 24) {PacketLossCaution,Halt}                                                                                     |
/// | 25) {PacketLossCaution,PacketLossCaution}                                                                        |
/// |******************************************************************************************************************|
/// </summary>

public partial class SendPingGroupAsyncTestingTwoPings
{
   // 16) {Halt,Success}
   [Fact]
   public async void SendPingGroupAsync_ReturnsProperPingGroupSummary_OnHaltSuccess() {
        IndividualPingerStub pingerStub = new IndividualPingerStub();
        //The target we are going to pretend to ping
        var target = IPAddress.Parse("8.8.8.8");
        
        // Values for the fake PingReply objects that SendPingIndividualAsync will send, every PingReply is one index
        List<long> rtts = [0, 0];
        List<IPStatus> statuses = [IPStatus.Unknown, IPStatus.Success];
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
        Assert.Equal(1, summary.PacketsSent!.Value);
        Assert.Equal(0, summary.PacketsLost!.Value);
        Assert.Null(summary.LastAbnormalStatus);
        Assert.Equal(IPStatus.Unknown, summary.TerminatingIPStatus);
   }
   
   // 17) {Halt,Unsuccessful}
   [Fact]
   public async void SendPingGroupAsync_ReturnsProperPingGroupSummary_OnHaltUnsuccessful() {
        IndividualPingerStub pingerStub = new IndividualPingerStub();
        //The target we are going to pretend to ping
        var target = IPAddress.Parse("8.8.8.8");
        
        // Values for the fake PingReply objects that SendPingIndividualAsync will send, every PingReply is one index
        List<long> rtts = [0, 0];
        List<IPStatus> statuses = [IPStatus.Unknown, IPStatus.BadRoute];
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
        Assert.Equal(1, summary.PacketsSent!.Value);
        Assert.Equal(0, summary.PacketsLost!.Value);
        Assert.Null(summary.LastAbnormalStatus);
        Assert.Equal(IPStatus.Unknown, summary.TerminatingIPStatus);
   }
   
   // 18) {Halt,Pause}      
   [Fact]
   public async void SendPingGroupAsync_ReturnsProperPingGroupSummary_OnHaltPause() {
        IndividualPingerStub pingerStub = new IndividualPingerStub();
        //The target we are going to pretend to ping
        var target = IPAddress.Parse("8.8.8.8");
        
        // Values for the fake PingReply objects that SendPingIndividualAsync will send, every PingReply is one index
        List<long> rtts = [0, 0];
        List<IPStatus> statuses = [IPStatus.Unknown, IPStatus.SourceQuench];
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
        Assert.Equal(1, summary.PacketsSent!.Value);
        Assert.Equal(0, summary.PacketsLost!.Value);
        Assert.Null(summary.LastAbnormalStatus);
        Assert.Equal(IPStatus.Unknown, summary.TerminatingIPStatus);
   }
   
   // 19) {Halt,Halt}  
   [Fact]
   public async void SendPingGroupAsync_ReturnsProperPingGroupSummary_OnHaltHalt() {
        IndividualPingerStub pingerStub = new IndividualPingerStub();
        //The target we are going to pretend to ping
        var target = IPAddress.Parse("8.8.8.8");
        
        // Values for the fake PingReply objects that SendPingIndividualAsync will send, every PingReply is one index
        List<long> rtts = [0, 0];
        List<IPStatus> statuses = [IPStatus.Unknown, IPStatus.BadRoute];
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
        Assert.Equal(1, summary.PacketsSent!.Value);
        Assert.Equal(0, summary.PacketsLost!.Value);
        Assert.Null(summary.LastAbnormalStatus);
        Assert.Equal(IPStatus.Unknown, summary.TerminatingIPStatus);
   }
   
   //20) {Halt,PacketLossCaution}
   [Fact]
   public async void SendPingGroupAsync_ReturnsProperPingGroupSummary_OnHaltPacketLossCaution() {
        IndividualPingerStub pingerStub = new IndividualPingerStub();
        //The target we are going to pretend to ping
        var target = IPAddress.Parse("8.8.8.8");
        
        // Values for the fake PingReply objects that SendPingIndividualAsync will send, every PingReply is one index
        List<long> rtts = [0, 0];
        List<IPStatus> statuses = [IPStatus.Unknown, IPStatus.TimedOut];
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
        Assert.Equal(1, summary.PacketsSent!.Value);
        Assert.Equal(0, summary.PacketsLost!.Value);
        Assert.Null(summary.LastAbnormalStatus);
        Assert.Equal(IPStatus.Unknown, summary.TerminatingIPStatus);
   }
    
   
}