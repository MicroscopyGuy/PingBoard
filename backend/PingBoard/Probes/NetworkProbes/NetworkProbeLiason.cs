namespace PingBoard.Probes.NetworkProbes;

using Microsoft.AspNetCore.Components.Infrastructure;
using PingBoard.Database.Utilities;
using PingBoard.Services;

/// A class that implements INetworkProbeBase (sic) can combine as many other low level probes (raw networking functionality)
/// as it needs to be operational, and will be presented to a NetworkProbe as a single unit. A full-fledged NetworkProbe
/// bundles utilities needed to present the manager with a simple API for starting, stopping and viewing the status
/// of ongoing probes.
public class NetworkProbeLiason : IDisposable
{
    private INetworkProbeBase _baseNetworkProbe; // need an INetworkProbeBase factory
    private CancellationTokenSource _cancellationTokenSource;
    private CrudOperations _crudOperations;
    private ServerEventEmitter _serverEventEmitter;
    private INetworkProbeTarget _networkProbeTarget;
    private IProbeInvocationParams _probeInvocationParams;
    private ProbeScheduler _probeScheduler;

    //_probeInvocationThresholds = probeInvocationThresholds;
    // private ProbeStrategy _probeStrategy;

    private ILogger<NetworkProbeLiason> _logger;
    private Task _probeTask;

    public NetworkProbeLiason(
        INetworkProbeBase baseNetworkProbe,
        CrudOperations crudOperations,
        CancellationTokenSource cancellationTokenSource,
        ServerEventEmitter serverEventEmitter,
        IProbeInvocationParams probeInvocationParams,
        INetworkProbeTarget networkProbeTarget,
        ProbeScheduler probeScheduler,
        ILogger<NetworkProbeLiason> logger
    )
    {
        _baseNetworkProbe = baseNetworkProbe;
        _crudOperations = crudOperations;
        _cancellationTokenSource = cancellationTokenSource;
        _serverEventEmitter = serverEventEmitter;
        _networkProbeTarget = networkProbeTarget;
        _probeInvocationParams = probeInvocationParams;
        _probeScheduler = probeScheduler;
        _logger = logger;

        //_probeInvocationSchedule = probeInvocationSchedule;
        //_probeInvocationThresholds = probeInvocationThresholds;
        //_probeStrategy = probeStrategy;
    }

    public async Task StopProbingAsync()
    {
        _cancellationTokenSource.Cancel();
        // emit server event: OnOffToggle
    }

    // What ServerEvent to emit?

    public void StartProbingAsync()
    {
        _logger.LogTrace(
            $"NetworkProbeLiason with probe type {_baseNetworkProbe.GetType()}: Entered StartProbingAsync"
        );
        _probeTask = DoProbingAsync().ContinueWith(AfterProbingAsync);
    }

    // csharpier-ignore-start
    private async Task AfterProbingAsync(Task t)
    {
        if (!t.IsFaulted)
        {
            return;
        }
        if (!t.IsCanceled)
        {
            _logger.LogCritical("An exception occured while probing {target} Exception: {exception}",
                _probeInvocationParams.GetTarget(), t.Exception);
        }
        else
        {
            _logger.LogDebug("Probing of {target} was canceled", _probeInvocationParams.GetTarget());
        }
    }
    // csharpier-ignore-end

    private async Task DoProbingAsync()
    {
        //logging here
        _logger.LogTrace(
            $"NetworkProbeLiason with probe type {_baseNetworkProbe.GetType()}: Entered DoProbingAsync"
        );
        var token = _cancellationTokenSource.Token;
        var result = ProbeResult.Default();

        //emit server event, OnOffToggle
        while (!token.IsCancellationRequested && _baseNetworkProbe.ShouldContinue(result))
        {
            //_probeScheduler.StartIntervalTracking();
            result = await _baseNetworkProbe.ProbeAsync(_probeInvocationParams, token);
            await _crudOperations.InsertProbeResult(result, token);
            //emit server event, Info

            //_probeScheduler.DelayProbingAsync();
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
        return _networkProbeTarget;
    }

    public void Dispose()
    {
        _cancellationTokenSource.Dispose();
    }
}
