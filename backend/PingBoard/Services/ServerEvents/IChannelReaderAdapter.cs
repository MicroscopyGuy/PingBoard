namespace PingBoard.Services.ServerEvents;

public interface IChannelReaderAdapter
{
    public Task<bool> WaitToReadAsync(CancellationToken cancellationToken);
    public IServerEvent ReadNextServerEvent();
}
