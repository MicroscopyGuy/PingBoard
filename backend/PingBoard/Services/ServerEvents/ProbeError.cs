namespace PingBoard.Services.ServerEvents;

public record ProbeError : ServerEventBase, IProbeEvent, IErrorEvent
{
    public Guid ProbeId { get; set; }
    public string ProbeType { get; set; }
    public string ErrorDescription { get; set; }
    public string Target { get; set; }

    public ProbeError(Guid probeId, string probeType, string target, string errorDescription)
    {
        ProbeId = probeId;
        ProbeType = probeType;
        Target = target;
        ErrorDescription = errorDescription;
    }
}
