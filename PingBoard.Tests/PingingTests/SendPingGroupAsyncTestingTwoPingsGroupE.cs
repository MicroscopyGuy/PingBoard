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
/// | 16) {Halt,Success}                                                                                               |
/// | 17) {Halt,Unsuccessful}                                                                                          |
/// | 18) {Halt,Pause}                                                                                                 |
/// | 19) {Halt,Halt}                                                                                                  |
/// | 20) {Halt,PacketLossCaution}                                                                                     |  
/// | 21) {PacketLossCaution,Success}            ←- Start of Group E ^                                                 |
/// | 22) {PacketLossCaution,Unsuccessful}                           |                                                 |
/// | 23) {PacketLossCaution,Pause}                                  |                                                 |
/// | 24) {PacketLossCaution,Halt}                                   |                                                 |
/// | 25) {PacketLossCaution,PacketLossCaution}  ←- End of Group E   v                                                 |
/// |******************************************************************************************************************|
/// </summary>

public partial class SendPingGroupAsyncTestingTwoPings{
    
   //21) {PacketLossCaution, Success}
   [Fact]
   public async void SendPingGroupAsync_ReturnsProperPingGroupSummary_PacketLossCautionSuccess() {
        IndividualPingerStub pingerStub = new IndividualPingerStub();
        //The target we are going to pretend to ping
        var target = IPAddress.Parse("8.8.8.8");
        
        // Values for the fake PingReply objects that SendPingIndividualAsync will send, every PingReply is one index
        List<long> rtts = [0, 6];
        List<IPStatus> statuses = [IPStatus.TimedOut, IPStatus.Success];
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
        Assert.Equal(6, summary.AveragePing!.Value);
        Assert.Equal(6, summary.MinimumPing!.Value);
        Assert.Equal(6, summary.MaximumPing!.Value);
        Assert.Equal(0, summary.Jitter);
        Assert.Equal(0, summary.ConsecutiveTimeouts!.Value);
        Assert.Equal(
            PingQualification.ThresholdExceededFlags.HighMinimumPing
                  | PingQualification.ThresholdExceededFlags.HighPacketLoss, summary.PingQualityFlags);
        Assert.Equal(50, summary.PacketLoss);
        Assert.Equal(2, summary.PacketsSent!.Value);
        Assert.Equal(1, summary.PacketsLost!.Value);
        Assert.Equal(1, summary.ExcludedPings!.Value);
        Assert.Equal(IPStatus.TimedOut, summary.LastAbnormalStatus);
        Assert.Null(summary.TerminatingIPStatus);
   }
   
   // 22) {PacketLossCaution,Unsuccessful} 
   [Fact]
   public async void SendPingGroupAsync_ReturnsProperPingGroupSummary_OnPacketLossCautionUnsuccessful() {
        IndividualPingerStub pingerStub = new IndividualPingerStub();
        //The target we are going to pretend to ping
        var target = IPAddress.Parse("8.8.8.8");
        
        // Values for the fake PingReply objects that SendPingIndividualAsync will send, every PingReply is one index
        List<long> rtts = [0, 0];
        List<IPStatus> statuses = [IPStatus.TimedOut, IPStatus.BadRoute];
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
        Assert.Equal(PingQualification.ThresholdExceededFlags.HighPacketLoss, summary.PingQualityFlags);
        Assert.Equal(50, summary.PacketLoss);
        Assert.Equal(2, summary.PacketsSent!.Value);
        Assert.Equal(1, summary.PacketsLost!.Value);
        Assert.Equal(2, summary.ExcludedPings!.Value);
        Assert.Equal(IPStatus.BadRoute, summary.LastAbnormalStatus);
        Assert.Null(summary.TerminatingIPStatus);
   }
   
   // 23) {PacketLossCaution,Pause}
   [Fact]
   public async void SendPingGroupAsync_ReturnsProperPingGroupSummary_OnPacketLossCautionPause() {
        IndividualPingerStub pingerStub = new IndividualPingerStub();
        //The target we are going to pretend to ping
        var target = IPAddress.Parse("8.8.8.8");
        
        // Values for the fake PingReply objects that SendPingIndividualAsync will send, every PingReply is one index
        List<long> rtts = [0, 0];
        List<IPStatus> statuses = [IPStatus.TimedOut, IPStatus.SourceQuench];
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
        Assert.Equal(PingQualification.ThresholdExceededFlags.HighPacketLoss, summary.PingQualityFlags);
        Assert.Equal(50, summary.PacketLoss);
        Assert.Equal(2, summary.PacketsSent!.Value);
        Assert.Equal(1, summary.PacketsLost!.Value);
        Assert.Equal(2, summary.ExcludedPings!.Value);
        Assert.Equal(IPStatus.SourceQuench, summary.LastAbnormalStatus);
        Assert.Null(summary.TerminatingIPStatus);
   }
   // 24) {PacketLossCaution,Halt}
   [Fact]
   public async void SendPingGroupAsync_ReturnsProperPingGroupSummary_OnPacketLossCautionHalt() {
        IndividualPingerStub pingerStub = new IndividualPingerStub();
        //The target we are going to pretend to ping
        var target = IPAddress.Parse("8.8.8.8");
        
        // Values for the fake PingReply objects that SendPingIndividualAsync will send, every PingReply is one index
        List<long> rtts = [0, 0];
        List<IPStatus> statuses = [IPStatus.TimedOut, IPStatus.Unknown];
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
        Assert.Equal(PingQualification.ThresholdExceededFlags.HighPacketLoss, summary.PingQualityFlags);
        Assert.Equal(50, summary.PacketLoss);
        Assert.Equal(2, summary.PacketsSent!.Value);
        Assert.Equal(1, summary.PacketsLost!.Value);
        Assert.Equal(2, summary.ExcludedPings!.Value);
        Assert.Equal(IPStatus.TimedOut, summary.LastAbnormalStatus);
        Assert.Equal(IPStatus.Unknown, summary.TerminatingIPStatus);
   }
   
   // 25) {PacketLossCaution,PacketLossCaution}
   [Fact]
   public async void SendPingGroupAsync_ReturnsProperPingGroupSummary_OnPacketLossCautionPacketLossCaution() {
        IndividualPingerStub pingerStub = new IndividualPingerStub();
        //The target we are going to pretend to ping
        var target = IPAddress.Parse("8.8.8.8");
        
        // Values for the fake PingReply objects that SendPingIndividualAsync will send, every PingReply is one index
        List<long> rtts = [0, 0];
        List<IPStatus> statuses = [IPStatus.TimedOut, IPStatus.TimedOut];
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
        Assert.Equal(2, summary.ConsecutiveTimeouts!.Value);
        Assert.Equal(PingQualification.ThresholdExceededFlags.HighPacketLoss, summary.PingQualityFlags);
        Assert.Equal(100, summary.PacketLoss);
        Assert.Equal(2, summary.PacketsSent!.Value);
        Assert.Equal(2, summary.PacketsLost!.Value);
        Assert.Equal(2, summary.ExcludedPings!.Value);
        Assert.Equal(IPStatus.TimedOut, summary.LastAbnormalStatus);
        Assert.Null(summary.TerminatingIPStatus);
   }
   
}