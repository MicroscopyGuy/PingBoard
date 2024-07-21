namespace PingBoard.Tests.PingingTests;
using PingBoard.Pinging;
//namespace PingBoard.Tests;


public class PingGroupSummaryTesting{

    /*************************************************************Jitter Testing*****************************************/
    #region "JitterTesting"
    [Fact]
    public void ProperPingJitterOnOnePing(){
        List<long> rtts = [7];
        Assert.Equal(0, PingGroupSummary.CalculatePingJitter(rtts));
    }

    [Fact]
    public void ProperPingJitterOnTwoPings(){
        List<long> rtts = [7, 23];
        Assert.Equal(16, PingGroupSummary.CalculatePingJitter(rtts));
    }

    [Fact]
    public void ProperPingJitterOnSeveralPings(){
        List<long> rtts = [7, 23, 48, 57, 6, 3, 9, 123, 115];
        Assert.Equal(29, PingGroupSummary.CalculatePingJitter(rtts));
    }
    #endregion
    /************************************************************End Jitter Testing*****************************************/
    
    /************************************************************Average Ping Testing***************************************/
    /// <seealso>
    /// GroupPinger assumes a certain calculation method for average ping. The AveragePing property on a PingGroupSummary
    /// object accumulates (sums) the values of all non-TimedOut pings, and only at the end divides by the
    /// number of pings that were sent and NOT lost, ie, that were actually received.
    /// 
    /// For this reason these tests require setting up a PingGroupSummary object with the AveragePing already summed.
    /// </seealso>
    #region "AveragePingTesting"
    [Fact]
    public void ProperAveragePing_OnOnePing_NoPacketLoss(){
        PingGroupSummary summary = new PingGroupSummary{
            AveragePing = 11, // say only one ping so
            PacketsLost = 0,
            PacketsSent = 1,
            ExcludedPings = 0
        };
        summary.AveragePing = PingGroupSummary.CalculateAveragePing(summary);
        Assert.Equal(11, summary.AveragePing);
    }

    [Fact]
    public void ProperAveragePing_OnManyPings_NoPacketLoss(){
        PingGroupSummary summary = new PingGroupSummary{
            AveragePing = 187, 
            PacketsLost = 0,
            PacketsSent = 34,
            ExcludedPings = 0
        };
        summary.AveragePing = PingGroupSummary.CalculateAveragePing(summary);
        Assert.Equal(5.5, summary.AveragePing);
    }

    [Fact]
    public void ProperAveragePing_OnOnePing_WhenPreviousPacketLoss(){
        PingGroupSummary summary = new PingGroupSummary{
            AveragePing = 5,
            PacketsLost = 1,
            PacketsSent = 2,
            ExcludedPings = 1
        };
        summary.AveragePing = PingGroupSummary.CalculateAveragePing(summary);
        Assert.Equal(5, summary.AveragePing);
    }
    
    [Fact]
    public void ProperAveragePing_OnManyPings_WhenPreviousPacketLoss(){
        PingGroupSummary summary = new PingGroupSummary{
            AveragePing = 261.25F,
            PacketsLost = 13,
            PacketsSent = 32,
            ExcludedPings = 13
        };
        float avg = PingGroupSummary.CalculateAveragePing(summary);
        Assert.Equal(13.75, avg);
    }
    
    [Fact]
    public void ProperAveragePing_OnManyPings_WhenPacketLossAndExcluded(){
        PingGroupSummary summary = new PingGroupSummary{
            AveragePing = 50,
            PacketsLost = 3,
            PacketsSent = 10,
            ExcludedPings = 5
        };
        float avg = PingGroupSummary.CalculateAveragePing(summary);
        Assert.Equal(10, avg);
    }
    
    [Fact]
    public void ProperAveragePing_OnManyPings_ExcludedAndNoPacketLoss(){
        PingGroupSummary summary = new PingGroupSummary{
            AveragePing = 50,
            PacketsLost = 0,
            PacketsSent = 10,
            ExcludedPings = 5
        };
        float avg = PingGroupSummary.CalculateAveragePing(summary);
        Assert.Equal(10, avg);
    }
    #endregion
    /********************************************************End Average Ping Testing****************************************/

    /************************************************************PacketLoss Testing******************************************/
    #region "PacketLossTesting"

    [Fact]
    public void ProperPacketLoss_OnZeroLostPackets(){
        PingGroupSummary summary = new PingGroupSummary{
            PacketsLost = 0,
            PacketsSent = 2
        };
        float packetLoss = PingGroupSummary.CalculatePacketLoss(
            summary.PacketsSent, summary.PacketsLost);
        Assert.Equal(0, packetLoss);
    }

    [Fact]
    public void ProperPacketLoss_OnOneLostPacket(){
        PingGroupSummary summary = new PingGroupSummary{
            PacketsLost = 1,
            PacketsSent = 2
        };
        float packetLoss = PingGroupSummary.CalculatePacketLoss(
            summary.PacketsSent, summary.PacketsLost);
        Assert.Equal(50, packetLoss);
    }

    [Fact]
    public void ProperPacketLoss_OnManyLostPackets(){
        PingGroupSummary summary = new PingGroupSummary{
            PacketsLost = 4,
            PacketsSent = 16
        };
        float packetLoss = PingGroupSummary.CalculatePacketLoss(
            summary.PacketsSent, summary.PacketsLost);
        Assert.Equal(25, packetLoss);
    }

    #endregion
    /******************************************************** End PacketLoss Testing*****************************************/ 


}
