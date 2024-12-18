namespace PingBoard.Probes.NetworkProbes;

using PingBoard.Database.Utilities;
using PingBoard.Services;

/// A class that implements INetworkProbeBase (sic) can combine as many other low level probes (raw networking functionality)
/// as it needs to be operational, and will be presented to a NetworkProbe as a single unit. A full-fledged NetworkProbe
/// bundles utilities needed to present the manager with a simple API for starting, stopping and viewing the status
/// of ongoing probes.
public class NetworkProbeLiason
{
    private INetworkProbeBase _baseNetworkProbe; // need an INetworkProbeBase factory
    private CancellationTokenSource _cancellationTokenSource;
    private CrudOperations _crudOperations;
    private ServerEventEmitter _serverEventEmitter;
    private INetworkProbeTarget _networkProbeTarget;

    // private ProbeStrategy _probeStrategy;
    // private ProbeScheduler _probeScheduler;
    private ILogger<NetworkProbeLiason> _logger;
    private Task _probeTask;

    public NetworkProbeLiason(
        INetworkProbeBase baseNetworkProbe,
        CrudOperations crudOperations,
        CancellationTokenSource cancellationTokenSource,
        ServerEventEmitter serverEventEmitter,
        INetworkProbeTarget networkProbeTarget,
        ILogger<NetworkProbeLiason> logger
    )
    {
        _baseNetworkProbe = baseNetworkProbe;
        _crudOperations = crudOperations;
        _cancellationTokenSource = cancellationTokenSource;
        _serverEventEmitter = serverEventEmitter;
        _networkProbeTarget = networkProbeTarget;
        //_probeStrategy = probeStrategy;
        //_probeScheduler = probeScheduler;
        _logger = logger;
    }

    public async Task StopProbingAsync()
    {
        _cancellationTokenSource.Cancel();
    }

    // need to find the return type somehow of the result
    // how does the database handle the new result types?
    // what if one requires a foreign key? Need to somehow give the data and the type to CrudOperations?
    // What ServerEvent to emit?
    public async Task StartProbingAsync()
    {
        _logger.LogTrace(
            $"NetworkProbeLiason with probe type {_baseNetworkProbe.GetType()}: Entered StartProbingAsync"
        );
        _probeTask = DoProbingAsync();
    }

    private async Task DoProbingAsync()
    {
        //logging here
        _logger.LogTrace(
            $"NetworkProbeLiason with probe type {_baseNetworkProbe.GetType()}: Entered DoProbingAsync"
        );
        var token = _cancellationTokenSource.Token;
        var result = ProbeResult.Default();

        while (!token.IsCancellationRequested && _baseNetworkProbe.ShouldContinue(result))
        {
            //_probeScheduler.StartIntervalTracking();
            result = await _baseNetworkProbe.ProbeAsync(_networkProbeTarget, token);
            await _crudOperations.InsertProbeResult(result, token);
            //_probeScheduler.DelayProbingAsync();

            // emit a server event? How is this handled?
            await Task.Delay(100, token);
        }
    }

    public TaskStatus GetProbingStatus()
    {
        return _probeTask.Status;
    }
}
