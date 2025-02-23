namespace PingBoard.Probes.NetworkProbes.Common;

public record ProbeConfigAggregate
{
    public string ProbeType { get; set; }
    public IProbeBehavior Behavior { get; set; }
    public IProbeThresholds Thresholds { get; set; }
    public ProbeSchedule Schedule { get; set; }
}
