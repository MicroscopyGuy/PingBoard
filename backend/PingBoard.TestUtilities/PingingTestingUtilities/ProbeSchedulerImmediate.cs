namespace PingBoard.Tests.PingingTests.PingingTestingUtilities;
using PingBoard.Pinging;


public class ProbeSchedulerImmediate : IProbeScheduler
{
    public async Task DelayProbingAsync()
    {
        await Task.Delay(TimeSpan.FromMilliseconds(0));
    }
    
    public void EndIntervalTracking(){}

    public void StartIntervalTracking(){}
}