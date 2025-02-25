namespace PingBoard.Services;

using PingBoard.Database.Utilities;
using PingBoard.Probes;
using PingBoard.Services;
using Probes.NetworkProbes.Common;
using ServerEvents;

/// A class that implements INetworkProbeBase (sic) can combine as many other low level probes (raw networking functionality)
/// as it needs to be operational, and will be presented to a NetworkProbe as a single unit. A full-fledged NetworkProbe
/// bundles utilities needed to present the manager with a simple API for starting, stopping and viewing the status
/// of ongoing probes.
public class NetworkProbeLiaison : IDisposable
{
    public record Configuration
    {
        public INetworkProbeBase BaseNetworkProbe { get; init; }
        public CancellationTokenSource CancellationTokenSource { get; init; }
        public CrudOperations CrudOperations { get; init; }
        public ServerEventEmitter ServerEventEmitter { get; init; }
        public IProbeBehavior ProbeBehavior { get; init; }
        public IProbeThresholds ProbeThresholds { get; init; }
        public ProbeSchedule ProbeSchedule { get; init; }
        public ProbeScheduler ProbeScheduler { get; init; }
        public ILogger<NetworkProbeLiaison> Logger { get; init; }
    }

    private readonly INetworkProbeBase _probe; // need an INetworkProbeBase factory
    private readonly CancellationTokenSource _cancellationTokenSource;
    private readonly CrudOperations _crudOperations;
    private readonly ServerEventEmitter _serverEventEmitter;
    private readonly IProbeBehavior _probeBehavior;
    private readonly IProbeThresholds _probeThresholds;
    private readonly ProbeSchedule _probeSchedule;
    private readonly ProbeScheduler _probeScheduler;
    private readonly ILogger<NetworkProbeLiaison> _logger;

    // private ProbeStrategy _probeStrategy;
    private Task _probeTask;

    public NetworkProbeLiaison(Configuration configuration)
    {
        _probe = configuration.BaseNetworkProbe;
        _crudOperations = configuration.CrudOperations;
        _cancellationTokenSource = configuration.CancellationTokenSource;
        _serverEventEmitter = configuration.ServerEventEmitter;
        _probeBehavior = configuration.ProbeBehavior;
        _probeThresholds = configuration.ProbeThresholds;
        _probeSchedule = configuration.ProbeSchedule;
        _probeScheduler = configuration.ProbeScheduler;
        _logger = configuration.Logger;
    }

    public async Task StopProbingAsync()
    {
        _cancellationTokenSource.Cancel();
    }

    public void StartProbingAsync()
    {
        _logger.LogTrace(
            $"NetworkProbeLiaison with probe type {_probe.GetType()}: Entered StartProbingAsync"
        );
        _logger.LogInformation("(9) NetworkProbeLiaison: StartProbing: About to DoProbingAsync()");
        _probeTask = DoProbingAsync().ContinueWith(AfterProbingAsync);
    }

    private async Task AfterProbingAsync(Task t)
    {
        // The possible TaskStatus enums are Created (0), WaitingForActivation (1), WaitingToRun (2),
        // Running (3), WaitingForChildrenToComplete (4), RanToCompletion (5), Canceled (6) or Faulted (6)
        // Since this function is only invoked after probing is done (see StartProbingAsync()), the only three
        // possible states to check are RanToCompletion, Canceled and Faulted.
        _logger.LogInformation("(13) NetworkProbeLiaison: AfterProbingAsync() entered");
        if (t.IsCanceled)
        {
            _logger.LogInformation(
                "(14) NetworkProbeLiaison: AfterProbingAsync(): task state is canceled"
            );
            _logger.LogDebug(
                "Probing of {target.ToString()} was canceled",
                _probeBehavior.GetTarget()
            );
        }

        if (t.IsFaulted)
        {
            _logger.LogInformation(
                "(14) NetworkProbeLiaison: AfterProbingAsync(): task state is faulted"
            );

            _serverEventEmitter.IndicateServerEvent(
                new ProbeError(
                    _probe.ProbeId,
                    _probe.GetName(),
                    _probeBehavior.GetTarget(),
                    t.Exception.Message
                ),
                "NetworkProbeLiaison: AfterProbingAsync"
            );
            _logger.LogCritical(
                "An exception occured while probing {target.ToString()} Exception: {exception}",
                _probeBehavior.GetTarget(),
                t.Exception
            );
        }
        else // RanToCompletion
        {
            _logger.LogDebug("NetworkProbeLiaison ran to completion");
        }

        _serverEventEmitter.IndicateServerEvent(
            new ProbeStatus(_probe.ProbeId, "off", _probeBehavior.GetTarget(), _probe.GetName()),
            "NetworkProbeLiaison: AfterProbingAsync"
        );
    }

    private async Task DoProbingAsync()
    {
        //logging here
        _logger.LogTrace(
            $"NetworkProbeLiaison with probe type {_probe.GetType()}: Entered DoProbingAsync"
        );
        _logger.LogInformation(
            $"(10) NetworkProbeLiaison: Entered DoProbingAsync with probe type {_probe.GetName()}"
        );
        var token = _cancellationTokenSource.Token;
        var result = _probe.NewResult();
        var onEventReported = false;

        //emit server event, OnOffToggle
        while (!token.IsCancellationRequested && _probe.ShouldContinue(result))
        {
            //_probeScheduler.StartIntervalTracking();
            _logger.LogInformation(
                "(11) NetworkProbeLiaison: DoProbingAsync, about to call the probe"
            );
            result = await _probe.ProbeAsync(_probeBehavior, token);
            _logger.LogInformation(
                $"(12) NetworkProbeLiaison: DoProbingAsync: result: {(result is null ? "null" : result.ToString())}"
            );
            //hardcode event type for now

            if (!onEventReported)
            {
                _serverEventEmitter.IndicateServerEvent(
                    new ProbeStatus(
                        _probe.ProbeId,
                        "on",
                        _probeBehavior.GetTarget(),
                        _probe.GetName()
                    ),
                    "NetworkProbeLiaison: DoProbingAsync"
                );
            }

            await _crudOperations.InsertProbeResult(result, token);
            _serverEventEmitter.IndicateServerEvent(
                new ProbeInfo(_probe.ProbeId, _probeBehavior.GetTarget(), _probe.GetName()),
                "NetworkProbeLiaison: DoProbingAsync"
            );

            if (_probe.IsAnomaly(result, _probeThresholds))
            {
                _serverEventEmitter.IndicateServerEvent(
                    new ProbeAnomaly(_probe.ProbeId, _probeBehavior.GetTarget(), _probe.GetName()),
                    "NetworkProbeLiaison: DoProbingAsync"
                );
            }

            //_probeScheduler.DelayProbingAsync();
            await Task.Delay(1000, token);
        }
    }

    // https://learn.microsoft.com/en-us/dotnet/api/System.Threading.Tasks.TaskStatus?view=net-9.0
    public TaskStatus GetProbingStatus()
    {
        return _probeTask.Status;
    }

    public INetworkProbeTarget GetTarget()
    {
        return _probeBehavior.Target;
    }

    public void Dispose()
    {
        _cancellationTokenSource.Dispose();
    }
}
