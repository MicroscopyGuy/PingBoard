namespace PingBoard.Services;

public class PingMonitoringJobManager : BackgroundService
{
    private readonly Func<string, PingMonitoringJobRunner> _getPingMonitoringJobRunner;
    private readonly ILogger<PingMonitoringJobManager> _logger;
    private volatile PingMonitoringJobRunner? _currentJobRunner;
    private int _checkRunningJobsDelayMs = 100;
    private readonly object _lockingObject = new object();
    private PingStatusIndicator _pingStatusIndicator;
    
    public PingMonitoringJobManager(Func<string, PingMonitoringJobRunner> pingMonitoringJobRunnerSource,
        PingStatusIndicator pingStatusIndicator, ILogger<PingMonitoringJobManager> logger)
    {
        _getPingMonitoringJobRunner = pingMonitoringJobRunnerSource;
        _pingStatusIndicator = pingStatusIndicator;
        _logger = logger;
    }
    

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogDebug("PingMonitoringJobManager: ExecuteAsync: Entered");
        while (!stoppingToken.IsCancellationRequested)
        {
            Task? pingingTask;
            string pingingTarget = "";
            lock (_lockingObject) {
                if (IsPinging()) {
                    //_logger.LogDebug("PingMonitoringJobManager: ExecuteAsync: AlreadyPinging");
                    pingingTask = _currentJobRunner!.GetPingingTask();
                    pingingTarget = _currentJobRunner.GetTarget();
                }

                else {
                    pingingTask = null;
                }
            }

            if (pingingTask != null)
            {
               //logger.LogDebug($"PingMonitoringJobManager: ExecuteAsync: Currently pinging {pingingTarget}");
                var completedTask = await Task.WhenAny(pingingTask);

                // if job is done, reset _currentJobRunner and the pinging target
                // For overview of task properties:
                //      https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task?view=net-8.0
                if (completedTask.IsCompleted)
                {
                    _logger.LogDebug($"PingMonitoringJobManager: ExecuteAsync: PingingJob of target {pingingTarget} has completed");
                    ResetJobRunner(); // keep reset just in case, since reset checks to see if it's safe, first
                    IndicateChangedPingStatus(pingingTarget, false, "ExecuteAsync");
                }
                
                if (completedTask.Exception != null)
                {
                    // in the future send a signal somewhere
                    _logger.LogError(completedTask.Exception.InnerException?.ToString() ??
                                     completedTask.Exception.ToString());
                }
                // Just a measure to make sure an uncaught exception will always be logged
                if (completedTask.IsFaulted)
                {
                    _logger.LogCritical("PingMonitoringJobManager: ExecuteAsync: pinging task of target {target} had an unhandled exception", pingingTarget);
                }
            }

            else
            {
                await Task.Delay(_checkRunningJobsDelayMs);
            }
        }
    }

    /// <summary>
    /// Returns whether any jobs are running
    /// </summary>
    /// <returns>true if a job is running, and false otherwise</returns>
    public bool IsPinging()
    {
        //_logger.LogDebug("PingMonitoringJobManager: IsPinging: Entered");
        lock (_lockingObject) {
            return _currentJobRunner != null;
        }
    }
    
    public void StartPinging(string target)
    {
        lock (_lockingObject)
        {
            _logger.LogDebug($"PingMonitoringJobManager: StartPinging: Entered with target:{target}"); 
            // simply do nothing if the target is already being pinged
            if (_currentJobRunner != null)
            {
                _logger.LogDebug($"PingMonitoringJobManager: StartPinging: Already pinging"); 
                return;
            }
            
            _currentJobRunner = _getPingMonitoringJobRunner(target);
            //_logger.LogDebug($"PingMonitoringJobManager: StartPinging: new job runner created:{_currentJobRunner.GetHashCode()}");
            _currentJobRunner.StartPinging();
            IndicateChangedPingStatus(_currentJobRunner.GetTarget(), true, "StartPinging");
        }
    }

    public void StopPinging()
    {
        lock (_lockingObject)
        {
            _logger.LogDebug("PingMonitoringJobManager: StopPinging: Entered");
            if (_currentJobRunner != null)
            {
                // save before getting rid of the job runner, so UI knows the target that is no longer being pinged
                var oldTarget = _currentJobRunner.GetTarget(); 
                _currentJobRunner.CancelTokenSource();
                ResetJobRunner();
                IndicateChangedPingStatus(oldTarget, false, "StopPinging");
            }
        }
    }

    public void ResetJobRunner()
    {
        lock (_lockingObject)
        {
            _logger.LogDebug("PingMonitoringJobManager: ResetJobRunner: Entered");
            
            if (_currentJobRunner != null)
            {
                _logger.LogDebug("PingMonitoringJobManager: ResetJobRunner: Going to dispose of job runner");
                var cjr = _currentJobRunner;
                _currentJobRunner = null; // defensive measure to prevent intermediate state of disposed job runner, but non-null currentJobRunner
                cjr.Dispose();

            }
        }
    }

    public void IndicateChangedPingStatus(string target, bool status, string caller)
    {
        try
        {
            var writeSuccess = _pingStatusIndicator.Writer.TryWrite(new PingStatusMessage
            {
                PingTarget = new PingTarget { Target = target },
                Active = status
            });
                
            if (!writeSuccess)
            {
                string message = "An attempt to write to the PingStatusIndicator channel was unsuccessful.";
                throw (new InvalidOperationException(message));
            }
            _logger.LogDebug($"PingMonitoringJobManager: IndicateChangedPingStatus: target{target}, status:{status}, caller:{caller}", target, status, caller);
        }
        catch (Exception e)
        {
            _logger.LogCritical("PingMonitoringJobManager: IndicateChangedPingStatus: ${caller}: ${eText}", e.ToString);
        }
    }
}
    