namespace PingBoard.Services.ServerEvents;

public record ProbeStatus : ServerEventBase, IProbeEvent
{
    public Guid ProbeId { get; set; }
    public string Status { get; set; }
    public string Target { get; set; }
    public string ProbeType { get; set; }

    public ProbeStatus(Guid probeId, string status, string target, string probeType)
    {
        ProbeId = probeId;
        Status = status;
        Target = target;
        ProbeType = probeType;
    }
}
