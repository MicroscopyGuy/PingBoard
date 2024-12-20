namespace PingBoard.Endpoints;

using Protos;

public interface IChannelReaderAdapter
{
    public Task<bool> WaitToReadAsync(CancellationToken cancellationToken);
    public ServerEvent? ReadNextServerEvent();
}
