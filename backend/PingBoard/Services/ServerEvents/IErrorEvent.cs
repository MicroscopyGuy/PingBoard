namespace PingBoard.Services.ServerEvents;

public interface IErrorEvent : IServerEvent
{
    string ErrorDescription { get; set; }
}
