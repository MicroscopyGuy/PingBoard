namespace PingBoard.Endpoints;

using System.Threading.Channels;
using Protos;

public class ChannelReaderAdapter<T> : IChannelReaderAdapter
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

    public ServerEvent? ReadNextServerEvent()
    {
        T item = default(T);
        // if there is nothing to read
        if (!_channelReader.TryRead(out item))
        {
            return null;
        }

        var serverEvent = new ServerEvent();
        foreach (var serverEventProperty in serverEvent.GetType().GetProperties())
        {
            if (typeof(T) == serverEventProperty.PropertyType)
            {
                serverEventProperty.SetValue(serverEvent, item);
                break;
            }
        }
        return serverEvent;
    }
}
