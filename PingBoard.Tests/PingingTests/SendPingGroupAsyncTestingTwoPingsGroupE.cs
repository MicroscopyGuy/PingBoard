namespace PingBoard.Tests.PingingTests;
using PingBoard.Tests.PingingTests.PingingTestingUtilities;
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
   public async void SendPingGroupAsync_ReturnsProperPingGroupSummary_PacketLossCautionSuccess()
   {
        var summary = await PingGroupSummaryStub
             .GeneratePacketLossSuccess(PingBehaviorOptions, PingThresholdsOptions);
        Assert.Equal(6, summary.AveragePing);
        Assert.Equal(6, summary.MinimumPing);
        Assert.Equal(6, summary.MaximumPing);
        Assert.Equal(0, summary.Jitter);
        Assert.Equal(0, summary.ConsecutiveTimeouts);
        Assert.Equal(
            PingQualification.ThresholdExceededFlags.HighMinimumPing
                  | PingQualification.ThresholdExceededFlags.HighPacketLoss, summary.PingQualityFlags);
        Assert.Equal(50, summary.PacketLoss);
        Assert.Equal(2, summary.PacketsSent);
        Assert.Equal(1, summary.PacketsLost);
        Assert.Equal(1, summary.ExcludedPings);
        Assert.Equal(IPStatus.TimedOut, summary.LastAbnormalStatus);
        Assert.Null(summary.TerminatingIPStatus);
   }
   
   // 22) {PacketLossCaution,Unsuccessful} 
   [Fact]
   public async void SendPingGroupAsync_ReturnsProperPingGroupSummary_OnPacketLossCautionUnsuccessful() {
        var summary = await PingGroupSummaryStub
             .GeneratePacketLossUnsuccessful(PingBehaviorOptions, PingThresholdsOptions);
        Assert.Equal(0, summary.AveragePing);
        Assert.Equal(0, summary.MinimumPing);
        Assert.Equal(0, summary.MaximumPing);
        Assert.Equal(0, summary.Jitter);
        Assert.Equal(0, summary.ConsecutiveTimeouts);
        Assert.Equal(PingQualification.ThresholdExceededFlags.HighPacketLoss, summary.PingQualityFlags);
        Assert.Equal(50, summary.PacketLoss);
        Assert.Equal(2, summary.PacketsSent);
        Assert.Equal(1, summary.PacketsLost);
        Assert.Equal(2, summary.ExcludedPings);
        Assert.Equal(IPStatus.BadRoute, summary.LastAbnormalStatus);
        Assert.Null(summary.TerminatingIPStatus);
   }
   
   // 23) {PacketLossCaution,Pause}
   [Fact]
   public async void SendPingGroupAsync_ReturnsProperPingGroupSummary_OnPacketLossCautionPause() {
        var summary = await PingGroupSummaryStub
             .GeneratePacketLossPause(PingBehaviorOptions, PingThresholdsOptions);
        Assert.Equal(0, summary.AveragePing);
        Assert.Equal(0, summary.MinimumPing);
        Assert.Equal(0, summary.MaximumPing);
        Assert.Equal(0, summary.Jitter);
        Assert.Equal(0, summary.ConsecutiveTimeouts);
        Assert.Equal(PingQualification.ThresholdExceededFlags.HighPacketLoss, summary.PingQualityFlags);
        Assert.Equal(50, summary.PacketLoss);
        Assert.Equal(2, summary.PacketsSent);
        Assert.Equal(1, summary.PacketsLost);
        Assert.Equal(2, summary.ExcludedPings);
        Assert.Equal(IPStatus.SourceQuench, summary.LastAbnormalStatus);
        Assert.Null(summary.TerminatingIPStatus);
   }
   // 24) {PacketLossCaution,Halt}
   [Fact]
   public async void SendPingGroupAsync_ReturnsProperPingGroupSummary_OnPacketLossCautionHalt() {
        var summary = await PingGroupSummaryStub
             .GeneratePacketLossHalt(PingBehaviorOptions, PingThresholdsOptions);
        Assert.Equal(0, summary.AveragePing);
        Assert.Equal(0, summary.MinimumPing);
        Assert.Equal(0, summary.MaximumPing);
        Assert.Equal(0, summary.Jitter);
        Assert.Equal(0, summary.ConsecutiveTimeouts);
        Assert.Equal(PingQualification.ThresholdExceededFlags.HighPacketLoss, summary.PingQualityFlags);
        Assert.Equal(50, summary.PacketLoss);
        Assert.Equal(2, summary.PacketsSent);
        Assert.Equal(1, summary.PacketsLost);
        Assert.Equal(2, summary.ExcludedPings);
        Assert.Equal(IPStatus.TimedOut, summary.LastAbnormalStatus);
        Assert.Equal(IPStatus.Unknown, summary.TerminatingIPStatus);
   }
   
   // 25) {PacketLossCaution,PacketLossCaution}
   [Fact]
   public async void SendPingGroupAsync_ReturnsProperPingGroupSummary_OnPacketLossCautionPacketLossCaution() {
        var summary = await PingGroupSummaryStub
             .GeneratePacketLossPacketLoss(PingBehaviorOptions, PingThresholdsOptions);
        Assert.Equal(0, summary.AveragePing);
        Assert.Equal(0, summary.MinimumPing);
        Assert.Equal(0, summary.MaximumPing);
        Assert.Equal(0, summary.Jitter);
        Assert.Equal(2, summary.ConsecutiveTimeouts);
        Assert.Equal(PingQualification.ThresholdExceededFlags.HighPacketLoss, summary.PingQualityFlags);
        Assert.Equal(100, summary.PacketLoss);
        Assert.Equal(2, summary.PacketsSent);
        Assert.Equal(2, summary.PacketsLost);
        Assert.Equal(2, summary.ExcludedPings);
        Assert.Equal(IPStatus.TimedOut, summary.LastAbnormalStatus);
        Assert.Null(summary.TerminatingIPStatus);
   }
   
}