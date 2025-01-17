namespace PingBoard.Probes.NetworkProbes.Common;

public class ProbeStatusChange
{
    public enum ProbeStatus
    {
        IDLE,
        RUNNING,
        FAULTED,
        COMPLETED,
        PAUSED,
        THROTTLED_BACKING_OFF,
        MISCONFIGURED,
    }

    public ProbeStatus NewStatus { get; set; }
    public ProbeStatus OldStatus { get; set; }
    public string ProbeType { get; set; }
    public string Target { get; set; }
    public string Message { get; set; }
}
