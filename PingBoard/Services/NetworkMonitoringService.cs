using PingBoard.Pinging;
using System.Net;

public class NetworkMonitoringService : BackgroundService
{
    private readonly ILogger<IGroupPinger> _logger;
    private readonly IGroupPinger _groupPinger;

    public NetworkMonitoringService(ILogger<IGroupPinger> logger, IGroupPinger groupPinger)
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