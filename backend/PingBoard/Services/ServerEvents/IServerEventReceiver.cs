namespace PingBoard.Services.ServerEvents;

public interface IServerEventReceiver
{
    Task ReceiveServerEvent(string eventDetails);
}
