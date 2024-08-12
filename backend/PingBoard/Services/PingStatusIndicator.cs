namespace PingBoard.Services;
using System.Threading.Channels;
using Grpc.Core;

public class PingStatusIndicator
{
    private Channel<PingStatusMessage> _channel;
    public ChannelReader<PingStatusMessage> Reader;
    public ChannelWriter<PingStatusMessage> Writer;
    
    public PingStatusIndicator(Channel<PingStatusMessage> channel)
    {
        _channel = channel;
        this.Reader = channel.Reader;
        this.Writer = channel.Writer;
    }
    
    
}