using PingBoard.Database.Models;

namespace PingBoard.Tests.PingingTests.PingingTestingUtilities;
using PingBoard.Pinging;
using PingBoard.Pinging.Configuration;
using Microsoft.Extensions.Options;

public static partial class PingGroupSummaryStub{

    public static IEnumerable<string> GetPingGroupScenarios(){
        foreach (string scenario in PingGroupSummaryExpectedValues.ExpectedSummaries.Keys){
            yield return scenario;
        }
    }

    public static Task<PingGroupSummary> RunFunctionByName(string scenarioFunctionName, 
    IOptions<PingingBehaviorConfig> behavior, IOptions<PingingThresholdsConfig> thresholds){
        var type = typeof(PingGroupSummaryStub);
        var method = type.GetMethod(scenarioFunctionName);

        if (method == null){
            throw new ArgumentOutOfRangeException($"""
                                                     There is no function in PingGroupSummaryStub 
                                                     by the name of {scenarioFunctionName}
                                                   """);
        }

        var parameters = new object[]{ behavior, thresholds};
        return (Task<PingGroupSummary>) method!.Invoke(null, parameters)!;
    }
}