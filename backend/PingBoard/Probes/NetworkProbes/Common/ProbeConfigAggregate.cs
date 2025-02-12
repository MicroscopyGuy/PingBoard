namespace PingBoard.Probes.NetworkProbes.Common;

public record ProbeConfigAggregate(
    string ProbeType,
    IProbeBehavior Behavior,
    IProbeThresholds Thresholds,
    ProbeSchedule Schedule
);
