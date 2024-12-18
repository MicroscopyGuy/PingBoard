namespace PingBoard.Probes.NetworkProbes;

public interface INetworkProbeBase<TProbeConfiguration, TStatusChange, TProbeResult>
    where TProbeResult : ProbeResult
    where TStatusChange : ProbeStatusChange
    where TProbeConfiguration : IProbeInvocationParams
{
    public string Name { get; }
    List<Type> SupportedTargetTypes { get; }
    Task<TProbeResult> ProbeAsync(
        TProbeConfiguration probeTarget,
        CancellationToken cancellationToken
    );
    bool ShouldContinue(TProbeResult result);
}

public interface INetworkProbeBase
{
    Task<ProbeResult> ProbeAsync(
        IProbeInvocationParams probeTarget,
        CancellationToken cancellationToken
    );
    bool ShouldContinue(ProbeResult result);
}
