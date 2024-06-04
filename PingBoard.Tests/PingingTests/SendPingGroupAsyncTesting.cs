using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace PingBoard.Tests.PingingTests;
using PingBoard.Tests.PingingTests.PingingTestingUtilities;
using PingBoard.Pinging.Configuration;
using PingBoard.Pinging;
using System.Net.NetworkInformation;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Xunit.Sdk;

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
/// |1) {Success,Success}                                                                                              |
/// |2) {Success,Unsuccessful}                                                                                         |
/// |3) {Success,Pause}                                                                                                |
/// |4) {Success,Halt}                                                                                                 |
/// |5) {Success,PacketLossCaution}                                                                                    |
/// |6) {Unsuccessful,Success}                                                                                         | 
/// |7) {Unsuccessful,Unsuccessful}                                                                                    |            |
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

public class SendPingGroupAsyncTesting {
    
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

    [Theory]
    [MemberData(nameof(PingGroupSummaryTestGenerator.GetScenarioFunctions), MemberType = typeof(PingGroupSummaryTestGenerator))]
    public async void TestAllScenarios(string scenarioName){

        // use reflection to translate the scenarioName into the appropriate function, and then invoke it
        PingGroupSummary actual = await PingGroupSummaryStub.RunFunctionByName(
            scenarioName,
            PingBehaviorOptions,
            PingThresholdsOptions
        );
        PingGroupSummary expected = PingGroupSummaryExpectedValues.ExpectedSummaries[scenarioName];
        PingGroupSummaryExpectedValues.AssertExpectedValues(expected, actual);
    }

}