namespace PingBoard.Pinging;
using Microsoft.Extensions.Logging.Abstractions;
using PingBoard.Database.Models;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.NetworkInformation;
using Microsoft.Extensions.Options;
using PingBoard.Pinging.Configuration;

/// <summary>
/// A class which allows the sending of *groups* of pings, an abstraction of the IndividualPinger class.
/// </summary>
public class GroupPinger : IGroupPinger{
    private readonly ILogger<IGroupPinger> _logger;
    private readonly PingingBehaviorConfig _pingBehavior;
    private readonly PingingThresholdsConfig _pingThresholds;
    private readonly PingQualification _pingQualifier; 
    private readonly IIndividualPinger _individualPinger;
    private readonly IPingScheduler _scheduler;
    
    public GroupPinger(IIndividualPinger individualPinger, PingQualification pingQualifier, IPingScheduler scheduler,
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
    ///     A function which asynchronously sends a group of pings and reports back a summary of their trips
    /// </summary>
    /// <param name="target">A domain or IP Address that the user wishes to send pings to</param>
    /// <param name="stoppingToken">An optional CancellationToken which if cancelled, indicates a user's desire to stop pinging</param>
    /// 
    /// <returns> 
    ///     A PingGroupSummary object which summarizes the results of the pings that were sent
    /// </returns>
    public async Task<PingGroupSummary> SendPingGroupAsync(IPAddress target, CancellationToken stoppingToken = default(CancellationToken)){
        _logger.LogInformation("GroupPinger: Entered SendPingGroupAsync");
        Console.WriteLine("GroupPinger: Entered SendPingGroupAsync");
        PingGroupSummary pingGroupInfo = PingGroupSummary.Empty();
        pingGroupInfo.Target = target.ToString();
        PingingStates.PingState currentPingState = PingingStates.PingState.Continue;
        List<long> responseTimes = new List<long>();
        int pingCounter = 0;
        pingGroupInfo.Start = DateTime.UtcNow;

        bool HasRemainingPings() => pingCounter++ < _pingBehavior.PingsPerCall;
        bool PingStateNotHalt() => currentPingState != PingingStates.PingState.Halt;
        bool BelowReportThreshold() => pingGroupInfo.ConsecutiveTimeouts < _pingBehavior.ReportBackAfterConsecutiveTimeouts;
        bool NotCancelled() => !stoppingToken.IsCancellationRequested;

        while(HasRemainingPings() && PingStateNotHalt() && BelowReportThreshold() && NotCancelled()){
            _scheduler.StartIntervalTracking();
            PingReply response = await _individualPinger.SendPingIndividualAsync(target, stoppingToken);
            pingGroupInfo.End = DateTime.UtcNow;  // set time received, since may terminate prematurely keep this up to date
            currentPingState = IcmpStatusCodeLookup.StatusCodes[response.Status].State;
            pingGroupInfo.PacketsSent++;
            pingGroupInfo.ExcludedPings += (byte) ((response.Status == IPStatus.Success) ? 0 : 1);
            
            switch(currentPingState){
                case PingingStates.PingState.Continue: ProcessContinue(pingGroupInfo, response, responseTimes); break;
                case PingingStates.PingState.Halt:     ProcessHalt(pingGroupInfo, response); break;
                case PingingStates.PingState.Pause:    ProcessPause(pingGroupInfo, response); break;
                case PingingStates.PingState.PacketLossCaution: ProcessPacketLossCaution(pingGroupInfo, response); break;
            }
            
            _scheduler.EndIntervalTracking();
            await _scheduler.DelayPingingAsync();
        }
        
        pingGroupInfo.AveragePing = PingGroupSummary.CalculateAveragePing(pingGroupInfo);
        pingGroupInfo.Jitter      = PingGroupSummary.CalculatePingJitter(responseTimes);
        pingGroupInfo.PacketLoss  = PingGroupSummary.CalculatePacketLoss(pingGroupInfo.PacketsSent, pingGroupInfo.PacketsLost);
        pingGroupInfo.ResetMinMaxPingsIfUnused();
        
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
    [ExcludeFromCodeCoverage]
    public static void ProcessContinue(PingGroupSummary currentPingGroup, PingReply reply, List<long> rtts) {
        currentPingGroup.ConsecutiveTimeouts = 0;
        currentPingGroup.AveragePing += reply.RoundtripTime;
        
        if (reply.Status != IPStatus.Success) {
            currentPingGroup.LastAbnormalStatus = reply.Status;
            return;
        }
        
        PingGroupSummary.SetIfMinPing(currentPingGroup, (short) reply.RoundtripTime);
        PingGroupSummary.SetIfMaxPing(currentPingGroup, (short) reply.RoundtripTime);
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
    public static void ProcessHalt(PingGroupSummary currentPingGroup, PingReply reply){
        currentPingGroup.TerminatingIPStatus = reply.Status;
        currentPingGroup.ConsecutiveTimeouts = 0;
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
    public static void ProcessPause(PingGroupSummary currentPingGroup, PingReply reply) {
        currentPingGroup.LastAbnormalStatus = reply.Status;
        currentPingGroup.ConsecutiveTimeouts = 0;
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
        currentPingGroup.PacketsLost++;
        currentPingGroup.ConsecutiveTimeouts++;
        currentPingGroup.LastAbnormalStatus = IPStatus.TimedOut;
    }

}

