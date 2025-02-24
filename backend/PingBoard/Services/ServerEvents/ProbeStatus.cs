namespace PingBoard.Services.ServerEvents;

public record ProbeStatus : IProbeEvent
{
    public Guid EventId { get; set; }
    public Guid ProbeId { get; set; }
    public DateTime EventTime { get; set; }
    public string Status { get; set; }
    public string Target { get; set; }
    public string ProbeType { get; set; }
}
