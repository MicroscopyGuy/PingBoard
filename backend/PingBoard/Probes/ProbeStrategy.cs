namespace PingBoard.Services;

public class ProbeStrategy
{
    private int GroupCount { get; set; }
    private TimeSpan DelayGroup { get; set; }
    private TimeSpan DelayIndividual { get; set; }
    private int FailuresBeforeStop { get; set; }
}