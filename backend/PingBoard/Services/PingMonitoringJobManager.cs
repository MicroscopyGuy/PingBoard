using Microsoft.EntityFrameworkCore;
using PingBoard.Pinging;

namespace PingBoard.Services;

/// <summary>
/// Handles the logistics of pinging by creating, destroying, and directing PingMonitoringJobRunners
/// in response to requests passed from the front end. <see cref="GroupPingMonitoringJobRunner"/>
/// </summary>
public class PingMonitoringJobManager : BackgroundService
{
    private readonly Func<string, NetworkProbeLiason> _getPingMonitoringJobRunner;
    private readonly ILogger<PingMonitoringJobManager> _logger;
    private ServerEventEmitter _serverEventEmitter;
    private volatile NetworkProbeLiason? _currentJobRunner;
    private int _checkRunningJobsDelayMs = 100;
    private readonly object _lockingObject = new object();
    // private Func<INetworkTarget, INetworkProbe, NetworkProbeLiason> _probeLiasonFactory;
    
    
    public PingMonitoringJobManager(Func<string, NetworkProbeLiason> pingMonitoringJobRunnerSource,
        ServerEventEmitter serverEventEmitter, ILogger<PingMonitoringJobManager> logger)
    {
        _getPingMonitoringJobRunner = pingMonitoringJobRunnerSource;
        _serverEventEmitter = serverEventEmitter;
        _logger = logger;
    }
    
    
    /// <summary>
    /// The continuous loop that monitors the Pinging job to see if any administrative action is necessary,
    /// such as getting rid of the PingMonitoringJobRunner, informing the front end that its no longer pinging, etc.
    /// </summary>
    /// <param name="stoppingToken"></param>
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
                    //pingingTask = _currentJobRunner!.GetPingingTask();
                    //pingingTarget = _currentJobRunner.GetTarget();
                }

                else {
                    pingingTask = null;
                }
            }

            /*
            if (IsPinging())
            {
               //logger.LogDebug($"PingMonitoringJobManager: ExecuteAsync: Currently pinging {pingingTarget}");
                var completedTask = await Task.WhenAny(pingingTask);

                // if job is done, reset _currentJobRunner and the pinging target
                // For overview of task properties:
                // https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task?view=net-8.0
                lock(_lockingObject){
                    if (completedTask.IsCompleted)
                    {
                        _logger.LogDebug($"PingMonitoringJobManager: ExecuteAsync: PingingJob of target {pingingTarget} has completed");
                        ResetJobRunner(); // keep reset just in case, since reset checks to see if it's safe, first
                        _serverEventEmitter.IndicatePingOnOffToggle(pingingTarget, false, "ExecuteAsync");
                    }
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
            }*/
        }
    } 

    /// <summary>
    /// Returns whether any jobs are running.
    /// </summary>
    /// <returns>true if a job is running, and false otherwise</returns>
    public bool IsPinging()
    {
        //_logger.LogDebug("PingMonitoringJobManager: IsPinging: Entered");
        lock (_lockingObject)
        {
            return _currentJobRunner != null;
        }
        
    }
    
    /// <summary>
    /// When appropriate, it synchronously begins pinging and informs the UI through the ServerEventEmitter that the
    /// pinging has been toggled to "on."
    /// </summary>
    /// <param name="target"></param>
    public void StartPinging(string target)
    {
        lock (_lockingObject)
        {
            _logger.LogDebug($"PingMonitoringJobManager: StartPinging: Entered with target:{target}"); 
            // simply do nothing if the target is already being pinged
            if (IsPinging())
            {
                _logger.LogDebug($"PingMonitoringJobManager: StartPinging: Already pinging"); 
                return;
            }
            
            _currentJobRunner = _getPingMonitoringJobRunner(target);
            //_logger.LogDebug($"PingMonitoringJobManager: StartPinging: new job runner created:{_currentJobRunner.GetHashCode()}");
            //_currentJobRunner.StartPinging();
            //_serverEventEmitter.IndicatePingOnOffToggle(_currentJobRunner.GetTarget(), true, "StartPinging");
        }
    }

    /// <summary>
    /// When appropriate, it asynchronously stops pinging and informs the UI through the ServerEventEmitter that the
    /// pinging has been toggled to "off."
    /// </summary>
    public async Task StopPingingAsync()
    {
        var safeToStop = false;
        lock (_lockingObject)
        {
            _logger.LogDebug("PingMonitoringJobManager: StopPinging: Entered");
            if (IsPinging())
            {
                safeToStop = true;
            }
        }

        if (safeToStop)
        {
            // save before getting rid of the job runner, so UI knows the target that is no longer being pinged
            //var oldTarget = _currentJobRunner.GetTarget();
            //await _currentJobRunner.CancelTokenSourceAsync();
            ResetJobRunner();
            //_serverEventEmitter.IndicatePingOnOffToggle(oldTarget, false, "StopPingingAsync");
        }

    }

    /// <summary>
    /// Safely resets the current PingMonitoringJobRunner to null and disposes of the old one,
    /// used as a cleanup function when the pinging is stopped by the user, halted for any reason, or
    /// cancelled using a cancellation token.
    /// </summary>
    public void ResetJobRunner()
    {
        lock (_lockingObject)
        {
            _logger.LogDebug("PingMonitoringJobManager: ResetJobRunner: Entered");
            
            if (IsPinging())
            {
                _logger.LogDebug("PingMonitoringJobManager: ResetJobRunner: Going to dispose of job runner");
                var cjr = _currentJobRunner;
                _currentJobRunner = null; // defensive measure to prevent intermediate state of disposed job runner, but non-null currentJobRunner
                //cjr.Dispose();

            }
        }
    }

    
}
    