namespace PingBoard.Tests.PingingTests.PingingTestingUtilities;
using PingBoard.Pinging;
using System.Text.Json;
public static class PingGroupSummaryExpectedValues
{
    public static Dictionary<string, PingGroupSummary> ExpectedSummaries;

    static PingGroupSummaryExpectedValues()
    {
        string path = Path.Combine(AppContext.BaseDirectory, "PingGroupSummaryExpectedValues.json");
        string jsonText = File.ReadAllText(path);
        ExpectedSummaries = JsonSerializer.Deserialize<Dictionary<string, PingGroupSummary>>(jsonText)!;
    }

    public static void AssertExpectedValues(PingGroupSummary expected, PingGroupSummary actual)
    {
        Assert.Equal(expected.MinimumPing, actual.MinimumPing);
        Assert.Equal(expected.AveragePing, actual.AveragePing);
        Assert.Equal(expected.MaximumPing, actual.MaximumPing);
        Assert.Equal(expected.Jitter, actual.Jitter);
        Assert.Equal(expected.PacketLoss, actual.PacketLoss);
        Assert.Equal(expected.PacketsLost, actual.PacketsLost);
        Assert.Equal(expected.PacketsSent, actual.PacketsSent);
        Assert.Equal(expected.ConsecutiveTimeouts, actual.ConsecutiveTimeouts);
        Assert.Equal(expected.ExcludedPings, actual.ExcludedPings);
        Assert.Equal(expected.LastAbnormalStatus, actual.LastAbnormalStatus);
        Assert.Equal(expected.TerminatingIPStatus, actual.TerminatingIPStatus);
    }
}