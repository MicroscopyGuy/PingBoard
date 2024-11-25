using System.Net.NetworkInformation;
using Microsoft.Extensions.Options;
using PingBoard.Database.Models;
using PingBoard.Pinging;
using PingBoard.Pinging.Configuration;


namespace PingBoard.Tests.PingingTests{
    public class PingGroupQualifierTesting
    {
        [Fact]
        public void QualityDeterminedOutsideThresholds_OnPingQualityOutsideThresholds()
        { 
            PingingThresholdsConfig testThresholdsConfig = new PingingThresholdsConfig(){
                MinimumPingMs = 50,
                AveragePingMs = 60,
                MaximumPingMs = 100,
                JitterMs      = 15,
                PacketLossPercentage = 0
            };
            
            IOptions<PingingThresholdsConfig> testThresholdOptions = Options.Create(testThresholdsConfig);
            PingGroupQualifier pingQualifier = new PingGroupQualifier(testThresholdOptions);

            PingGroupQualifier.ThresholdExceededFlags testFlags = (PingGroupQualifier.ThresholdExceededFlags) 0b0010_0000;
            Assert.False(PingGroupQualifier.PingQualityWithinThresholds(testFlags));
        }

        [Fact]
        public void QualityDeterminedWithinThresholds_OnPingQualityInsideThresholds()
        { 
            PingingThresholdsConfig testThresholdsConfig = new PingingThresholdsConfig(){
                MinimumPingMs = 35,
                AveragePingMs = 49,
                MaximumPingMs = 78,
                JitterMs      = 13,
                PacketLossPercentage = 0
            };
            
            IOptions<PingingThresholdsConfig> testThresholdOptions = Options.Create(testThresholdsConfig);
            PingGroupQualifier pingQualifier = new PingGroupQualifier(testThresholdOptions);

            PingGroupQualifier.ThresholdExceededFlags testFlags = (PingGroupQualifier.ThresholdExceededFlags) 0b0000_0000;
            Assert.True(PingGroupQualifier.PingQualityWithinThresholds(testFlags));
        }

        [Fact]
        public void AllFlagsSetWhenAllThresholdsExceeded()
        { 
            PingingThresholdsConfig testThresholdsConfig = new PingingThresholdsConfig(){
                MinimumPingMs = 1,
                AveragePingMs = 2,
                MaximumPingMs = 3,
                JitterMs      = 4,
                PacketLossPercentage = 0
            };

            PingGroupSummary poorQualityResult = PingGroupSummary.Empty();
            poorQualityResult.MinimumPing = 102;
            poorQualityResult.AveragePing = 182;
            poorQualityResult.MaximumPing = 2987;
            poorQualityResult.Jitter = 678;
            poorQualityResult.PacketLoss = 0.5f;

            IOptions<PingingThresholdsConfig> testThresholdOptions = Options.Create(testThresholdsConfig);
            PingGroupQualifier pingQualifier = new PingGroupQualifier(testThresholdOptions);
            PingGroupQualifier.ThresholdExceededFlags correctFlags = (PingGroupQualifier.ThresholdExceededFlags) 0b0001_1111;
            Assert.Equal(correctFlags, pingQualifier.CalculatePingQualityFlags(poorQualityResult));
        }

        [Fact]
        public void NoFlagsSetWhenNoThresholdsExceeded()
        { 
            PingingThresholdsConfig testThresholdsConfig = new PingingThresholdsConfig(){
                MinimumPingMs = 50,
                AveragePingMs = 60,
                MaximumPingMs = 70,
                JitterMs      = 20,
                PacketLossPercentage = 0
            };

            PingGroupSummary goodQualityResult = new PingGroupSummary();
            goodQualityResult.MinimumPing = 3;
            goodQualityResult.AveragePing = 6;
            goodQualityResult.MaximumPing = 21;
            goodQualityResult.Jitter = 12;
            goodQualityResult.PacketLoss = 0.0f;

            IOptions<PingingThresholdsConfig> testThresholdOptions = Options.Create(testThresholdsConfig);
            PingGroupQualifier pingQualifier = new PingGroupQualifier(testThresholdOptions);
            PingGroupQualifier.ThresholdExceededFlags correctFlags = (PingGroupQualifier.ThresholdExceededFlags) 0b0000_0000;
            Assert.Equal(correctFlags, pingQualifier.CalculatePingQualityFlags(goodQualityResult));
        }

        [Fact]
        public void ProperFlagsSetWhenTheirThresholdsExceeded(){
            PingingThresholdsConfig testThresholdsConfig = new PingingThresholdsConfig(){
                MinimumPingMs = 40,
                AveragePingMs = 50,
                MaximumPingMs = 100,
                JitterMs      = 25,
                PacketLossPercentage = 0
            };

            PingGroupSummary mixedQualityResult = new PingGroupSummary();
            mixedQualityResult.MinimumPing = 5;
            mixedQualityResult.AveragePing = 21;
            mixedQualityResult.MaximumPing = 127;
            mixedQualityResult.Jitter = 21;
            mixedQualityResult.PacketLoss = 0.1f;

            IOptions<PingingThresholdsConfig> testThresholdOptions = Options.Create(testThresholdsConfig);
            PingGroupQualifier pingQualifier = new PingGroupQualifier(testThresholdOptions);
            PingGroupQualifier.ThresholdExceededFlags correctFlags = (PingGroupQualifier.ThresholdExceededFlags) 0b0001_0100;
            Assert.Equal(correctFlags, pingQualifier.CalculatePingQualityFlags(mixedQualityResult));
        }

        [Fact]
        public void EmptyQualityDescriptionOnNoFlagsSet(){
            PingGroupQualifier.ThresholdExceededFlags emptyFlags = PingGroupQualifier.ThresholdExceededFlags.NotExceeded; 
            Assert.Equal("", PingGroupQualifier.DescribePingQualityFlags(emptyFlags));
        }

        [Fact]
        public void FullQualityDescriptionOnAllFlagsSet(){
            PingGroupQualifier.ThresholdExceededFlags fullFlags = (PingGroupQualifier.ThresholdExceededFlags) 0b0001_1111;
            string fullDescription = "HighMinimumPing, HighAveragePing, HighMaximumPing, HighJitter, HighPacketLoss"; 
            Assert.Equal(fullDescription, PingGroupQualifier.DescribePingQualityFlags(fullFlags));
        }

        [Fact]
        public void MixedQualityDescriptionOnSomeFlagsSet(){
            PingGroupQualifier.ThresholdExceededFlags mixedFlags = (PingGroupQualifier.ThresholdExceededFlags) 0b0001_1010;
            string fullDescription = "HighAveragePing, HighJitter, HighPacketLoss"; 
            Assert.Equal(fullDescription, PingGroupQualifier.DescribePingQualityFlags(mixedFlags));
        }

        [Fact]
        public void SingleThresholdMentionedInDescriptionOnSingleFlagSet(){
            PingGroupQualifier.ThresholdExceededFlags singleFlag = (PingGroupQualifier.ThresholdExceededFlags) 0b0000_0001;
            string shortDescription = "HighMinimumPing"; 
            Assert.Equal(shortDescription, PingGroupQualifier.DescribePingQualityFlags(singleFlag));
        }
    }
}