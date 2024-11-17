namespace PingBoard.Services;

public interface INetworkProbe
{
    Task ExecuteAsync(CancellationToken cancellationToken);
}