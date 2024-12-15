namespace PingBoard.Probes.NetworkProbes;

using Microsoft.EntityFrameworkCore.Internal;

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

    public ProbeStatus NewStatus;
    public ProbeStatus OldStatus;
    public string ProbeType;
    public string Target;
    public string Message;
}
