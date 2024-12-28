using Microsoft.EntityFrameworkCore;
using PingBoard.Pinging;
using PingBoard.Probes.NetworkProbes;

namespace PingBoard.Services;

using System.Diagnostics;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Serilog;

/// <summary>
/// Handles the logistics of probing by creating, destroying, and directing PingMonitoringJobRunners
/// in response to requests passed from the front end. <see cref="GroupPingMonitoringJobRunner"/>
/// </summary>
public class ProbeOperationsCenter : BackgroundService
{
    private readonly Func<
        string,
        IProbeInvocationParams,
        INetworkProbeTarget,
        NetworkProbeLiason
    > _probeLiasonFactory;
    private ServerEventEmitter _serverEventEmitter;
    private readonly ILogger<ProbeOperationsCenter> _logger;
    private volatile NetworkProbeLiason? _currentLiason;
    private int _checkRunningJobsDelayMs = 100;
    private readonly object _lockingObject = new object();

    // private Func<INetworkTarget, INetworkProbe, NetworkProbeLiason> _probeLiasonFactory;


    public ProbeOperationsCenter(
        Func<
            string,
            IProbeInvocationParams,
            INetworkProbeTarget,
            NetworkProbeLiason
        > probeLiasonFactory,
        ServerEventEmitter serverEventEmitter,
        ILogger<ProbeOperationsCenter> logger
    )
    {
        _probeLiasonFactory = probeLiasonFactory;
        _serverEventEmitter = serverEventEmitter;
        _logger = logger;
    }

    /// <summary>
    /// The continuous loop that monitors the Pinging job to see if any administrative action is necessary,
    /// such as getting rid of the PingMonitoringJobRunner, informing the front end that its no longer probing, etc.
    /// </summary>
    /// <param name="stoppingToken"></param>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogDebug("ProbeOperationsCenter: ExecuteAsync: Entered");
        while (!stoppingToken.IsCancellationRequested)
        {
            if (_currentLiason is not null)
            {
                lock (_lockingObject)
                {
                    var target = _currentLiason.GetTarget();
                    var currentStatus = _currentLiason.GetProbingStatus();
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
                        ResetLiason();
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
            return _currentLiason != null;
        }
    }

    /// <summary>
    /// When appropriate, it synchronously begins probing
    /// </summary>
    /// <param name="target">
    /// The INetworkProbeTarget object deserialized from the user, representing the subject of the probing operation.
    /// </param>
    /// <param name="probeParams">
    /// Parametric values entered by the user which will govern the probing operation
    /// </param>
    /// <param name="target"> </param>
    public void StartProbing(
        string probeOperation,
        IProbeInvocationParams probeParams,
        INetworkProbeTarget target
    )
    {
        lock (_lockingObject)
        {
            _logger.LogDebug($"ProbeOperationsCenter: StartPinging: Entered with target:{target}");
            // simply do nothing if the target is already being pinged
            if (IsProbingActive())
            {
                _logger.LogDebug($"ProbeOperationsCenter: StartPinging: Already probing");
                return;
            }

            _currentLiason = _probeLiasonFactory(probeOperation, probeParams, target);
            _logger.LogDebug($"ProbeOperationsCenter: Probing: new liason created");
            _currentLiason.StartProbingAsync();
        }
    }

    /// <summary>
    /// Directs the NetworkProbeLiason to stop probing by invoking its StopProbingAsync() function
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
            await _currentLiason.StopProbingAsync();
            ResetLiason();
        }
    }

    /// <summary>
    /// Safely resets the current ProbeLiason to null and disposes of the old one,
    /// used as a cleanup function when the probing is stopped by the user, halted for any reason, or
    /// cancelled using a cancellation token.
    /// </summary>
    public void ResetLiason()
    {
        lock (_lockingObject)
        {
            _logger.LogDebug("ProbeOperationsCenter: ResetJobRunner: Entered");

            if (IsProbingActive())
            {
                _logger.LogDebug(
                    "ProbeOperationsCenter: ResetJobRunner: Going to dispose of job runner"
                );
                var npl = _currentLiason;
                _currentLiason = null; // defensive measure to prevent intermediate state of disposed job runner, but non-null currentJobRunner
                npl.Dispose();
            }
        }
    }
}
