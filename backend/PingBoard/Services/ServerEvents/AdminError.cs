namespace PingBoard.Services.ServerEvents;

public record AdminError : ServerEventBase, IErrorEvent
{
    public string ErrorDescription { get; set; }

    public AdminError(string errorDescription)
    {
        ErrorDescription = errorDescription;
    }
}
