namespace PingBoard.Services.ServerEvents;

public class AdminError : IErrorEvent
{
    public Guid EventId { get; set; }
    public string ErrorDescription { get; set; }
    public DateTime EventTime { get; set; }
}
