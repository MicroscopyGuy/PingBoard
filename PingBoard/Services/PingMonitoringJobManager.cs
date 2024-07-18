namespace PingBoard.Services;

public class PingMonitoringJobManager : BackgroundService
{
    private readonly Func<string, PingMonitoringJobRunner> _getPingMonitoringJobRunner;
    private readonly ILogger<PingMonitoringJobManager> _logger;
    private volatile PingMonitoringJobRunner? _currentJobRunner;
    private int _checkRunningJobsDelayMs = 100;
    private readonly object lockingObject = new object();
    
    public PingMonitoringJobManager(Func<string, PingMonitoringJobRunner> pingMonitoringJobRunnerSource,
        ILogger<PingMonitoringJobManager> logger)
    {
        _getPingMonitoringJobRunner = pingMonitoringJobRunnerSource;
        _logger = logger;
    }
    

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogDebug("PingMonitoringJobManager: ExecuteAsync: Entered");
        while (true)
        {
            Task? pingingTask;
            string pingingTarget = "";
            lock (lockingObject) {
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
                if (completedTask.IsCompleted)
                {
                    _logger.LogDebug($"PingMonitoringJobManager: PingingJob of target {pingingTarget} has completed");
                    ResetJobRunner(); //dont want to do this twice, endpoint already calls StopPinging() -> ResetJobRunner() -> Dispose()
                }
                
                if (completedTask.Exception != null)
                {
                    // in the future send a signal somewhere
                    _logger.LogError(completedTask.Exception.InnerException?.ToString() ??
                                     completedTask.Exception.ToString());
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
        lock (lockingObject) {
            return _currentJobRunner != null;
        }
    }
    
    public void StartPinging(string target)
    {
        lock (lockingObject)
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
        }
    }

    public void StopPinging()
    {
        lock (lockingObject)
        {
            _logger.LogDebug("PingMonitoringJobManager: StopPinging: Entered");
            if (_currentJobRunner != null)
            {
                _currentJobRunner.CancelTokenSource();
                ResetJobRunner();
            }
        }
    }

    public void ResetJobRunner()
    {
        lock (lockingObject)
        {
            _logger.LogDebug("PingMonitoringJobManager: ResetJobRunner: Entered");
            
            if (_currentJobRunner != null)
            {
                _logger.LogDebug("PingMonitoringJobManager: ResetJobRunner: Going to dispose of job runner");
                var cjr = _currentJobRunner;
                _currentJobRunner = null; // defensive measure to prevent intermediate state of disposed job runner, but non-null currentJObRunner
                cjr.Dispose();
            }
        }
    }
}
    