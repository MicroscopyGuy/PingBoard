namespace PingBoard.Probes.NetworkProbes.Common;

public record ProbeConfigAggregate(
    IProbeBehavior Behavior,
    IProbeThresholds Thresholds,
    ProbeSchedule Schedule
);
