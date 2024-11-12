namespace PingBoard.Endpoints;

public interface IChannelReaderAdapter
{
    public Task<bool> WaitToReadAsync(CancellationToken cancellationToken);
    public ServerEvent? ReadNextServerEvent();
}