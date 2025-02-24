namespace PingBoard.Services;

using System.Threading.Channels;
using Grpc.Core;
using Services.ServerEvents;

/// <summary>
/// Provides a single, combined stream of the ServerEvent channels to the frontend, through which all ServerEvents
/// are communicated.
/// </summary>
public class ServerEventEmitter //<T> where T: INetworkProbeBase
{
    private readonly Channel<IServerEvent> _serverEventChannel;

    //private readonly IChannelReaderAdapter _channelReaderAdapter;

    //private readonly Func<INetworkProbeBase, channels> channelFactory;
    private readonly ILogger<ServerEventEmitter> _logger;

    public ServerEventEmitter(
        Channel<IServerEvent> serverEventChannel,
        ILogger<ServerEventEmitter> logger
    )
    {
        _serverEventChannel = serverEventChannel;
        //_channelReaderAdapter = new ChannelReaderAdapter<IServerEvent>(_serverEventChannel.Reader);
        _logger = logger;
    }

    /// <summary>
    /// Sends a ServerEvent to the frontend
    /// </summary>
    /// <param name="serverEvent">The server event to be sent.</param>
    /// <param name="caller">The function that invoked this function, used for logging purposes.</param>
    /// <exception cref="InvalidOperationException"></exception>
    public void IndicateServerEvent(IServerEvent serverEvent, string caller)
    {
        try
        {
            var writeSuccess = _serverEventChannel.Writer.TryWrite(serverEvent);
            if (!writeSuccess)
            {
                string message =
                    "An attempt to write to the PingOnOffToggle channel was unsuccessful.";
                throw (new InvalidOperationException(message));
            }
            _logger.LogDebug(
                $"ServerEventEmitter: IndicateServerEvent: caller:{caller} event:{serverEvent}",
                caller,
                serverEvent
            );
        }
        catch (Exception e)
        {
            _logger.LogCritical(
                "ServerEventEmitter: IndicatePingOnOffToggle: ${caller}: ${eText}",
                caller,
                e.ToString()
            );

            // If an event cannot be written to the server event channel, information that the frontend needs will not
            // be available; buttons will not update, graphs won't request new information. In short, the application
            // would be, or soon become unusable. For this reason, the application must be closed and restarted.
            string failFastMsg = """
                One or more events could not be communicated to the frontend for an unknown reason.
                Application is now in an unusable state and must be restarted.
                """;

            // kills the application immediately, logs to the OS crash logs
            Environment.FailFast(failFastMsg, e);
        }
    }
}
