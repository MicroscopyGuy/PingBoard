namespace PingBoard.Services.ServerEvents;

public record ProbeInfo : ServerEventBase, IProbeEvent
{
    public Guid ProbeId { get; set; }
    public string Target { get; set; }
    public string ProbeType { get; set; }

    public ProbeInfo(Guid probeId, string target, string probeType)
    {
        ProbeId = probeId;
        Target = target;
        ProbeType = probeType;
    }
};
