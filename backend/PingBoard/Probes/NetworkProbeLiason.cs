using PingBoard.Database.Utilities;

namespace PingBoard.Services;

/// A class that implements INetworkProbeBase (sic) can combine as many other low level probes (raw networking functionality)
/// as it needs to be operational, and will be presented to a NetworkProbe as a single unit. A full-fledged NetworkProbe
/// bundles utilities needed to present the manager with a simple API for starting, stopping and viewing the status
/// of ongoing probes. 
public class NetworkProbeLiason
{
    private INetworkProbe _baseNetworkProbe;
    private CancellationTokenSource _cancellationTokenSource;
    private CrudOperations _crudOperations;
    private ServerEventEmitter _serverEventEmitter;
    private ILogger<NetworkProbeLiason> _logger;
    
    public NetworkProbeLiason(INetworkProbe baseNetworkProbe, CrudOperations crudOperations, 
        CancellationTokenSource cancellationTokenSource, ServerEventEmitter serverEventEmitter, 
        ILogger<NetworkProbeLiason> logger)
    {
        _baseNetworkProbe = baseNetworkProbe;
        _crudOperations = crudOperations;
        _cancellationTokenSource = cancellationTokenSource;
        _serverEventEmitter = serverEventEmitter;
        _logger = logger;
    }

    // need to find the return type somehow of the result
    // how does the database handle the new result types?
    // what if one requires a foreign key? Need to somehow give the data and the type to CrudOperations?
    // What ServerEvent to emit?
    public async Task StartProbing()
    {
        _logger.LogDebug("<ClassName>: Entered StartProbing");
        var token = _cancellationTokenSource.Token;
        var result = ProbeResult.Default();
        
        while (!token.IsCancellationRequested && _baseNetworkProbe.ShouldContinue(result))
        {
            // 
            result = await _baseNetworkProbe.ProbeAsync(token);
            await _crudOperations.InsertProbeResult(result, token);
            // emit a server event? How is this handled?
        }

        return;
    }
    
}