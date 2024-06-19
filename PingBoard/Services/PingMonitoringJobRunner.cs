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
       
        _groupPinger = groupPinger;
        _databaseHelper = databaseHelper;
        _cancellationTokenSource = cancellationTokenSource;
        _target = target;
        _logger = logger;
        
        // validate configured information
        thresholdsValidator.ValidateAndThrow(pingingThresholds.Value);
        behaviorValidator.ValidateAndThrow(pingingBehavior.Value);
    }

    public void StartPinging()
    {
        var stoppingToken = _cancellationTokenSource.Token;
        _pingingTask = ExecutePingingAsync(stoppingToken);
    }
    
    private async Task ExecutePingingAsync(CancellationToken stoppingToken){
        _logger.LogDebug("PingMonitoringJob: ExecutePingAsync Entered");
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
        return _pingingTask;
    }

    public string GetTarget()
    {
        return _target;
    }

    public void CancelTokenSource()
    {
        _cancellationTokenSource.Cancel();
    }

    public bool CancellationTokenSourceIsNull()
    {
        return _cancellationTokenSource == null;
    }
    
    public void Dispose()
    {
        _cancellationTokenSource.Dispose();
        _cancellationTokenSource = null;
    }
}