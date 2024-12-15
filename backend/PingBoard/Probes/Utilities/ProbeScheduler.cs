using PingBoard.Probes.NetworkProbes;
using PingBoard.Services;

namespace PingBoard.Probes;
using System.Diagnostics;
using Microsoft.Extensions.Options;
using PingBoard.Pinging;
using PingBoard.Pinging.Configuration;

/// <summary>
/// A class that assists (<see cref="NetworkProbeLiason"/> class) by helping it evenly spread out probes
/// across the interval specified in appsettings.json as "WaitTimeMs". The result, under normal networking conditions,
/// should be that one probe is sent, received and processed in as close to "WaitTimeMs" milliseconds as possible.
/// 
/// It does this by timing each iteration of the loop in NetworkProbeLiason and using this time to inform the wait
/// before sending the next ping.
/// 
/// An example is below:
/// If PingsPerCall is set in appsettings.json as 2 and WaitTimeMs is set to 1000ms, then each individual
/// ping should be received and processed (again, ideally) within 500ms. Since the ping sending and processing
/// has its own runtime, however, a PingScheduler object tracks this and adjusts the estimated wait time before the next
/// individual ping is sent.
/// |~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~1000ms~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~|
/// |~~~~~~~~~~~~~~~~~~~~~~~500ms~~~~~~~~~~~~~~~~~~~| |~~~~~~~~~~~~~~~~~~~~~500ms~~~~~~~~~~~~~~~~~~~~~~|
/// ----------------------------------------------------------------------------------------------------
/// |probe|                                          |probe|                                           |
/// |sent |                                          |sent |                                           |
/// |and  |                                          |and  |                                           |
/// |proc |                                          |proc |                                           |
/// |essed|                                          |essed|                                           |
/// |in   |                                          |in   |                                           |
/// |70ms |                                          |50ms |                                           |
/// ----------------------------------------------------------------------------------------------------
/// |      PingScheduler calculates (500-70)ms wait  | PingScheduler calculates (500-50)ms wait        |
/// |      GroupPinger now waits 430ms before        | GroupPinger now waits 450ms before finishing    |
/// |      sending the next ping in the group        | SendPingGroupAsync function call                |
/// ----------------------------------------------------------------------------------------------------
/// 
/// If the Ping send/processing time exceeds that of the estimated wait (500ms in the diagram above), which
/// could happen from a slow computer, as well as from long or timed out pings, then the PingScheduler
/// will wait the established minimum wait time, 10ms.
/// 
/// Evenly spread out pings do a few things:
///     1) The PingGroupSummary object contains a more accurate sample of the interval over which it was gathered
///     2) Per the Microsoft Ping documentation, pings cannot be sent very frequently. And from my own testing, can result
///        in erroneous 0ms ping times if sent too often.
/// </summary>
public class ProbeScheduler : IProbeScheduler{

    private readonly PingingBehaviorConfig _pingBehaviorConfig;
    private readonly Stopwatch _timer; 
    private readonly TimeSpan _minimumWaitBeforeNextProbeMs = TimeSpan.FromMilliseconds(10);
    private readonly TimeSpan _estimatedWaitTimeInBetweenProbesMs;
    private TimeSpan _waitMinusProbeTime; 

    public ProbeScheduler(IOptions<PingingBehaviorConfig> pingBehaviorConfig){
        _pingBehaviorConfig = pingBehaviorConfig.Value;
        _timer = new Stopwatch();
        //_estimatedWaitTimeInBetweenPingsMs = TimeSpan.FromMilliseconds(_pingBehaviorConfig.WaitMs/_pingBehaviorConfig.PingsPerCall);
        _estimatedWaitTimeInBetweenProbesMs = TimeSpan.FromMilliseconds(1000);
    }
    
    public void StartIntervalTracking(){
        _timer.Restart(); // can be more flexibly invoked in the loop in NetworkProbeLiason this way
    }

    public void EndIntervalTracking(){
        _timer.Stop();
        _waitMinusProbeTime = _estimatedWaitTimeInBetweenProbesMs - TimeSpan.FromMilliseconds(_timer.Elapsed.TotalMilliseconds);
    }

    /// <summary>
    /// Calculates 
    /// </summary>
    /// <returns>the amount of time the NetworkProbeLiason should wait before sending another probe</returns>
    private TimeSpan CalculateProbingDelay(){
        TimeSpan adjustedWaitBeforeNextProbe = _waitMinusProbeTime > _minimumWaitBeforeNextProbeMs 
                                              ? _waitMinusProbeTime 
                                              : _minimumWaitBeforeNextProbeMs;
                                              
        return adjustedWaitBeforeNextProbe;
    }

    /// <summary>
    /// Waits the appropriate time to maintain roughly even timing between probes
    /// </summary>
    /// <returns></returns>
    public async Task DelayProbingAsync(){
        Stopwatch delayTimer = new Stopwatch();
        delayTimer.Start();

        TimeSpan adjustedWaitBeforeNextProbeMs = CalculateProbingDelay();
        long adjustedWaitMsToLong = (long) adjustedWaitBeforeNextProbeMs.TotalMilliseconds;
        long remainingMs, imprecisionBufferRemainingTimeMs = 0;
        
        // waits up to the last 16 milliseconds (since windows has timing accuracy of 15ms)
        // and then eats up the remaining time with the while loop, kind of a reverse 2 phase wait
        while (delayTimer.ElapsedMilliseconds < adjustedWaitMsToLong){
            remainingMs = adjustedWaitMsToLong - delayTimer.ElapsedMilliseconds;
            if (remainingMs < 25) continue;
            imprecisionBufferRemainingTimeMs = remainingMs - 16;
            await Task.Delay(TimeSpan.FromMilliseconds(imprecisionBufferRemainingTimeMs));      
        } 
        delayTimer.Stop();
        
    }
}
