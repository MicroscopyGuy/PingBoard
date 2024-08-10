namespace PingBoard.Services;
using System.Threading.Channels;
using Grpc.Core;

public class PingStatusIndicator
{
    private Channel<PingStatusMessage> _channel;
    public ChannelReader<PingStatusMessage> Reader;
    public ChannelWriter<PingStatusMessage> Writer;
    private const int _ChannelCapacity = 100;
    
    public PingStatusIndicator(Channel<PingStatusMessage> channel, ChannelReader<PingStatusMessage> reader,
                               ChannelWriter<PingStatusMessage> writer)
    {
        _channel = Channel.CreateBounded<PingStatusMessage>(
            new BoundedChannelOptions(_ChannelCapacity)
        );
    }
    
    
}