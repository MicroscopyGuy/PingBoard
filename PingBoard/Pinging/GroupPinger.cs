using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.NetworkInformation;
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
        private readonly PingingThresholdsConfig _pingThresholds;
        private readonly PingQualification _pingQualifier; 
        private readonly IIndividualPinger _individualPinger;
        private readonly PingScheduler _scheduler;

        public GroupPinger(IIndividualPinger individualPinger, PingQualification pingQualifier, PingScheduler scheduler,
                           IOptions<PingingBehaviorConfig> pingBehavior, IOptions<PingingThresholdsConfig> pingThresholds,
                           ILogger<IGroupPinger> logger){
            _pingQualifier    = pingQualifier;
            _pingBehavior     = pingBehavior.Value;
            _pingThresholds   = pingThresholds.Value;
            _logger           = logger;
            _individualPinger = individualPinger;
            _scheduler        = scheduler;
        }

        /// <summary>
        ///     An asynchronous function which sends a group of pings and reports back their metrics.
        /// </summary>
        /// <param name="target">A domain or IP Address that the user wishes to send pings to</param>
        /// <param name="numberOfPings">The number of pings to be sent in the group</param>
        /// <returns> 
        ///     A PingGroupSummary object which summarizes the results of the pings that were sent
        /// </returns>
        public async Task<PingGroupSummary> SendPingGroupAsync(IPAddress target, int numberOfPings){
            PingGroupSummary pingGroupInfo = PingGroupSummary.Empty();
            PingingStates.PingState currentPingState = PingingStates.PingState.Continue;
            List<long> responseTimes = new List<long>();
            int pingCounter = 0;
            pingGroupInfo.Start = DateTime.UtcNow;
            
            Func<bool> hasRemainingPings    = () => pingCounter < numberOfPings;
            Func<bool> pingStateIsContinue  = () => currentPingState == PingingStates.PingState.Continue;
            Func<bool> belowReportThreshold = () => pingGroupInfo.ConsecutiveTimeouts < _pingBehavior.ReportBackAfterConsecutiveTimeouts;

            while(hasRemainingPings() && pingStateIsContinue() && belowReportThreshold()){
                _scheduler.StartIntervalTracking();
                PingReply response = await _individualPinger.SendPingIndividualAsync(target);
                pingGroupInfo.End = DateTime.UtcNow;  // set time received, since may terminate prematurely keep this up to date
                currentPingState = IcmpStatusCodeLookup.StatusCodes[response.Status].State;
                
                switch(currentPingState){
                    case PingingStates.PingState.Continue: ProcessContinue(pingGroupInfo, response, responseTimes); break;
                    case PingingStates.PingState.Halt:     ProcessHalt(pingGroupInfo, response, responseTimes); break;
                    case PingingStates.PingState.Pause:    ProcessPause(pingGroupInfo, response, responseTimes); break;
                    case PingingStates.PingState.PacketLossCaution: ProcessPacketLossCaution(pingGroupInfo, response); break;
                }

                pingCounter++;
                _scheduler.EndIntervalTracking();
                await _scheduler.DelayPingingAsync();
            }
            
            pingGroupInfo.AveragePing = PingGroupSummary.CalculateAveragePing(pingGroupInfo.AveragePing!.Value, pingGroupInfo.PacketsSent!.Value, pingGroupInfo.PacketsLost!.Value);
            pingGroupInfo.Jitter      = PingGroupSummary.CalculatePingJitter(responseTimes);
            pingGroupInfo.PacketLoss  = PingGroupSummary.CalculatePacketLoss(pingGroupInfo.PacketsSent!.Value, pingGroupInfo.PacketsLost!.Value);
            pingGroupInfo.PingQualityFlags = _pingQualifier.CalculatePingQualityFlags(pingGroupInfo);
            return pingGroupInfo;
        }

        /// <summary>
        /// A helper function for SendPingGroupAsync which handles the logic in the event there is a Continue state.
        /// It does so by updating three properties (AveragePing, MinimumPing, MaximumPing) as appropriate,
        /// and then storing the most recent ping time in the array of response times that SendPingGroupAsync
        /// will eventually pass to a function that calculates jitter.
        /// </summary>
        /// <param name="currentPingGroup">
        ///     The PingGroupSummary object summarizing the ping group that SendPingGroupAsync is working on sending
        /// </param>
        /// <param name="reply">
        ///     The PingReply object retrieved from the IndividualPinger's latest ping function call
        /// </param>
        /// <param name="rtts">
        ///     The list of response times which is used to calculate jitter. It is declared and instantiated in
        ///     SendPingGroupAsync.
        /// </param>
        /// <param name="pingCounter">
        ///     A counter to indicate the sequential number of the ping that was last sent
        /// </param>
        [ExcludeFromCodeCoverage]
        public static void ProcessContinue(PingGroupSummary currentPingGroup, PingReply reply, List<long> rtts){
            currentPingGroup.PacketsSent++;
            currentPingGroup.AveragePing += reply.RoundtripTime;
            
            if (reply.RoundtripTime < currentPingGroup.MinimumPing){ 
                currentPingGroup.MinimumPing = (short) reply.RoundtripTime; 
            }
            
            if (reply.RoundtripTime > currentPingGroup.MaximumPing){
                currentPingGroup.MaximumPing = (short) reply.RoundtripTime; 
            }
            rtts.Add(reply.RoundtripTime);
        }

        /// <summary>
        ///     A helper method for SendPingGroupAsync, and which handles the logic in the event there is a Halt state
        /// </summary>
        /// <param name="currentPingGroup">
        ///     The PingGroupSummary object summarizing the ping group that SendPingGroupAsync is working on sending
        /// </param>
        /// <param name="reply">
        ///     The PingReply object retrieved from the IndividualPinger's latest ping function call
        /// </param>
        public static void ProcessHalt(PingGroupSummary currentPingGroup, PingReply reply, List<long> rtts){
            currentPingGroup.PacketsSent++;
            currentPingGroup.TerminatingIPStatus = reply.Status;
            PingGroupSummary.SetIfMinPing(currentPingGroup, (short) reply.RoundtripTime);
            PingGroupSummary.SetIfMaxPing(currentPingGroup, (short) reply.RoundtripTime);
            rtts.Add(reply.RoundtripTime);
        }


        /// <summary>
        ///     At the moment, pausing has the same behavior as ProcessStop, but it may not in the future.
        ///     For the time being this simply invokes the ProcessHalt() function.
        /// </summary>
        /// <param name="currentPingGroup">
        ///     The PingGroupSummary object summarizing the ping group that SendPingGroupAsync is working on sending
        /// </param>
        /// <param name="reply">
        ///     The PingReply object retrieved from the IndividualPinger's latest ping function call
        /// </param>
        /// <param name="rtts">
        ///     The list of response times which is used to calculate jitter. It is declared and instantiated in
        ///     SendPingGroupAsync.
        /// </param>
        public static void ProcessPause(PingGroupSummary currentPingGroup, PingReply reply, List<long> rtts){
            ProcessHalt(currentPingGroup, reply, rtts);       
        }


        /// <summary>
        ///     A helper method for SendPingGroupAsync, and which handles the logic in the event there is 
        ///     a PacketLossCaution state. 
        /// </summary>
        /// <param name="currentPingGroup">
        ///     The PingGroupSummary object summarizing the ping group that SendPingGroupAsync is working on sending
        /// </param>
        /// <param name="reply">
        ///     The PingReply object retrieved from the IndividualPinger's latest ping function call
        /// </param>
        public static void ProcessPacketLossCaution(PingGroupSummary currentPingGroup, PingReply reply){
            currentPingGroup.PacketsSent++;
            currentPingGroup.PacketsLost++;
            currentPingGroup.ConsecutiveTimeouts++;
        }

    }

}