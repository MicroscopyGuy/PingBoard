using PingBoard.Pinging;
using System.Net;

public class NetworkMonitoringService : BackgroundService
{
    private readonly ILogger<GroupPinger> _logger;
    private readonly GroupPinger _groupPinger;

    public NetworkMonitoringService(ILogger<GroupPinger> logger, GroupPinger groupPinger)
    {
        _logger = logger;
        _groupPinger = groupPinger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            PingGroupSummary result = await _groupPinger.SendPingGroupAsync(IPAddress.Parse("8.8.8.8"), 8);
            Console.WriteLine(result.MaximumPing);
            await Task.Delay(1000, stoppingToken);
        }
    }
}