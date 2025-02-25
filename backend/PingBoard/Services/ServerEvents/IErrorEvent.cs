namespace PingBoard.Services.ServerEvents;

public interface IErrorEvent
{
    string ErrorDescription { get; set; }
}
