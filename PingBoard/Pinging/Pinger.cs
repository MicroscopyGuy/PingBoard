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
public class GroupPinger{
    private readonly PingingThresholdsConfig _pingThresholds;
    private readonly ILogger<GroupPinger> _logger;
    private readonly PingQualification _pingQualifier; 

    private readonly IndividualPinger _individualPinger;

    public GroupPinger(IndividualPinger individualPinger, PingQualification pingQualifier, 
                       IOptions<PingingThresholdsConfig> thresholdsConfig, ILogger<GroupPinger> logger){
        _pingQualifier = pingQualifier;
        _pingThresholds = thresholdsConfig.Value;
        _logger = logger;
        _individualPinger = individualPinger;

    }

    public async Task<PingGroupSummary> SendPingGroupAsync(IPAddress target, int numberOfPings){
        PingGroupSummary pingGroupInfo = PingGroupSummary.Empty();
        long[] responseTimes = new long[numberOfPings];
        int packetsLost = 0;

        int pingCounter = 0;
        while(pingCounter < numberOfPings){

            if (pingGroupInfo.Start == DateTime.MinValue){ pingGroupInfo.Start = DateTime.Now; }

            //PingReply response = await pingSender.SendPingAsync(target, _pingBehavior.WaitMs, buffer, options);
            PingReply response = await _individualPinger.SendPingIndividualAsync(target);
            packetsLost += (response.Status == IPStatus.TimedOut) ? 1 : 0; 
            PingingStates.PingState currentPingState = IcmpStatusCodeLookup.StatusCodes[response.Status].State;

            if (currentPingState == PingingStates.PingState.Continue){
                if (pingCounter == numberOfPings){ pingGroupInfo.End = DateTime.Now; } // set time received

                pingGroupInfo.AveragePing += response.RoundtripTime;
                if (response.RoundtripTime < pingGroupInfo.MinimumPing){ pingGroupInfo.MinimumPing = (short) response.RoundtripTime; }
                if (response.RoundtripTime > pingGroupInfo.MaximumPing){ pingGroupInfo.MaximumPing = (short) response.RoundtripTime; }

                responseTimes[pingCounter] = response.RoundtripTime;
            }

            else if (currentPingState == PingingStates.PingState.Pause || currentPingState == PingingStates.PingState.Halt){

                // probably should pause, this way each pause results in a wait of 1 x WaitMs, 
                // as opposed to NumberOfPingsToSend x WaitMs (32 x 3 sec = 96 sec, or  > 1.5 minutes!)
                pingGroupInfo.End = DateTime.Now;
                pingGroupInfo.TerminatingIPStatus = response.Status;
                 
                // 
                break;
            }
            pingCounter++; 

        }
        pingGroupInfo.Jitter = CalculatePingVariance(responseTimes, pingGroupInfo.AveragePing!.Value);
        pingGroupInfo.AveragePing /= pingCounter;
        pingGroupInfo.PacketLoss = packetsLost/pingCounter;
        pingGroupInfo.PingQualityFlags = _pingQualifier.CalculatePingQualityFlags(pingGroupInfo);
        return pingGroupInfo;
    }

    public static float CalculatePingVariance(long[] responseTimes, float mean){
        if (responseTimes.Length <= 1){ 
            return 0;
        }

        float sumSquaredMeanDiff = 0;
        foreach (long rtt in responseTimes){
            sumSquaredMeanDiff += (float) Math.Pow(rtt-mean, 2);
        }

        
        // variance
        return (float) sumSquaredMeanDiff / responseTimes.Length;
    }

}