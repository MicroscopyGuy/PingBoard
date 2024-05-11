using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;
using Pingboard.Pinging;
using PingBoard.Pinging;

///<TODO>
/// Make sure SendPingGroupAsync is designed correctly
/// keep in mind the ability to mock it -- how does that change the design?
/// test the variance method
/// 
/// The exception handling may be long and verbose -- can this be done with another function? perhaps?
/// Put ThresholdExceededFlags in own class, PingQualification. Static class? Dependency on PingThresholdConfig -- how to manage?
///     -does Pinger really need the PingThresholdConfig dependency?
/// Change data types on PingGroupSummary from floats to ints (except for AveragePing)
/// 
/// DONE
/// Make TTL configurable in AppSettings
/// Flags for tripped thresholds
/// Add minimumPingThreshold back in
/// the round trip time is a long, not a float, fix this accordingly
/// 
///</TODO>
public class GroupPinger : IGroupPinger{
    private readonly ILogger<IGroupPinger> _logger;

    private readonly PingingBehaviorConfig _pingBehavior;
    private readonly PingQualification _pingQualifier; 
    private readonly IIndividualPinger _individualPinger;

    public GroupPinger(IIndividualPinger individualPinger, PingQualification pingQualifier,
                       IOptions<PingingBehaviorConfig> pingBehavior, ILogger<IGroupPinger> logger){
        _pingQualifier    = pingQualifier;
        _pingBehavior     = pingBehavior.Value;
        _logger           = logger;
        _individualPinger = individualPinger;

    }

    public async Task<PingGroupSummary> SendPingGroupAsync(IPAddress target, int numberOfPings){
        PingGroupSummary pingGroupInfo = PingGroupSummary.Empty();
        long[] responseTimes = new long[numberOfPings];

        TimeSpan baselineWaitTimeInBetweenPings = TimeSpan.FromMilliseconds(_pingBehavior.WaitMs/numberOfPings);
        TimeSpan MinimumWaitTime                = TimeSpan.FromMilliseconds(10);
        TimeSpan waitMinusPingTime, adjustedWaitBeforeNextPing;
        Stopwatch timer = new Stopwatch();
        pingGroupInfo.Start = DateTime.UtcNow;
        
        int packetsLost = 0;
        int pingCounter = 0;
        while(pingCounter < numberOfPings){
            timer.Restart();
            if (pingGroupInfo.Start == DateTime.MinValue){ pingGroupInfo.Start = DateTime.Now; }
  
            PingReply response = await _individualPinger.SendPingIndividualAsync(target);
            pingGroupInfo.End = DateTime.UtcNow;  // set time received, since may terminate prematurely keep this up to date
            packetsLost += (response.Status == IPStatus.TimedOut) ? 1 : 0;
            PingingStates.PingState currentPingState = IcmpStatusCodeLookup.StatusCodes[response.Status].State;

            if (currentPingState == PingingStates.PingState.Continue){
                pingGroupInfo.AveragePing += response.RoundtripTime;
                if (response.RoundtripTime < pingGroupInfo.MinimumPing){ pingGroupInfo.MinimumPing = (short) response.RoundtripTime; }
                if (response.RoundtripTime > pingGroupInfo.MaximumPing){ pingGroupInfo.MaximumPing = (short) response.RoundtripTime; }
                responseTimes[pingCounter] = response.RoundtripTime;
            }

            else if (currentPingState == PingingStates.PingState.Pause || currentPingState == PingingStates.PingState.Halt){
                pingGroupInfo.TerminatingIPStatus = response.Status; 
                break;
            }

            pingCounter++;
            timer.Stop();
            waitMinusPingTime = baselineWaitTimeInBetweenPings-TimeSpan.FromMilliseconds(timer.Elapsed.TotalMilliseconds);
            adjustedWaitBeforeNextPing = waitMinusPingTime > MinimumWaitTime ? waitMinusPingTime : MinimumWaitTime;
            await Task.Delay(adjustedWaitBeforeNextPing);
        }
        
        pingGroupInfo.AveragePing = (float) Math.Round(pingCounter > 0 ? pingGroupInfo.AveragePing!.Value/pingCounter : 0, 3);
        pingGroupInfo.Jitter = PingGroupSummary.CalculatePingJitter(responseTimes);
        pingGroupInfo.PacketLoss = pingCounter > 0 ? packetsLost/pingCounter : 0;
        pingGroupInfo.PingQualityFlags = _pingQualifier.CalculatePingQualityFlags(pingGroupInfo);
        return pingGroupInfo;
    }

    

}