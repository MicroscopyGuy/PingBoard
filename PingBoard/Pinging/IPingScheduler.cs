namespace PingBoard.Pinging;

public interface IPingScheduler
{
    public Task DelayPingingAsync();
    public void StartIntervalTracking();
    public void EndIntervalTracking();
}