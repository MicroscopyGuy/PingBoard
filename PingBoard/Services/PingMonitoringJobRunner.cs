namespace PingBoard.Services;
using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Options;
using PingBoard.Pinging;
using PingBoard.Pinging.Configuration;
using PingBoard.DatabaseUtilities;
using System.Net;

[ExcludeFromCodeCoverage]
public class PingMonitoringJobRunner : IDisposable
{
    private readonly IGroupPinger _groupPinger;
    private readonly DatabaseHelper _databaseHelper;
    private CancellationTokenSource _cancellationTokenSource;
    private readonly string _target;
    private Task _pingingTask;
    private readonly ILogger<IGroupPinger> _logger;

    public PingMonitoringJobRunner(IGroupPinger groupPinger, IOptions<PingingBehaviorConfig> pingingBehavior, 
                                   IOptions<PingingThresholdsConfig> pingingThresholds, PingingBehaviorConfigValidator behaviorValidator, 
                                   PingingThresholdsConfigValidator thresholdsValidator, DatabaseHelper databaseHelper, 
                                   CancellationTokenSource cancellationTokenSource, string target, ILogger<IGroupPinger> logger){
        _logger = logger;
        _logger.LogDebug("PingMonitoringJobRunner: Entered Constructor");
        _groupPinger = groupPinger;
        _databaseHelper = databaseHelper;
        _cancellationTokenSource = cancellationTokenSource;
        _target = target;
        
        // validate configured information
        thresholdsValidator.ValidateAndThrow(pingingThresholds.Value);
        behaviorValidator.ValidateAndThrow(pingingBehavior.Value);
        
        // for logging to solve bug:
        _logger.LogDebug("PingMonitoringJobRunner: Constructor: CancellationTokenSourceHash: {ctsHash}", 
            _cancellationTokenSource.GetHashCode());
    }

    public void StartPinging(){
        _logger.LogDebug("PingMonitoringJobRunner: StartPinging Entered");

        if (_cancellationTokenSource.IsCancellationRequested)
        {
            _logger.LogDebug("PingMonitoringJobRunner: StartPinging: Cancellation requested, aborting start");
            return;
        }
        var stoppingToken = _cancellationTokenSource.Token;
        _pingingTask = ExecutePingingAsync(stoppingToken);
    }
    
    private async Task ExecutePingingAsync(CancellationToken stoppingToken){
        _logger.LogDebug("PingMonitoringJobRunner: ExecutePingAsync Entered");
        _databaseHelper.InitializeDatabase();
        var result = new PingGroupSummary{TerminatingIPStatus = null};
        
        try
        {
            while (!stoppingToken.IsCancellationRequested && result.TerminatingIPStatus == null)
            {
                result = await _groupPinger.SendPingGroupAsync(IPAddress.Parse(_target));
                Console.WriteLine(result.ToString());
                _databaseHelper.InsertPingGroupSummary(result);
            }
        }
        
        catch (Exception e)
        {
            _logger.LogError(e.ToString());
        }
    }

    public Task GetPingingTask()
    {
        _logger.LogDebug("PingMonitoringJobRunner: Entered GetPingingTask");
        return _pingingTask;
    }

    public string GetTarget()
    {
        _logger.LogDebug("PingMonitoringJobRunner: Entered GetTarget");
        return _target;
    }

    public void CancelTokenSource()
    {
        try
        {
            _logger.LogDebug("PingMonitoringJobRunner: Entered CancelTokenSource");
            _logger.LogDebug("PingMonitoringJobRunner: CancelTokenSource: CancellationTokenSourceHash: {ctsHash}", 
                _cancellationTokenSource.GetHashCode());
            _cancellationTokenSource.Cancel();
        }
 
        catch (ObjectDisposedException oDE) {
            _logger.LogError($"PingMonitoringJobRunner: CancelTokenSource: {oDE}");
        }

    }
    
    
    public void Dispose()
    {
        try
        {
            _logger.LogDebug("PingMonitoringJobRunner: Entered Dispose");
            _logger.LogDebug("PingMonitoringJobRunner: Dispose: CancellationTokenSourceHash: {ctsHash}", 
                _cancellationTokenSource.GetHashCode());
            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = null;
            
            //_databaseHelper.Dispose(); // newly added
        }
        catch (System.ObjectDisposedException oDE)
        {
            _logger.LogError($"PingMonitoringJobRunner: Dispose: {oDE}");
        }
    }
}