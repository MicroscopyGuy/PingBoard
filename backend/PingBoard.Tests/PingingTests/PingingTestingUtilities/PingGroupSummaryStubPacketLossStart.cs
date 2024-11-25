using Microsoft.Extensions.Options;
using PingBoard.Database.Models;

namespace PingBoard.Tests.PingingTests.PingingTestingUtilities;
using Microsoft.Extensions.Logging.Abstractions;
using PingBoard.Pinging.Configuration;
using PingBoard.Pinging;
using System.Net.NetworkInformation;
using System.Net;

/// <summary>
/// Stubbed PingGroupSummary objects are needed for unit testing SendPingGroupAsync as well as for integration testing
/// with the database. This class provides functions that stub PingGroupSummary objects with 25 different sets of
/// values, representing every permutation of behavior that SendPingGroupAsync can experience and/or produce.
///
/// Note that each function relies on the IOptions-wrapped PingingThresholdsConfig and PingingBehaviorConfig objects
/// passed to them. This allows the flexibility to meet the separate needs of integration and unit testing.
///
/// Finally, note that this stubbing intentionally uses SendPingGroupAsync to ensure proper behavior. If any
/// arbitrary PingGroupSummary object is desired, this class is not needed. This class intentionally uses
/// SendPingGroupAsync to facilitate its unit and integration testing.
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

public static partial class PingGroupSummaryStub{
    
   //21) {PacketLossCaution, Success}
   public static async Task<PingGroupSummary> GeneratePacketLossSuccess(IOptions<PingingBehaviorConfig> behaviorOptions, 
       IOptions<PingingThresholdsConfig> thresholdsOptions){
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
        PingingTestingMiscUtilities.AssertAllProperLength(rtts, statuses, buffers, addresses, ttls, 2);
        
        // Prepare the PingReply stubs which are stored on the pingerStub object, one of which will be returned
        // from each SendPingIndividualAsync function call that SendPingGroupAsync makes
        pingerStub.PrepareStubbedPingReplies(rtts, statuses, buffers, addresses, ttls);
        
        // Make PingScheduler object
        var pingScheduler = new PingScheduler(behaviorOptions);
        
        // Make logger (though not implemented as of writing this)
        var logger = new NullLogger<IGroupPinger>();
        
        // Finally, make the GroupPinger
        var groupPinger = new GroupPinger(
            pingerStub,
            new PingGroupQualifier(thresholdsOptions),
            pingScheduler,
            behaviorOptions,
            thresholdsOptions,
            logger
        );
        
        var summary = await groupPinger.SendPingGroupAsync(target);
        return summary;
   }
   
   // 22) {PacketLossCaution,Unsuccessful} 
   public static async Task<PingGroupSummary> GeneratePacketLossUnsuccessful(IOptions<PingingBehaviorConfig> behaviorOptions, 
       IOptions<PingingThresholdsConfig> thresholdsOptions) {
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
        PingingTestingMiscUtilities.AssertAllProperLength(rtts, statuses, buffers, addresses, ttls, 2);
        
        // Prepare the PingReply stubs which are stored on the pingerStub object, one of which will be returned
        // from each SendPingIndividualAsync function call that SendPingGroupAsync makes
        pingerStub.PrepareStubbedPingReplies(rtts, statuses, buffers, addresses, ttls);
        
        // Make PingScheduler object
        var pingScheduler = new PingScheduler(behaviorOptions);
        
        // Make logger (though not implemented as of writing this)
        var logger = new NullLogger<IGroupPinger>();
        
        // Finally, make the GroupPinger
        var groupPinger = new GroupPinger(
            pingerStub,
            new PingGroupQualifier(thresholdsOptions),
            pingScheduler,
            behaviorOptions,
            thresholdsOptions,
            logger
        );
        
        var summary = await groupPinger.SendPingGroupAsync(target);
        return summary;
   }
   
   // 23) {PacketLossCaution,Pause}
   public static async Task<PingGroupSummary> GeneratePacketLossPause(IOptions<PingingBehaviorConfig> behaviorOptions, 
       IOptions<PingingThresholdsConfig> thresholdsOptions) {
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
        PingingTestingMiscUtilities.AssertAllProperLength(rtts, statuses, buffers, addresses, ttls, 2);
        
        // Prepare the PingReply stubs which are stored on the pingerStub object, one of which will be returned
        // from each SendPingIndividualAsync function call that SendPingGroupAsync makes
        pingerStub.PrepareStubbedPingReplies(rtts, statuses, buffers, addresses, ttls);
        
        // Make PingScheduler object
        var pingScheduler = new PingScheduler(behaviorOptions);
        
        // Make logger (though not implemented as of writing this)
        var logger = new NullLogger<IGroupPinger>();
        
        // Finally, make the GroupPinger
        var groupPinger = new GroupPinger(
            pingerStub,
            new PingGroupQualifier(thresholdsOptions),
            pingScheduler,
            behaviorOptions,
            thresholdsOptions,
            logger
        );
        
        var summary = await groupPinger.SendPingGroupAsync(target);
        return summary;
   }
   
   // 24) {PacketLossCaution,Halt}
   public static async Task<PingGroupSummary> GeneratePacketLossHalt(IOptions<PingingBehaviorConfig> behaviorOptions, 
       IOptions<PingingThresholdsConfig> thresholdsOptions) {
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
        PingingTestingMiscUtilities.AssertAllProperLength(rtts, statuses, buffers, addresses, ttls, 2);
        
        // Prepare the PingReply stubs which are stored on the pingerStub object, one of which will be returned
        // from each SendPingIndividualAsync function call that SendPingGroupAsync makes
        pingerStub.PrepareStubbedPingReplies(rtts, statuses, buffers, addresses, ttls);
        
        // Make PingScheduler object
        var pingScheduler = new PingScheduler(behaviorOptions);
        
        // Make logger (though not implemented as of writing this)
        var logger = new NullLogger<IGroupPinger>();
        
        // Finally, make the GroupPinger
        var groupPinger = new GroupPinger(
            pingerStub,
            new PingGroupQualifier(thresholdsOptions),
            pingScheduler,
            behaviorOptions,
            thresholdsOptions,
            logger
        );
        
        var summary = await groupPinger.SendPingGroupAsync(target);
        return summary;
   }
   
   // 25) {PacketLossCaution,PacketLossCaution}
   public static async Task<PingGroupSummary> GeneratePacketLossPacketLoss(IOptions<PingingBehaviorConfig> behaviorOptions, 
       IOptions<PingingThresholdsConfig> thresholdsOptions) {
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
        PingingTestingMiscUtilities.AssertAllProperLength(rtts, statuses, buffers, addresses, ttls, 2);
        
        // Prepare the PingReply stubs which are stored on the pingerStub object, one of which will be returned
        // from each SendPingIndividualAsync function call that SendPingGroupAsync makes
        pingerStub.PrepareStubbedPingReplies(rtts, statuses, buffers, addresses, ttls);
        
        // Make PingScheduler object
        var pingScheduler = new PingScheduler(behaviorOptions);
        
        // Make logger (though not implemented as of writing this)
        var logger = new NullLogger<IGroupPinger>();
        
        // Finally, make the GroupPinger
        var groupPinger = new GroupPinger(
            pingerStub,
            new PingGroupQualifier(thresholdsOptions),
            pingScheduler,
            behaviorOptions,
            thresholdsOptions,
            logger
        );
        
        var summary = await groupPinger.SendPingGroupAsync(target);
        return summary;
   }
   
}