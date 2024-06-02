using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace PingBoard.Tests.PingingTests;
using PingBoard.Tests.PingingTests.PingingTestingUtilities;
using PingBoard.Pinging.Configuration;
using PingBoard.Pinging;
using PingBoard.Tests.PingingTests.PingingTestingUtilities;
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
/// |*************************************************Combinations:****************************************************|
/// |1) {Success,Success}                     ←- Start of Group A  ^                                                   |
/// |2) {Success,Unsuccessful}                                     |                                                   |
/// |3) {Success,Pause}                                            |                                                   |
/// |4) {Success,Halt}                                             |                                                   |
/// |5) {Success,PacketLossCaution}           ←- End of Group A    v                                                   |
/// |6) {Unsuccessful,Success}                                                                                         | 
/// |7) {Unsuccessful,Unsuccessful}                                                                                                |
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
        WaitMs = 20 // technically violates the WaitMs limits, but the wait time isn't relevant here
    };

    private static readonly IOptions<PingingBehaviorConfig> PingBehaviorOptions = Options.Create(PingBehavior);
    
    //1) {Success,Success}
    [Fact]
    public async void SendPingGroupAsync_ReturnsProperPingGroupSummary_OnSuccessSuccess()
    {
        //var summary = await PingGroupSummaryStub
            //.GenerateSuccessSuccess(PingBehaviorOptions, PingThresholdsOptions);
        
        /*
        Assert.Equal(4.5, summary.AveragePing);
        Assert.Equal(4, summary.MinimumPing);
        Assert.Equal(5, summary.MaximumPing);
        Assert.Equal(1, summary.Jitter);
        Assert.Equal(0, summary.ConsecutiveTimeouts);
        Assert.Equal(PingQualification.ThresholdExceededFlags.NotExceeded, summary.PingQualityFlags);
        Assert.Equal(0, summary.PacketLoss);
        Assert.Equal(2, summary.PacketsSent);
        Assert.Equal(0, summary.PacketsLost);
        Assert.Null(summary.LastAbnormalStatus);
        Assert.Null(summary.TerminatingIPStatus);*/
        
        PingGroupSummaryExpectedValues.AssertExpectedValues(
            PingGroupSummaryExpectedValues.ExpectedSummaries["GenerateSuccessSuccess"], 
            await PingGroupSummaryStub.GenerateSuccessSuccess(PingBehaviorOptions, PingThresholdsOptions)
        );
        
        
    }
    
    // 2) {Success,Unsuccessful}
    [Fact]
    public async void SendPingGroupAsync_ReturnsProperPingGroupSummary_OnSuccessUnsuccessful(){
        var summary = await PingGroupSummaryStub
            .GenerateSuccessUnsuccessful(PingBehaviorOptions, PingThresholdsOptions);
        
        Assert.Equal(5, summary.AveragePing);
        Assert.Equal(5, summary.MinimumPing);
        Assert.Equal(5, summary.MaximumPing);
        Assert.Equal(0, summary.Jitter);
        Assert.Equal(0, summary.ConsecutiveTimeouts);
        Assert.Equal(PingQualification.ThresholdExceededFlags.NotExceeded, summary.PingQualityFlags);
        Assert.Equal(0, summary.PacketLoss);
        Assert.Equal(2, summary.PacketsSent);
        Assert.Equal(0, summary.PacketsLost);
        Assert.Equal(1, summary.ExcludedPings);
        Assert.Equal(IPStatus.DestinationPortUnreachable, summary.LastAbnormalStatus);
        Assert.Null(summary.TerminatingIPStatus);
    }
    
    // 3) {Success,Pause}
    [Fact]
    public async void SendPingGroupAsync_ReturnsProperPingGroupSummary_OnSuccessPause(){
        
        var summary = await PingGroupSummaryStub
            .GenerateSuccessPause(PingBehaviorOptions, PingThresholdsOptions);
        Assert.Equal(5, summary.AveragePing);
        Assert.Equal(5, summary.MinimumPing);
        Assert.Equal(5, summary.MaximumPing);
        Assert.Equal(0, summary.Jitter);
        Assert.Equal(0, summary.ConsecutiveTimeouts);
        Assert.Equal(PingQualification.ThresholdExceededFlags.NotExceeded, summary.PingQualityFlags);
        Assert.Equal(0, summary.PacketLoss);
        Assert.Equal(2, summary.PacketsSent);
        Assert.Equal(0, summary.PacketsLost);
        Assert.Equal(1, summary.ExcludedPings);
        Assert.Equal(IPStatus.SourceQuench, summary.LastAbnormalStatus);
        Assert.Null(summary.TerminatingIPStatus);
    }
    
    //4) {Success, Halt}
    [Fact]
    public async void SendPingGroupAsync_ReturnsProperPingGroupSummary_OnSuccessHalt() {
        
        var summary = await PingGroupSummaryStub
            .GenerateSuccessHalt(PingBehaviorOptions, PingThresholdsOptions);
        Assert.Equal(5, summary.AveragePing);
        Assert.Equal(5, summary.MinimumPing);
        Assert.Equal(5, summary.MaximumPing);
        Assert.Equal(0, summary.Jitter);
        Assert.Equal(0, summary.ConsecutiveTimeouts);
        Assert.Equal(PingQualification.ThresholdExceededFlags.NotExceeded, summary.PingQualityFlags);
        Assert.Equal(0, summary.PacketLoss);
        Assert.Equal(2, summary.PacketsSent);
        Assert.Equal(0, summary.PacketsLost);
        Assert.Equal(1, summary.ExcludedPings);
        Assert.Null(summary.LastAbnormalStatus);
        Assert.Equal(IPStatus.BadHeader, summary.TerminatingIPStatus);
    }
    
    // 5) {Success,PacketLossCaution}
    [Fact]
    public async void SendPingGroupAsync_ReturnsProperPingGroupSummary_OnSuccessPacketLossCaution() {
        var summary = await PingGroupSummaryStub
            .GenerateSuccessPacketLoss(PingBehaviorOptions, PingThresholdsOptions);
        Assert.Equal(5, summary.AveragePing);
        Assert.Equal(5, summary.MinimumPing);
        Assert.Equal(5, summary.MaximumPing);
        Assert.Equal(0, summary.Jitter);
        Assert.Equal(1, summary.ConsecutiveTimeouts);
        Assert.Equal(PingQualification.ThresholdExceededFlags.HighPacketLoss, summary.PingQualityFlags);
        Assert.Equal(50, summary.PacketLoss);
        Assert.Equal(2, summary.PacketsSent);
        Assert.Equal(1, summary.PacketsLost);
        Assert.Equal(1, summary.ExcludedPings);
        Assert.Equal(IPStatus.TimedOut, summary.LastAbnormalStatus);
        Assert.Null(summary.TerminatingIPStatus);
    }
}