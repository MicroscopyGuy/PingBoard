namespace PingBoard.Services;

public interface INetworkProbeBase
{
    public string Name { get; }
    List<Type> SupportedTargetTypes { get; }
    Task<ProbeResult> ProbeAsync(INetworkProbeTarget probeTarget, CancellationToken cancellationToken);
    bool ShouldContinue(ProbeResult result);
}

