namespace PingBoard.TestUtilities.PingingTestingUtilities;
using Microsoft.Extensions.Options;
using PingBoard.Pinging.Configuration;

public static class SendPingGroupAsyncTestingConfig
{
    public static readonly PingingThresholdsConfig PingThresholds = new PingingThresholdsConfig() {
        MinimumPingMs = 5, 
        AveragePingMs = 10, 
        MaximumPingMs = 25,
        JitterMs = 5,
        PacketLossPercentage = 0
    };

    public static readonly IOptions<PingingThresholdsConfig> PingThresholdsOptions = Options.Create(PingThresholds);
    
    public static readonly PingingBehaviorConfig PingBehavior = new PingingBehaviorConfig(){
        PayloadStr = "this is a payload string", // not crucial here
        PingsPerCall = 2, // crucial for this test
        ReportBackAfterConsecutiveTimeouts = 2, // not crucial here
        TimeoutMs = 1500, // not crucial for this test
        Ttl = 64,
        WaitMs = 20 // technically violates the WaitMs limits, but the wait time isn't relevant here
    };

    public static readonly IOptions<PingingBehaviorConfig> PingBehaviorOptions = Options.Create(PingBehavior);
}