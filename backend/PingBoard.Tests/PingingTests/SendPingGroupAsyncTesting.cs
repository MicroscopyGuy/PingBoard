using PingBoard.Database.Models;

namespace PingBoard.Tests.PingingTests;
using PingBoard.Tests.PingingTests.PingingTestingUtilities;

using PingBoard.Pinging;

/// <summary>
/// Ideal for testing proper PingGroupSummary return values for every combination of two states/behaviors.
/// Every "Continue" state most likely is because the ping responded with IPStatus.Success, but there (as of writing this)
/// are other IPStatus codes also mapped to PingingStates.Continue, simply because the status codes are reflective
/// of possibly transient problems.
///
/// In short, the Continue state may not be caused by a successful ping. For this reason there is an additional 
/// term (Unsuccessful) included below in the list of combinations to test.
/// |*************************************************Combinations:*****************************************************|
/// |1) {Success, Success}                                                                                              |
/// |2) {Success, Unsuccessful}                                                                                         |
/// |3) {Success, Pause}                                                                                                |
/// |4) {Success, Halt}                                                                                                 |
/// |5) {Success, PacketLossCaution}                                                                                    |
/// |6) {Unsuccessful, Success}                                                                                         | 
/// |7) {Unsuccessful, Unsuccessful}                                                                                    |
/// |8) {Unsuccessful, Pause}                                                                                           |
/// |9) {Unsuccessful, Halt}                                                                                            |
/// |10) {Unsuccessful, PacketLossCaution}                                                                              |
/// |11) {Pause, Success}                                                                                               |
/// |12) {Pause, Unsuccessful}                                                                                          |
/// |13) {Pause, Pause}                                                                                                 |
/// |14) {Pause, Halt}                                                                                                  |
/// |15) {Pause, PacketLossCaution}                                                                                     |
/// |16) {Halt, Success}                                                                                                |
/// |17) {Halt, Unsuccessful}                                                                                           |
/// |18) {Halt, Pause}                                                                                                  |
/// |19) {Halt, Halt}                                                                                                   |
/// |20) {Halt, PacketLossCaution}                                                                                      |
/// |21) {PacketLossCaution, Success}                                                                                   |
/// |22) {PacketLossCaution, Unsuccessful}                                                                              |
/// |23) {PacketLossCaution, Pause}                                                                                     |
/// |24) {PacketLossCaution, Halt}                                                                                      |
/// |25) {PacketLossCaution, PacketLossCaution}                                                                         |
/// |*******************************************************************************************************************|
/// </summary>
public class SendPingGroupAsyncTesting {
    /// <summary>
    /// Runs the 25 scenario functions, each of which each inject stubbed PingReply objects into SendPingGroupAsync
    /// to generate PingGroupSummaries reflective of the scenario contained in their name. For instance,
    /// GenerateSuccessSuccess.
    /// </summary>
    /// <param name="scenarioName">The name of the scenario being tested</param>
    [Theory]
    [MemberData(nameof(PingGroupSummaryTestGenerator.GetScenarioFunctions), MemberType = typeof(PingGroupSummaryTestGenerator))]
    public async void TestAllScenarios(string scenarioName){

        // use reflection to translate the scenarioName into the appropriate function, and then invoke it
        PingGroupSummary actual = await PingGroupSummaryStub.RunFunctionByName(
            scenarioName,
            SendPingGroupAsyncTestingConfig.PingBehaviorOptions,
            SendPingGroupAsyncTestingConfig.PingThresholdsOptions
        );
        PingGroupSummary expected = PingGroupSummaryExpectedValues.ExpectedSummaries[scenarioName];
        PingGroupSummaryExpectedValues.AssertExpectedValues(expected, actual);
    }

}