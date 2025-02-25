namespace PingBoard.Services.ServerEvents;

public record ProbeAnomaly : ServerEventBase, IProbeEvent
{
    public Guid ProbeId { get; set; }
    public string ProbeType { get; set; }
    public string Target { get; set; }

    //public string AnomalyDescription { get; set; } not used for right now, will be later
    public ProbeAnomaly(Guid probeId, string target, string probeType)
    {
        ProbeId = probeId;
        Target = target;
        ProbeType = probeType;
    }
}
