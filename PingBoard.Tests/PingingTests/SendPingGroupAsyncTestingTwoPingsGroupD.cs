using PingBoard.Tests.PingingTests.PingingTestingUtilities;

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
        
       var summary = await PingGroupSummaryStub
           .GenerateHaltSuccess(PingBehaviorOptions, PingThresholdsOptions);

        Assert.Equal(0, summary.AveragePing);
        Assert.Equal(0, summary.MinimumPing);
        Assert.Equal(0, summary.MaximumPing);
        Assert.Equal(0, summary.Jitter);
        Assert.Equal(0, summary.ConsecutiveTimeouts);
        Assert.Equal(PingQualification.ThresholdExceededFlags.NotExceeded, summary.PingQualityFlags);
        Assert.Equal(0, summary.PacketLoss);
        Assert.Equal(1, summary.PacketsSent);
        Assert.Equal(0, summary.PacketsLost);
        Assert.Null(summary.LastAbnormalStatus);
        Assert.Equal(IPStatus.Unknown, summary.TerminatingIPStatus);
   }
   
   // 17) {Halt,Unsuccessful}
   [Fact]
   public async void SendPingGroupAsync_ReturnsProperPingGroupSummary_OnHaltUnsuccessful() {
       var summary = await PingGroupSummaryStub
           .GenerateHaltUnsuccessful(PingBehaviorOptions, PingThresholdsOptions);
        Assert.Equal(0, summary.AveragePing);
        Assert.Equal(0, summary.MinimumPing);
        Assert.Equal(0, summary.MaximumPing);
        Assert.Equal(0, summary.Jitter);
        Assert.Equal(0, summary.ConsecutiveTimeouts);
        Assert.Equal(PingQualification.ThresholdExceededFlags.NotExceeded, summary.PingQualityFlags);
        Assert.Equal(0, summary.PacketLoss);
        Assert.Equal(1, summary.PacketsSent);
        Assert.Equal(0, summary.PacketsLost);
        Assert.Null(summary.LastAbnormalStatus);
        Assert.Equal(IPStatus.Unknown, summary.TerminatingIPStatus);
   }
   
   // 18) {Halt,Pause}      
   [Fact]
   public async void SendPingGroupAsync_ReturnsProperPingGroupSummary_OnHaltPause() {
       var summary = await PingGroupSummaryStub
            .GenerateHaltPause(PingBehaviorOptions, PingThresholdsOptions);
        Assert.Equal(0, summary.AveragePing);
        Assert.Equal(0, summary.MinimumPing);
        Assert.Equal(0, summary.MaximumPing);
        Assert.Equal(0, summary.Jitter);
        Assert.Equal(0, summary.ConsecutiveTimeouts);
        Assert.Equal(PingQualification.ThresholdExceededFlags.NotExceeded, summary.PingQualityFlags);
        Assert.Equal(0, summary.PacketLoss);
        Assert.Equal(1, summary.PacketsSent);
        Assert.Equal(0, summary.PacketsLost);
        Assert.Null(summary.LastAbnormalStatus);
        Assert.Equal(IPStatus.Unknown, summary.TerminatingIPStatus);
   }
   
   // 19) {Halt,Halt}  
   [Fact]
   public async void SendPingGroupAsync_ReturnsProperPingGroupSummary_OnHaltHalt() {

        var summary = await PingGroupSummaryStub
            .GenerateHaltHalt(PingBehaviorOptions, PingThresholdsOptions);
        Assert.Equal(0, summary.AveragePing);
        Assert.Equal(0, summary.MinimumPing);
        Assert.Equal(0, summary.MaximumPing);
        Assert.Equal(0, summary.Jitter);
        Assert.Equal(0, summary.ConsecutiveTimeouts);
        Assert.Equal(PingQualification.ThresholdExceededFlags.NotExceeded, summary.PingQualityFlags);
        Assert.Equal(0, summary.PacketLoss);
        Assert.Equal(1, summary.PacketsSent);
        Assert.Equal(0, summary.PacketsLost);
        Assert.Null(summary.LastAbnormalStatus);
        Assert.Equal(IPStatus.Unknown, summary.TerminatingIPStatus);
   }
   
   //20) {Halt,PacketLossCaution}
   [Fact]
   public async void SendPingGroupAsync_ReturnsProperPingGroupSummary_OnHaltPacketLossCaution() {
       var summary = await PingGroupSummaryStub
           .GenerateHaltPacketLoss(PingBehaviorOptions, PingThresholdsOptions);
        Assert.Equal(0, summary.AveragePing);
        Assert.Equal(0, summary.MinimumPing);
        Assert.Equal(0, summary.MaximumPing);
        Assert.Equal(0, summary.Jitter);
        Assert.Equal(0, summary.ConsecutiveTimeouts);
        Assert.Equal(PingQualification.ThresholdExceededFlags.NotExceeded, summary.PingQualityFlags);
        Assert.Equal(0, summary.PacketLoss);
        Assert.Equal(1, summary.PacketsSent);
        Assert.Equal(0, summary.PacketsLost);
        Assert.Null(summary.LastAbnormalStatus);
        Assert.Equal(IPStatus.Unknown, summary.TerminatingIPStatus);
   }
}