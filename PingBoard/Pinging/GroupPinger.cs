using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;
using Pingboard.Pinging;
using PingBoard.Pinging.Configuration;


namespace PingBoard.Pinging{
    /// <summary>
    /// A class which allows the sending of *groups* of pings, an abstraction of the IndividualPinger class.
    /// </summary>
    public class GroupPinger : IGroupPinger{
        private readonly ILogger<IGroupPinger> _logger;
        private readonly PingingBehaviorConfig _pingBehavior;
        private readonly PingQualification _pingQualifier; 
        private readonly IIndividualPinger _individualPinger;

        private readonly PingScheduler _scheduler;

        public GroupPinger(IIndividualPinger individualPinger, PingQualification pingQualifier, PingScheduler scheduler,
                        IOptions<PingingBehaviorConfig> pingBehavior, ILogger<IGroupPinger> logger){
            _pingQualifier    = pingQualifier;
            _pingBehavior     = pingBehavior.Value;
            _logger           = logger;
            _individualPinger = individualPinger;
            _scheduler        = scheduler;
        }

        public async Task<PingGroupSummary> SendPingGroupAsync(IPAddress target, int numberOfPings){
            PingGroupSummary pingGroupInfo = PingGroupSummary.Empty();
            long[] responseTimes = new long[numberOfPings];

            pingGroupInfo.Start = DateTime.UtcNow;
            
            int packetsLost = 0;
            int pingCounter = 0;
            PingingStates.PingState currentPingState = PingingStates.PingState.Continue;
            while(pingCounter < numberOfPings && currentPingState == PingingStates.PingState.Continue){
                _scheduler.StartIntervalTracking();
    
                PingReply response = await _individualPinger.SendPingIndividualAsync(target);
                pingGroupInfo.End = DateTime.UtcNow;  // set time received, since may terminate prematurely keep this up to date
                packetsLost += (response.Status == IPStatus.TimedOut) ? 1 : 0;
                currentPingState = IcmpStatusCodeLookup.StatusCodes[response.Status].State;

                if (currentPingState == PingingStates.PingState.Continue){
                    pingGroupInfo.AveragePing += response.RoundtripTime;
                    if (response.RoundtripTime < pingGroupInfo.MinimumPing){ pingGroupInfo.MinimumPing = (short) response.RoundtripTime; }
                    if (response.RoundtripTime > pingGroupInfo.MaximumPing){ pingGroupInfo.MaximumPing = (short) response.RoundtripTime; }
                    responseTimes[pingCounter] = response.RoundtripTime;
                }

                else if (currentPingState == PingingStates.PingState.Pause || currentPingState == PingingStates.PingState.Halt){
                    pingGroupInfo.TerminatingIPStatus = response.Status; 
                }

                pingCounter++;
                _scheduler.EndIntervalTracking();
                await Task.Delay(_scheduler.CalculateDelayToEvenlySpreadPings());
            }
            
            pingGroupInfo.AveragePing = (float) Math.Round(pingCounter > 0 ? pingGroupInfo.AveragePing!.Value/pingCounter : 0, 3);
            pingGroupInfo.Jitter = PingGroupSummary.CalculatePingJitter(responseTimes);
            pingGroupInfo.PacketLoss = pingCounter > 0 ? packetsLost/pingCounter * 100 : 0;
            pingGroupInfo.PingQualityFlags = _pingQualifier.CalculatePingQualityFlags(pingGroupInfo);
            return pingGroupInfo;
        }

    }

}