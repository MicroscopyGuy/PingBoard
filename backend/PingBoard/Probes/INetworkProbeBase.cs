namespace PingBoard.Services;

public interface INetworkProbeBase
{
    string Name { get; set; }
    List<Type> SupportedTargetTypes { get; set; }
    Task<ProbeResult> ProbeAsync(PingTarget probeTarget, CancellationToken cancellationToken);
    bool ShouldContinue(ProbeResult result);
}

