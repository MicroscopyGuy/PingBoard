namespace PingBoard.Services.ServerEvents;

public record ServerEventBase : IServerEvent
{
    public Guid EventId { get; set; }
    public DateTime EventTime { get; set; }

    public ServerEventBase()
    {
        EventId = Guid.CreateVersion7();
        EventTime = DateTime.UtcNow;
    }
}
