namespace PingBoard.Services;

public class PingMonitoringJobManager : BackgroundService
{
    private readonly Func<string, PingMonitoringJobRunner> _getPingMonitoringJobRunner;
    private readonly ILogger<PingMonitoringJobManager> _logger;
    private PingMonitoringJobRunner? _currentJobRunner;
    private int _checkRunningJobsDelayMs = 20;
    private readonly object lockingObject = new object();
    
    public PingMonitoringJobManager(Func<string, PingMonitoringJobRunner> pingMonitoringJobRunnerSource,
        ILogger<PingMonitoringJobManager> logger)
    {
        _getPingMonitoringJobRunner = pingMonitoringJobRunnerSource;
        _logger = logger;
    }
    

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogDebug("PingMonitoringJobManager: Started");
        await Task.Delay(Timeout.Infinite, stoppingToken);
        while (true)
        {
            Task? pingingTask;
            string pingingTarget = "";
            lock (lockingObject)
            {
                if (IsPinging())
                {
                    pingingTask = _currentJobRunner!.GetPingingTask();
                    pingingTarget = _currentJobRunner.GetTarget();
                }

                else
                {
                    pingingTask = null;
                }
            }

            if (pingingTask != null)
            {
                var completedTask = await Task.WhenAny(pingingTask);

                // if job is done, reset _currentJobRunner and the pinging target
                if (completedTask.IsCompleted)
                {
                    _logger.LogDebug($"""
                                                PingMonitoringJobManager: PingingJob of target {pingingTarget}
                                                has completed
                                              """);
                    ResetJobRunner();
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
        return _currentJobRunner != null;
    }
    
    public void StartPinging(string target)
    {
        lock (lockingObject)
        {
            // simply do nothing if the target is already being pinged
            if (_currentJobRunner != null)
            {
                return;
            }

            _logger.LogDebug("PingMonitoringJobManager: StartPinging: Entered");
            _currentJobRunner = _getPingMonitoringJobRunner(target);
            _currentJobRunner.StartPinging();
        }
    }

    public void StopPinging()
    {
        _logger.LogDebug("PingMonitoringJobManager: StopPinging: Entered");
        lock (lockingObject)
        {
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
            if (_currentJobRunner != null && !_currentJobRunner.CancellationTokenSourceIsNull())
            {
                _currentJobRunner.Dispose();
                _currentJobRunner = null;
            }
        }
    }

    public bool IsPingAlreadyRunning(){
        lock (lockingObject)
        {
            return _currentJobRunner != null;
        }
    }
}
    