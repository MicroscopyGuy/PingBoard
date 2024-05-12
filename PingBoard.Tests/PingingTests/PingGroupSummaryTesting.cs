using PingBoard.Pinging;
//namespace PingBoard.Tests;

namespace PingBoard.Tests.PingingTests{
    public class PingGroupSummaryTesting{
        [Fact]
        public void ProperPingJitterOnOnePing(){
            long[] rtts = { 7 };
            Assert.Equal(0, PingGroupSummary.CalculatePingJitter(rtts));
        }

        [Fact]
        public void ProperPingJitterOnTwoPings(){
            long[] rtts = { 7, 23 };
            Assert.Equal(16, PingGroupSummary.CalculatePingJitter(rtts));
        }

        [Fact]
        public void ProperPingJitterOnSeveralPings(){
            long[] rtts = { 7, 23, 48, 57, 6, 3, 9, 123, 115 };
            Assert.Equal(29, PingGroupSummary.CalculatePingJitter(rtts));
        }
    }
}