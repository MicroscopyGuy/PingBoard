using PingBoard.Pinging;

namespace PingBoard.Services;

public class PingMonitoringServiceManager : BackgroundService
{
    private readonly Func<PingMonitoringService> _getPingMonitoringService;
    private readonly Logger<PingMonitoringServiceManager> _logger;
    
    public PingMonitoringServiceManager(Func<PingMonitoringService> pingMonitoringServiceSource,
        Logger<PingMonitoringServiceManager> logger)
    {

        _getPingMonitoringService = pingMonitoringServiceSource;
        _logger = logger;
    }
    
    
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        throw new NotImplementedException();
    }
    
}