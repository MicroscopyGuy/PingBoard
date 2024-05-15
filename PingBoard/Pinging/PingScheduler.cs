using System.Diagnostics;
using Microsoft.Extensions.Options;
using PingBoard.Pinging;
using PingBoard.Pinging.Configuration;

namespace PingBoard.Pinging{
    /// <summary>
    /// A class that assists GroupPinger by helping it evenly spread out Pings across the interval specified 
    /// in appsettings.json as "WaitTimeMs". The result, under normal networking conditions, should be that
    /// one ping group is sent, received and passed to the calling service in as close to "WaitTimeMs" milliseconds
    /// as possible.
    /// 
    /// It does this by timing each iteration of the loop in GroupPinger that sends an individual ping and using
    /// this time to inform the wait before sending the next ping.
    /// 
    /// An example is below:
    /// If PingsPerCall is set in appsettings.json as 2 and WaitTimeMs is set to 1000ms, then each individual
    /// ping should be received and processed (again, ideally) within 500ms. Since the ping sending and processing
    /// has its own runtime, however, a PingScheduler object tracks this and adjusts the estimated wait time before the next
    /// individual ping is sent.
    /// <~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~1000ms~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~>
    /// <~~~~~~~~~~~~~~~~~~~~~~~500ms~~~~~~~~~~~~~~~~~~~> <~~~~~~~~~~~~~~~~~~~~~500ms~~~~~~~~~~~~~~~~~~~~~~>
    /// ----------------------------------------------------------------------------------------------------
    /// |ping |                                          |ping |                                           |
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
    public class PingScheduler{

        private readonly PingingBehaviorConfig _pingBehaviorConfig;
        private readonly Stopwatch _timer; 
        private readonly TimeSpan _MinimumWaitBeforeNextPingMs = TimeSpan.FromMilliseconds(10);
        private TimeSpan _estimatedWaitTimeInBetweenPingsMs;
        private TimeSpan _waitMinusPingTime; 
    
        public PingScheduler(IOptions<PingingBehaviorConfig> pingBehaviorConfig){
            _pingBehaviorConfig = pingBehaviorConfig.Value;
            _timer = new Stopwatch();
            _estimatedWaitTimeInBetweenPingsMs = TimeSpan.FromMilliseconds(_pingBehaviorConfig.WaitMs/_pingBehaviorConfig.PingsPerCall);
            
        }

        public void StartIntervalTracking(){
            _timer.Restart(); // can be more flexibly invoked in the loop in GroupPinger.cs this way
        }

        public void EndIntervalTracking(){
            _timer.Stop();
            _waitMinusPingTime = _estimatedWaitTimeInBetweenPingsMs - TimeSpan.FromMilliseconds(_timer.Elapsed.TotalMilliseconds);
            //Console.WriteLine($"The waitMinusPingTime is: {_waitMinusPingTime}");
        }

        public TimeSpan CalculateDelayToEvenlySpreadPings(){
            TimeSpan adjustedWaitBeforeNextPing = _waitMinusPingTime > _MinimumWaitBeforeNextPingMs 
                                                  ? _waitMinusPingTime 
                                                  : _MinimumWaitBeforeNextPingMs;
                                                  
            return adjustedWaitBeforeNextPing;
        }

        public async Task DelayPingingAsync(){
            Stopwatch delayTimer = new Stopwatch();
            delayTimer.Start();

            TimeSpan adjustedWaitBeforeNextPingMs = CalculateDelayToEvenlySpreadPings();
            long adjustedWaitMsToLong = (long) adjustedWaitBeforeNextPingMs.TotalMilliseconds;
            long remainingMs, imprecisionBufferRemainingTimeMs = 0;
            
            // waits up to the last 16 milliseconds (since windows has timing accuracy of 15ms)
            // and then eats up the remaining time with the while loop, kind of a reverse 2 phase wait
            int iterations = 0;
            while (delayTimer.ElapsedMilliseconds < adjustedWaitMsToLong){
                iterations++;
                remainingMs = adjustedWaitMsToLong - delayTimer.ElapsedMilliseconds;
                if (remainingMs < 25) continue;
                imprecisionBufferRemainingTimeMs = remainingMs - 16;
                await Task.Delay(TimeSpan.FromMilliseconds(imprecisionBufferRemainingTimeMs));      
            } 
            delayTimer.Stop();
            
        }
    }
}