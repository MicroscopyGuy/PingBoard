namespace PingBoard.Probes.NetworkProbes;

using Common;
using PingBoard.Database.Utilities;
using PingBoard.Services;

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
        public IProbeSchedule ProbeSchedule { get; init; }
        public ProbeScheduler ProbeScheduler { get; init; }
        public ILogger<NetworkProbeLiaison> Logger { get; init; }
    }

    private readonly INetworkProbeBase _baseNetworkProbe; // need an INetworkProbeBase factory
    private readonly CancellationTokenSource _cancellationTokenSource;
    private readonly CrudOperations _crudOperations;
    private readonly ServerEventEmitter _serverEventEmitter;
    private readonly IProbeBehavior _probeBehavior;
    private readonly IProbeThresholds _probeThresholds;
    private readonly IProbeSchedule _probeSchedule;
    private readonly ProbeScheduler _probeScheduler;
    private readonly ILogger<NetworkProbeLiaison> _logger;

    // private ProbeStrategy _probeStrategy;
    private Task _probeTask;

    public NetworkProbeLiaison(Configuration configuration)
    {
        _baseNetworkProbe = configuration.BaseNetworkProbe;
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
            $"NetworkProbeLiaison with probe type {_baseNetworkProbe.GetType()}: Entered StartProbingAsync"
        );
        _probeTask = DoProbingAsync().ContinueWith(AfterProbingAsync);
    }

    private async Task AfterProbingAsync(Task t)
    {
        if (!t.IsFaulted)
        {
            return;
        }
        if (!t.IsCanceled)
        {
            _logger.LogCritical(
                "An exception occured while probing {target} Exception: {exception}",
                _probeBehavior.GetTarget(),
                t.Exception
            );
            _serverEventEmitter.IndicatePingAgentError(
                _probeBehavior.GetTarget(),
                t.Exception.ToString()
            );
        }
        else
        {
            _logger.LogDebug("Probing of {target} was canceled", _probeBehavior.GetTarget());
            _serverEventEmitter.IndicatePingOnOffToggle(
                _probeBehavior.GetTarget(),
                false,
                "NetworkProbeLiaison: AfterProbingAsync"
            ); // hardcode for now
        }
    }

    private async Task DoProbingAsync()
    {
        //logging here
        _logger.LogTrace(
            $"NetworkProbeLiaison with probe type {_baseNetworkProbe.GetType()}: Entered DoProbingAsync"
        );
        var token = _cancellationTokenSource.Token;
        var result = new ProbeResult();

        //emit server event, OnOffToggle
        while (!token.IsCancellationRequested && _baseNetworkProbe.ShouldContinue(result))
        {
            //_probeScheduler.StartIntervalTracking();
            result = await _baseNetworkProbe.ProbeAsync(_probeBehavior, token);
            //hardcode event type for now
            _serverEventEmitter.IndicatePingOnOffToggle(
                _probeBehavior.GetTarget(),
                false,
                "NetworkProbeLiaison: DoProbingAsync"
            );

            await _crudOperations.InsertProbeResult(result, token);
            _serverEventEmitter.IndicatePingInfo(
                _probeBehavior.GetTarget(),
                "NetworkProbeLiaison: DoProbingAsync"
            );

            //_probeScheduler.DelayProbingAsync();
            await _crudOperations.InsertProbeResult(result, token);
            await Task.Delay(100, token);
        }
    }

    //https://learn.microsoft.com/en-us/dotnet/api/System.Threading.Tasks.TaskStatus?view=net-9.0
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
