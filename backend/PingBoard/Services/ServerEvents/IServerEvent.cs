namespace PingBoard.Services.ServerEvents;

public interface IServerEvent
{
    Guid EventId { get; }

    DateTime EventTime { get; }
}
