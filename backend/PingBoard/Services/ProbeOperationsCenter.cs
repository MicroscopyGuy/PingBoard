namespace PingBoard.Services;

using PingBoard.Probes.NetworkProbes;
using Probes.NetworkProbes.Common;

/// <summary>
/// Handles the logistics of probing by creating, destroying, and directing NetworkProbeLiasons
/// in response to requests passed from the front end. <see cref="NetworkProbeLiaison"/>
/// </summary>
public class ProbeOperationsCenter : BackgroundService
{
    private readonly Func<ProbeConfigAggregate, NetworkProbeLiaison> _probeLiaisonFactory;
    private readonly ILogger<ProbeOperationsCenter> _logger;
    private volatile NetworkProbeLiaison? _currentLiaison;
    private int _checkRunningJobsDelayMs = 10;
    private readonly object _lockingObject = new object();

    public ProbeOperationsCenter(
        Func<ProbeConfigAggregate, NetworkProbeLiaison> probeLiaisonFactory,
        ILogger<ProbeOperationsCenter> logger
    )
    {
        _probeLiaisonFactory = probeLiaisonFactory;
        _logger = logger;
    }

    /// <summary>
    /// The continuous loop that monitors the NetworkProbeLiaison to see if it is done.
    /// </summary>
    /// <param name="stoppingToken"></param>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogDebug("ProbeOperationsCenter: ExecuteAsync: Entered");
        while (!stoppingToken.IsCancellationRequested)
        {
            if (_currentLiaison is not null)
            {
                lock (_lockingObject)
                {
                    var target = _currentLiaison.GetTarget();
                    var currentStatus = _currentLiaison.GetProbingStatus();
                    var resetStatuses = new TaskStatus[3]
                    {
                        TaskStatus.RanToCompletion,
                        TaskStatus.Canceled,
                        TaskStatus.Faulted,
                    };

                    if (resetStatuses.Contains(currentStatus))
                    {
                        _logger.LogTrace(
                            $"ProbeOperationsCenter: ExecuteAsync: status of probing of target {target}: {Enum.GetName(currentStatus)}"
                        );
                        _logger.LogInformation(
                            "ProbeOperationsCenter: ExecuteAsync: About to reset liaison"
                        );
                        ResetLiaison();
                    }
                }
            }

            await Task.Delay(_checkRunningJobsDelayMs);
        }
    }

    /// <summary>
    /// Returns whether any jobs are running.
    /// </summary>
    /// <returns>true if a job is running, and false otherwise</returns>
    public bool IsProbingActive()
    {
        //_logger.LogDebug("ProbeOperationsCenter: IsProbingActive: Entered");
        lock (_lockingObject)
        {
            return _currentLiaison != null;
        }
    }

    /// <summary>
    /// When appropriate, it synchronously begins probing
    /// </summary>
    ///
    /// <param name="probeOperation">
    /// The type of probing operation selected by the user
    /// </param>
    ///
    /// <param name="probeConfig">
    /// An object which wraps one of each of the following: IProbeBehavior, IProbeThresholds, and ProbeSchedule.
    /// Used for more convenient handling of these types in several areas where all three are required.
    /// </param>
    public void StartProbing(ProbeConfigAggregate probeConfig)
    {
        _logger.LogInformation("(3) ProbeOperationsCenter: StartProbing hit");

        lock (_lockingObject)
        {
            _logger.LogDebug(
                $"ProbeOperationsCenter: StartProbing: Entered with target:{probeConfig.Behavior.Target}"
            );
            // simply do nothing if the target is already being probed
            _logger.LogInformation("(4) ProbeOperationsCenter: StartProbing hit");
            if (IsProbingActive())
            {
                _logger.LogDebug($"ProbeOperationsCenter: StartPinging: Already probing");
                _logger.LogInformation(
                    "(5) ProbeOperationsCenter: StartProbing: IsProbingActive check"
                );
                return;
            }

            _logger.LogInformation(
                "(6) ProbeOperationsCenter: StartProbing: About to get Liaison object"
            );
            _currentLiaison = _probeLiaisonFactory(probeConfig);
            _logger.LogDebug($"ProbeOperationsCenter: Probing: new liaison created");
            _logger.LogInformation(
                "(7) ProbeOperationsCenter: StartProbing: Liaison object created"
            );
            _currentLiaison.StartProbingAsync();
            _logger.LogInformation(
                "(8) ProbeOperationsCenter: StartProbing: Liaison started probing"
            );
        }
    }

    /// <summary>
    /// Directs the NetworkProbeLiaison to stop probing by invoking its StopProbingAsync() function
    /// </summary>
    public async Task StopProbingAsync()
    {
        var safeToStop = false;
        lock (_lockingObject)
        {
            _logger.LogDebug("ProbeOperationsCenter: StopProbingAsync: Entered");
            if (IsProbingActive())
            {
                safeToStop = true;
            }
        }

        if (safeToStop)
        {
            await _currentLiaison.StopProbingAsync();
            ResetLiaison();
        }
    }

    /// <summary>
    /// Safely resets the current ProbeLiaison to null and disposes of the old one,
    /// used as a cleanup function when the probing is stopped by the user, halted for any reason, or
    /// cancelled using a cancellation token.
    /// </summary>
    public void ResetLiaison()
    {
        lock (_lockingObject)
        {
            _logger.LogDebug("ProbeOperationsCenter: ResetJobRunner: Entered");

            if (IsProbingActive())
            {
                _logger.LogDebug(
                    "ProbeOperationsCenter: ResetLiaison: Going to dispose of NetworkProbeLiaison"
                );
                var npl = _currentLiaison;
                _currentLiaison = null; // defensive measure to prevent intermediate state of disposed Liaison, but non-null _currentLiaison
                npl.Dispose();
            }
        }
    }
}
