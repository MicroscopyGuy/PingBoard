namespace PingBoard.Probes.NetworkProbes;

using Common;
using Probes.Common;

public interface INetworkProbeBase<TProbeBehavior, TProbeThresholds, TProbeResult>
    where TProbeResult : ProbeResult
    where TProbeBehavior : IProbeBehavior
    where TProbeThresholds : IProbeThresholds
{
    Task<TProbeResult> ProbeAsync(TProbeBehavior behavior, CancellationToken cancellationToken);
    bool ShouldContinue(TProbeResult result);
}

public interface INetworkProbeBase
{
    public static abstract string Name { get; }
    Task<ProbeResult> ProbeAsync(IProbeBehavior probeBehavior, CancellationToken cancellationToken);
    bool ShouldContinue(ProbeResult result);

    bool IsAnomaly(ProbeResult result, IProbeThresholds thresholds);
}
