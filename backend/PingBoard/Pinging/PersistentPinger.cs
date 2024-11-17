using System.Net.Sockets;

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
public class PersistentPinger : IPersistentPinger{
    private readonly ILogger<IGroupPinger> _logger;
    private readonly PingingBehaviorConfig _pingBehavior;
    private readonly PingingThresholdsConfig _pingThresholds;
    private readonly IIndividualPinger _individualPinger;
    private readonly IPingScheduler _scheduler;
    
    public PersistentPinger(IIndividualPinger individualPinger,  IPingScheduler scheduler,
                       IOptions<PingingBehaviorConfig> pingBehavior, IOptions<PingingThresholdsConfig> pingThresholds,
                       ILogger<IGroupPinger> logger){
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
    public async IAsyncEnumerable<PingResult> SendPingsAsync(IPAddress target, CancellationToken stoppingToken = default(CancellationToken)){
        _logger.LogInformation("GroupPinger: Entered SendPingsAsync");
        PingingStates.PingState currentPingState = PingingStates.PingState.Continue;
        string ipVersion = target.AddressFamily == AddressFamily.InterNetwork ? "Ipv4" : "Ipv6";
        
        bool PingStateNotHalt() => currentPingState != PingingStates.PingState.Halt;
        bool NotCancelled() => !stoppingToken.IsCancellationRequested;

        while(PingStateNotHalt() && NotCancelled())
        {
            PingResult result = StartPingResult(target.ToString(), ipVersion, (short) _pingBehavior.Ttl, DateTime.UtcNow);
            _scheduler.StartIntervalTracking();
            PingReply response = await _individualPinger.SendPingIndividualAsync(target, stoppingToken);
            result = CompletePingResult(result, DateTime.UtcNow, response.Address.ToString(), (int)response.RoundtripTime, response.Status);

            currentPingState = IcmpStatusCodeLookup.StatusCodes[response.Status].State;
            _scheduler.EndIntervalTracking();
            await _scheduler.DelayPingingAsync();
            yield return result;
        }
     
    }

    public PingResult StartPingResult(string target, string targetType, short ttl, DateTime start)
    {
        return new PingResult()
        {
            Target = target,
            TargetType = targetType,
            Ttl = ttl,
            Start = start
        };
    }

    public PingResult CompletePingResult(PingResult inProgress, DateTime end, string replyAddress, int rtt, 
        IPStatus status)
    {
        inProgress.End = end;
        inProgress.ReplyAddress = replyAddress;
        inProgress.Rtt = rtt;
        inProgress.IpStatus = status;
        return inProgress;
    }
    
}