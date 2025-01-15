namespace PingBoard.Probes.NetworkProbes.Common;

public interface INetworkProbeBase<TProbeBehavior, TProbeThresholds, TProbeResult>
    where TProbeResult : ProbeResult
    where TProbeBehavior : IProbeBehavior
    where TProbeThresholds : IProbeThresholds
{
    Task<TProbeResult> ProbeAsync(TProbeBehavior behavior, CancellationToken cancellationToken);
    bool ShouldContinue(TProbeResult result);
    TProbeResult NewResult();
}

public interface INetworkProbeBase
{
    public static abstract string Name { get; }
    Task<ProbeResult> ProbeAsync(IProbeBehavior probeBehavior, CancellationToken cancellationToken);
    bool ShouldContinue(ProbeResult result);
    bool IsAnomaly(ProbeResult result, IProbeThresholds thresholds);
    string GetName();
    ProbeResult NewResult();
}
