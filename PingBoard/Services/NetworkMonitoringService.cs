using Microsoft.Extensions.Options;
using PingBoard.Pinging;
using System.Net;

public class NetworkMonitoringService : BackgroundService
{
    private readonly ILogger<IGroupPinger> _logger;
    private readonly IGroupPinger _groupPinger;

    private readonly PingingBehaviorConfig _pingingBehavior;

    public NetworkMonitoringService( IGroupPinger groupPinger, IOptions<PingingBehaviorConfig> pingingBehavior,
                                     ILogger<IGroupPinger> logger){
        _pingingBehavior = pingingBehavior.Value;
        _logger = logger;
        _groupPinger = groupPinger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            PingGroupSummary result = await _groupPinger.SendPingGroupAsync(IPAddress.Parse("8.8.8.8"), 64);
            if (!PingQualification.PingQualityWithinThresholds(result.PingQualityFlags) || result.TerminatingIPStatus != null){
                Console.WriteLine($"MinimumPing: {result.MinimumPing} AveragePing: {result.AveragePing} " +
                                  $"MaximumPing: {result.MaximumPing} Jitter: {result.Jitter} PacketLoss: {result.PacketLoss} " +
                                  $"TerminatingIPStatus: {result.TerminatingIPStatus} EndTime: {result.End}");
            }
            

            await Task.Delay(_pingingBehavior.WaitMs, stoppingToken);
        }
    }
}