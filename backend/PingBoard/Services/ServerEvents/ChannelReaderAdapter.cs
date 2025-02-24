namespace PingBoard.Services.ServerEvents;

using System.Threading.Channels;
using Protos;

/// <summary>
/// Allows reading events from one of the ServerEvent channels so the events can be placed on the main
/// channel that sends events to the frontend.
/// </summary>
/// <typeparam name="T"></typeparam>
public class ChannelReaderAdapter<T> : IChannelReaderAdapter
    where T : IServerEvent
{
    private readonly ChannelReader<T> _channelReader;

    public ChannelReaderAdapter(ChannelReader<T> channelReader)
    {
        _channelReader = channelReader;
    }

    public async Task<bool> WaitToReadAsync(CancellationToken cancellationToken)
    {
        return await _channelReader.WaitToReadAsync(cancellationToken);
    }

    public IServerEvent? ReadNextServerEvent()
    {
        // if there is nothing to read
        if (!_channelReader.TryRead(out var serverEvent))
        {
            return null;
        }

        return serverEvent;
    }
}
