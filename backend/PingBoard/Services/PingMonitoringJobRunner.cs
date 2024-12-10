namespace PingBoard.Services;
using PingBoard.Database.Utilities;
using PingBoard.Database.Models;
using FluentValidation;
using Microsoft.Extensions.Options;
using PingBoard.Pinging;
using PingBoard.Pinging.Configuration;
using System.Net;

/// <summary>
/// A disposable, self-contained class created by the PingMonitoringJobManager in response to a request
/// to ping a particular domain or IP address. 
/// </summary>
public class PingMonitoringJobRunner : IDisposable
{
    private readonly IGroupPinger _groupPinger;
    private readonly CrudOperations _crudOperations;
    private CancellationTokenSource _cancellationTokenSource;
    private readonly string _target;
    private Task _pingingTask;
    private ServerEventEmitter _serverEventEmitter;
    private PingGroupQualifier _pingQualifier;
    private readonly ILogger<NetworkProbeLiason> _logger;

    public PingMonitoringJobRunner(IGroupPinger groupPinger, IOptions<PingingBehaviorConfig> pingingBehavior, 
                                   IOptions<PingingThresholdsConfig> pingingThresholds, PingingBehaviorConfigValidator behaviorValidator, 
                                   PingingThresholdsConfigValidator thresholdsValidator, CrudOperations crudOperations, 
                                   PingGroupQualifier pingQualifier, CancellationTokenSource cancellationTokenSource,
                                   ServerEventEmitter serverEventEmitter, string target){
        //_logger = logger;
        _logger.LogDebug("PingMonitoringJobRunner: Entered Constructor");
        _groupPinger = groupPinger;
        _crudOperations = crudOperations;
        _pingQualifier = pingQualifier;
        _cancellationTokenSource = cancellationTokenSource;
        _serverEventEmitter = serverEventEmitter;
        _target = target;
        
        // validate configured information
        //thresholdsValidator.ValidateAndThrow(pingingThresholds.Value);
        //behaviorValidator.ValidateAndThrow(pingingBehavior.Value);    
        
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
        var result = new PingGroupSummary{TerminatingIPStatus = null};
        
        try
        {
            while (!stoppingToken.IsCancellationRequested && result.TerminatingIPStatus == null)
            {
                result = await _groupPinger.SendPingGroupAsync(IPAddress.Parse(_target), stoppingToken);
                Console.WriteLine(result.ToString());
                await _crudOperations.InsertPingGroupSummaryAsync(result, stoppingToken);
                var trippedFlags = _pingQualifier.CalculatePingQualityFlags(result);
                bool anomaly = !PingGroupQualifier.PingQualityWithinThresholds(trippedFlags);
                string caller = "PingMonitoringJobRunner: ExecutePingingAsync";
                
                if(anomaly)
                {
                    var anomalyDescription = PingGroupQualifier.DescribePingQualityFlags(trippedFlags);
                    _logger.LogDebug($"PingMonitoringJobRunner: New anomaly detected {anomalyDescription}");
                    _serverEventEmitter.IndicatePingAnomaly(_target, anomalyDescription, caller);
                }

                _serverEventEmitter.IndicatePingInfo(_target, caller);
            }
        }
        
        catch (Exception e)
        {
            _logger.LogError(e.ToString());
        }
    }

    /// <summary>
    /// A simple getter for the _pingingTask property.
    /// </summary>
    /// <returns></returns>
    public Task GetPingingTask()
    {
        _logger.LogDebug("PingMonitoringJobRunner: Entered GetPingingTask");
        return _pingingTask;
    }

    /// <summary>
    /// A simple getter for the _target property, used to return what the current ping target is.
    /// </summary>
    /// <returns></returns>
    public string GetTarget()
    {
        _logger.LogDebug("PingMonitoringJobRunner: Entered GetTarget");
        return _target;
    }

    /// <summary>
    /// Used to mark the CancellationToken passed to the GroupPinger as cancelled.
    /// </summary>
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
    
    /// <summary>
    /// Safely disposes of the CancellationTokenSource when the current PingMonitoringJobRunner is no longer needed.
    /// </summary>
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