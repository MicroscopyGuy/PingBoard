using System.Net.NetworkInformation;
using Microsoft.Extensions.Options;
using PingBoard.Pinging;

namespace PingBoard.Tests;

public class PingQualificationTesting
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
        
        /*
        PingingBehaviorConfig testBehaviorConfig = new PingingBehaviorConfig(){
            TimeoutMs = 3500,
            WaitMs = 1000,
            PayloadStr = "testing: https://github.com/MicroscopyGuy/PingBoard"
        };

        PingOptions options = new PingOptions{
            DontFragment = true,
            Ttl = testBehaviorConfig.Ttl
        };
        */
        IOptions<PingingThresholdsConfig> testThresholdOptions = Options.Create(testThresholdsConfig);
        //IOptions<PingingBehaviorConfig> testBehaviorOptions = Options.Create(testBehaviorConfig);
        PingQualification pingQualifier = new PingQualification(testThresholdOptions);

        PingQualification.ThresholdExceededFlags testFlags = (PingQualification.ThresholdExceededFlags) 0b0010_0000;
        Assert.False(PingQualification.PingQualityWithinThresholds(testFlags));
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
        
        /*
        PingingBehaviorConfig testBehaviorConfig = new PingingBehaviorConfig(){
            TimeoutMs = 3500,
            WaitMs = 1000,
            PayloadStr = "testing: https://github.com/MicroscopyGuy/PingBoard"
        };

        PingOptions options = new PingOptions{
            DontFragment = true,
            Ttl = testBehaviorConfig.Ttl
        };
        */
        IOptions<PingingThresholdsConfig> testThresholdOptions = Options.Create(testThresholdsConfig);
        PingQualification pingQualifier = new PingQualification(testThresholdOptions);

        PingQualification.ThresholdExceededFlags testFlags = (PingQualification.ThresholdExceededFlags) 0b0000_0000;
        Assert.True(PingQualification.PingQualityWithinThresholds(testFlags));
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
        PingQualification pingQualifier = new PingQualification(testThresholdOptions);
        PingQualification.ThresholdExceededFlags correctFlags = (PingQualification.ThresholdExceededFlags) 0b0001_1111;
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
        PingQualification pingQualifier = new PingQualification(testThresholdOptions);
        PingQualification.ThresholdExceededFlags correctFlags = (PingQualification.ThresholdExceededFlags) 0b0000_0000;
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
        PingQualification pingQualifier = new PingQualification(testThresholdOptions);
        PingQualification.ThresholdExceededFlags correctFlags = (PingQualification.ThresholdExceededFlags) 0b0001_0100;
        Assert.Equal(correctFlags, pingQualifier.CalculatePingQualityFlags(mixedQualityResult));
    }

    [Fact]
    public void EmptyQualityDescriptionOnNoFlagsSet(){
        PingQualification.ThresholdExceededFlags emptyFlags = PingQualification.ThresholdExceededFlags.NotExceeded; 
        Assert.Equal("", PingQualification.DescribePingQualityFlags(emptyFlags));
    }

    [Fact]
    public void FullQualityDescriptionOnAllFlagsSet(){
        PingQualification.ThresholdExceededFlags fullFlags = (PingQualification.ThresholdExceededFlags) 0b0001_1111;
        string fullDescription = "HighMinimumPing, HighAveragePing, HighMaximumPing, HighJitter, HighPacketLoss"; 
        Assert.Equal(fullDescription, PingQualification.DescribePingQualityFlags(fullFlags));
    }

    [Fact]
    public void MixedQualityDescriptionOnSomeFlagsSet(){
        PingQualification.ThresholdExceededFlags mixedFlags = (PingQualification.ThresholdExceededFlags) 0b0001_1010;
        string fullDescription = "HighAveragePing, HighJitter, HighPacketLoss"; 
        Assert.Equal(fullDescription, PingQualification.DescribePingQualityFlags(mixedFlags));
    }

    [Fact]
    public void SingleThresholdMentionedInDescriptionOnSingleFlagSet(){
        PingQualification.ThresholdExceededFlags singleFlag = (PingQualification.ThresholdExceededFlags) 0b0000_0001;
        string shortDescription = "HighMinimumPing"; 
        Assert.Equal(shortDescription, PingQualification.DescribePingQualityFlags(singleFlag));
    }

}