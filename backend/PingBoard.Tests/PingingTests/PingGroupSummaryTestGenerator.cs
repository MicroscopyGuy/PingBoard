using PingBoard.Pinging;
using PingBoard.Tests.PingingTests.PingingTestingUtilities;


public class PingGroupSummaryTestGenerator{
    public static IEnumerable<object[]> GetScenarioFunctions(){
        foreach (string key in PingGroupSummaryExpectedValues.ExpectedSummaries.Keys){
            yield return new object[] { key }; 
        }
    }
} 