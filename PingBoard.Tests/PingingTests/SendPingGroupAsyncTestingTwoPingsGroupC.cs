using PingBoard.Tests.PingingTests.PingingTestingUtilities;

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
/// | 11) {Pause,Success}                      ←- Start of Group C  ^                                                  |
/// | 12) {Pause,Unsuccessful}                                      |                                                  |
/// | 13) {Pause,Pause}                                             |                                                  |
/// | 14) {Pause,Halt}                                              |                                                  |
/// | 15) {Pause,PacketLossCaution}            ←- End of Group C    v                                                  |
/// | 16) {Halt,Success}                                                                                               |
/// | 17) {Halt,Unsuccessful}                                                                                          |
/// | 18) {Halt,Pause}                                                                                                 |
/// | 19) {Halt,Halt}                                                                                                  |
/// | 20) {Halt,PacketLossCaution}                                                                                     |
/// | 21) {PacketLossCaution,Success}                                                                                  |
/// | 22) {PacketLossCaution,Unsuccessful}                                                                             |
/// | 23) {PacketLossCaution,Pause}                                                                                    |
/// | 24) {PacketLossCaution,Halt}                                                                                     |
/// | 25) {PacketLossCaution,PacketLossCaution}                                                                        |
/// |******************************************************************************************************************|
/// </summary>

public partial class SendPingGroupAsyncTestingTwoPings
{
    // 11) {Pause,Success}
    [Fact]
    public async void SendPingGroupAsync_ReturnsProperPingGroupSummary_PauseSuccess() {
        var summary = await PingGroupSummaryStub
            .GeneratePauseSuccess(PingBehaviorOptions, PingThresholdsOptions);
        Assert.Equal(60, summary.AveragePing);
        Assert.Equal(60, summary.MinimumPing);
        Assert.Equal(60, summary.MaximumPing);
        Assert.Equal(0, summary.Jitter);
        Assert.Equal(0, summary.ConsecutiveTimeouts);
        Assert.NotEqual(PingQualification.ThresholdExceededFlags.NotExceeded, summary.PingQualityFlags);
        Assert.Equal(0, summary.PacketLoss);
        Assert.Equal(2, summary.PacketsSent);
        Assert.Equal(0, summary.PacketsLost);
        Assert.Equal(1, summary.ExcludedPings);
        Assert.Equal(IPStatus.SourceQuench, summary.LastAbnormalStatus);
        Assert.Null(summary.TerminatingIPStatus);
    }
    
    // 12) {Pause,Unsuccessful}
    [Fact]
    public async void SendPingGroupAsync_ReturnsProperPingGroupSummary_OnPauseUnsuccessful() {
        var summary = await PingGroupSummaryStub
            .GeneratePauseUnsuccessful(PingBehaviorOptions, PingThresholdsOptions);
        Assert.Equal(0, summary.AveragePing);
        Assert.Equal(0, summary.MinimumPing);
        Assert.Equal(0, summary.MaximumPing);
        Assert.Equal(0, summary.Jitter);
        Assert.Equal(0, summary.ConsecutiveTimeouts);
        Assert.Equal(PingQualification.ThresholdExceededFlags.NotExceeded, summary.PingQualityFlags);
        Assert.Equal(0, summary.PacketLoss);
        Assert.Equal(2, summary.PacketsSent);
        Assert.Equal(0, summary.PacketsLost);
        Assert.Equal(IPStatus.BadRoute, summary.LastAbnormalStatus);
        Assert.Null(summary.TerminatingIPStatus);
    }
    
    // 13) {Pause,Pause}  
    [Fact]
    public async void SendPingGroupAsync_ReturnsProperPingGroupSummary_OnPausePause() {
        var summary = await PingGroupSummaryStub
            .GeneratePausePause(PingBehaviorOptions, PingThresholdsOptions);
        Assert.Equal(0, summary.AveragePing);
        Assert.Equal(0, summary.MinimumPing);
        Assert.Equal(0, summary.MaximumPing);
        Assert.Equal(0, summary.Jitter);
        Assert.Equal(0, summary.ConsecutiveTimeouts);
        Assert.Equal(PingQualification.ThresholdExceededFlags.NotExceeded, summary.PingQualityFlags);
        Assert.Equal(0, summary.PacketLoss);
        Assert.Equal(2, summary.PacketsSent);
        Assert.Equal(0, summary.PacketsLost);
        Assert.Equal(IPStatus.SourceQuench, summary.LastAbnormalStatus);
        Assert.Null(summary.TerminatingIPStatus);
    }
    
    // 14) {Pause,Halt}     
    [Fact]
    public async void SendPingGroupAsync_ReturnsProperPingGroupSummary_OnPauseHalt() {
        var summary = await PingGroupSummaryStub
            .GeneratePauseHalt(PingBehaviorOptions, PingThresholdsOptions);
        Assert.Equal(0, summary.AveragePing);
        Assert.Equal(0, summary.MinimumPing);
        Assert.Equal(0, summary.MaximumPing);
        Assert.Equal(0, summary.Jitter);
        Assert.Equal(0, summary.ConsecutiveTimeouts);
        Assert.Equal(PingQualification.ThresholdExceededFlags.NotExceeded, summary.PingQualityFlags);
        Assert.Equal(0, summary.PacketLoss);
        Assert.Equal(2, summary.PacketsSent);
        Assert.Equal(0, summary.PacketsLost);
        Assert.Equal(IPStatus.SourceQuench, summary.LastAbnormalStatus);
        Assert.Equal(IPStatus.Unknown, summary.TerminatingIPStatus);
    }
    
    // 15) {Pause,PacketLossCaution}
    [Fact]
    public async void SendPingGroupAsync_ReturnsProperPingGroupSummary_OnPausePacketLossCaution() {
        var summary = await PingGroupSummaryStub
            .GeneratePausePacketLoss(PingBehaviorOptions, PingThresholdsOptions);
        Assert.Equal(0, summary.AveragePing);
        Assert.Equal(0, summary.MinimumPing);
        Assert.Equal(0, summary.MaximumPing);
        Assert.Equal(0, summary.Jitter);
        Assert.Equal(1, summary.ConsecutiveTimeouts);
        Assert.Equal(PingQualification.ThresholdExceededFlags.HighPacketLoss, summary.PingQualityFlags);
        Assert.Equal(50, summary.PacketLoss);
        Assert.Equal(2, summary.PacketsSent);
        Assert.Equal(1, summary.PacketsLost);
        Assert.Equal(IPStatus.TimedOut, summary.LastAbnormalStatus);
        Assert.Null( summary.TerminatingIPStatus);
    }
    
    
    
    
    
}