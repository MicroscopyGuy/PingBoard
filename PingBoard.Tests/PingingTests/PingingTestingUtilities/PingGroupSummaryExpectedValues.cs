using Microsoft.Extensions.Options;
using PingBoard.Pinging;
using PingBoard.Pinging.Configuration;
using PingBehavior = Microsoft.Extensions.Options.IOptions<PingBoard.Pinging.Configuration.PingingBehaviorConfig>;
using PingThresholds = Microsoft.Extensions.Options.IOptions<PingBoard.Pinging.Configuration.PingingThresholdsConfig>;
public class PingGroupSummaryExpectedValues
{
    public Dictionary<Func<PingBehavior, PingThresholds, PingGroupSummary>, PingGroupSummary> ExpectedSummaries;

     
}