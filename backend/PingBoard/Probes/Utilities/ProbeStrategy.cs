namespace PingBoard.Services;

public class ProbeStrategy
{
    public int GroupCount { get; set; }
    public TimeSpan DelayGroup { get; set; }
    public TimeSpan DelayIndividual { get; set; }
    public int FailuresBeforeStop { get; set; }
}
