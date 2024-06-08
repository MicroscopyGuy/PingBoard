namespace PingBoard.Tests.PingingTests.PingingTestingUtilities;
using PingBoard.Pinging;


public class PingSchedulerImmediate : IPingScheduler
{
    public async Task DelayPingingAsync()
    {
        await Task.Delay(TimeSpan.FromMilliseconds(0));
    }
    
    public void EndIntervalTracking(){}

    public void StartIntervalTracking(){}
}