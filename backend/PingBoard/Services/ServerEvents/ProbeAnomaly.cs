namespace PingBoard.Services.ServerEvents;

public record ProbeAnomaly : IProbeEvent
{
    public Guid EventId { get; set; }
    public Guid ProbeId { get; set; }
    public DateTime EventTime { get; set; }
    public string ProbeType { get; set; }
    public string AnomalyDescription { get; set; }
    public string Target { get; set; }
}
