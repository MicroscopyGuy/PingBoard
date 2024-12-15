namespace PingBoard.Probes.NetworkProbes;

public interface INetworkProbeBase<TStatusChange, TErrorResult, TProbeResult> 
    where TErrorResult : ErrorResult
    where TProbeResult : ProbeResult
{
    public string Name { get; }
    List<Type> SupportedTargetTypes { get; }
    Task<TProbeResult> ProbeAsync(INetworkProbeTarget probeTarget, CancellationToken cancellationToken);
    bool ShouldContinue(TProbeResult result);
}

 