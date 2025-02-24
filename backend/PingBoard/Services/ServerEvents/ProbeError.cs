namespace PingBoard.Services.ServerEvents;

public class ProbeError : IProbeEvent, IErrorEvent
{
    public Guid EventId { get; set; }
    public Guid ProbeId { get; set; }
    public string ProbeType { get; set; }
    public DateTime EventTime { get; set; }
    public string ErrorDescription { get; set; }
    public string Target { get; set; }
}
