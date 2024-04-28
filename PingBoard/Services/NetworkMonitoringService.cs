using PingBoard.Pinging;
using System.Net;

public class NetworkMonitoringService : BackgroundService
{
    private readonly ILogger<Pinger> _logger;
    private readonly Pinger _pinger;

    public NetworkMonitoringService(ILogger<Pinger> logger, Pinger pinger)
    {
        _logger = logger;
        _pinger = pinger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            PingGroupSummary result = await _pinger.SendPingGroupAsync(IPAddress.Parse("8.8.8.8"), 8);
            Console.WriteLine(result.MaximumPing);
            await Task.Delay(1000, stoppingToken);
        }
    }
}