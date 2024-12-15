namespace PingBoard.Pinging;

public interface IProbeScheduler
{
    public Task DelayProbingAsync();
    public void StartIntervalTracking();
    public void EndIntervalTracking();
}