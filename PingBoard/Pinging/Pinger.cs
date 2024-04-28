using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;
using PingBoard.Pinging;


public class Pinger{
    private readonly PingingBehaviorConfig _pingBehavior;
    private readonly PingingThresholdsConfig _pingThresholds;

    private readonly ILogger<Pinger> _logger;

    public Pinger(IOptions<PingingBehaviorConfig> behaviorConfig, IOptions<PingingThresholdsConfig> thresholdsConfig, ILogger<Pinger> logger){
        _pingBehavior = behaviorConfig.Value;
        _pingThresholds = thresholdsConfig.Value;
        _logger = logger;
    }

    public async Task<PingGroupSummary> SendPingGroupAsync(IPAddress target, int numberOfPings){
        PingGroupSummary pingGroupInfo = PingGroupSummary.Empty();
        float[] responseTimes = new float[numberOfPings];
        int packetsLost = 0;
 
        using Ping pingSender = new Ping();
        int pingCounter = 0;
        while(pingCounter < numberOfPings){
            PingOptions options = new PingOptions();
            options.DontFragment = true; // prevents data from being split into > 1 packet, crucial
            //string dataToSend = "@MIT_License@ https://github.com/MicroscopyGuy/PingBoard"; // make this configurable
            byte[] buffer = Encoding.ASCII.GetBytes(_pingBehavior.PayloadStr!);
            
            if (pingGroupInfo.Start == DateTime.MinValue){
                pingGroupInfo.Start = DateTime.Now;  // set time sent (attempted)
            }

            PingReply response = await pingSender.SendPingAsync(target, _pingBehavior.WaitMs, buffer, options);
            packetsLost += (response.Status == IPStatus.TimedOut) ? 1 : 0; // is this proper?
            PingingStates.PingState currentPingState = IcmpStatusCodeLookup.StatusCodes[response.Status].State;
            

            if (currentPingState == PingingStates.PingState.Continue){
                if (pingCounter == numberOfPings){ pingGroupInfo.End = DateTime.Now; } // set time received

                pingGroupInfo.AveragePing += response.RoundtripTime;
                if (response.RoundtripTime < pingGroupInfo.MinimumPing){
                    pingGroupInfo.MinimumPing = response.RoundtripTime;
                }

                if (response.RoundtripTime > pingGroupInfo.MaximumPing){
                    pingGroupInfo.MaximumPing = response.RoundtripTime;
                }
                responseTimes[pingCounter] = response.RoundtripTime;
            }

            else if (currentPingState == PingingStates.PingState.Pause ||
                     currentPingState == PingingStates.PingState.Halt){
                // update time received? Change to TimeEnd? If pause, do I pause? Do I terminate and try again?
                // probably should pause, this way each pause results in a wait of 1 x WaitMs, 
                // as opposed to NumberOfPingsToSend x WaitMs (32 x 3 sec = 96 sec,or  > 1.5 minutes!)
                pingGroupInfo.End = DateTime.Now;
                pingGroupInfo.HaltingIPStatus = response.Status;
                 
                // 
                break;
            }
            pingCounter++; 

        }

       
        // here need to store the information on the PingGroupSummary object -- should initialize earlier
        // need to add exception handling, response.Status interpretation per documentation: https://learn.microsoft.com/en-us/dotnet/api/system.net.networkinformation.ping?view=net-8.0
        pingGroupInfo.Jitter = CalculatePingVariance(responseTimes, pingGroupInfo.AveragePing!.Value);
        pingGroupInfo.AveragePing /= pingCounter;
        pingGroupInfo.PacketLoss = packetsLost/pingCounter;  
        return pingGroupInfo;
    }

    public static float CalculatePingVariance(float[] responseTimes, float mean){
        if (responseTimes.Length <= 1){ 
            return 0;
        }

        double variance = 0;
        float deltaMean;
        foreach (float rtt in responseTimes){
            deltaMean = rtt-mean;
            variance += Math.Pow(deltaMean, 2);
        }

        variance = variance / responseTimes.Length;
        
        /* the narrowing conversion here is only a problem if:
           A) significant floating point precision is needed -- which isn't here
           B) there are impossibly long ping times -- which this application can't possibly have
           and so while it is technically dangerous, this function is fine for this application
        */
        return (float) variance;
    }


}