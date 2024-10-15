using PingBoard.Database.Models;

namespace PingBoard.Services;
using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Options;
using PingBoard.Pinging;
using PingBoard.Pinging.Configuration;
using Database.Utilities;
using System.Net;

[ExcludeFromCodeCoverage]
public class PingMonitoringJobRunner : IDisposable
{
    private readonly IGroupPinger _groupPinger;
    private readonly DatabaseHelper _databaseHelper;
    private CancellationTokenSource _cancellationTokenSource;
    private readonly string _target;
    private Task _pingingTask;
    private ServerEventEmitter _serverEventEmitter;
    private PingQualification _pingQualifier;
    private readonly ILogger<PingMonitoringJobRunner> _logger;

    public PingMonitoringJobRunner(IGroupPinger groupPinger, IOptions<PingingBehaviorConfig> pingingBehavior, 
                                   IOptions<PingingThresholdsConfig> pingingThresholds, PingingBehaviorConfigValidator behaviorValidator, 
                                   PingingThresholdsConfigValidator thresholdsValidator, DatabaseHelper databaseHelper, 
                                   PingQualification pingQualifier, CancellationTokenSource cancellationTokenSource,
                                   ServerEventEmitter serverEventEmitter, string target, ILogger<PingMonitoringJobRunner> logger){
        _logger = logger;
        _logger.LogDebug("PingMonitoringJobRunner: Entered Constructor");
        _groupPinger = groupPinger;
        _databaseHelper = databaseHelper;
        _pingQualifier = pingQualifier;
        _cancellationTokenSource = cancellationTokenSource;
        _serverEventEmitter = serverEventEmitter;
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
                result = await _groupPinger.SendPingGroupAsync(IPAddress.Parse(_target), stoppingToken);
                Console.WriteLine(result.ToString());

                var trippedFlags = _pingQualifier.CalculatePingQualityFlags(result);
                bool anomaly = !PingQualification.PingQualityWithinThresholds(trippedFlags);
                
                if(anomaly)
                {
                    _serverEventEmitter.IndicatePingAnomaly(
                        _target, 
                        PingQualification.DescribePingQualityFlags(trippedFlags),
                        "PingMonitoringJobRunner: ExecutePingingAsync"
                        );
                }
                _databaseHelper.InsertPingGroupSummary(result);
            }
            _databaseHelper.Dispose();
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

    public async Task CancelTokenSourceAsync()
    {
        try
        {
            _logger.LogDebug("PingMonitoringJobRunner: Entered CancelTokenSource");
            _logger.LogDebug("PingMonitoringJobRunner: CancelTokenSource: CancellationTokenSourceHash: {ctsHash}", 
                _cancellationTokenSource.GetHashCode());
            await _cancellationTokenSource.CancelAsync();
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
        }
        
        catch (System.ObjectDisposedException oDE)
        {
            _logger.LogError($"PingMonitoringJobRunner: Dispose: {oDE}");
        }
    }
}