namespace PingBoard.Services.ServerEvents;

public interface IServerEvent
{
    Guid EventId { get; set; }

    DateTime EventTime { get; set; }
}
