namespace PingBoard.Services;

using System.Threading.Channels;
using Grpc.Core;
using Protos;
using static Protos.ServerEvent.Types;

/// <summary>
/// Provides a single, combined stream of the ServerEvent channels to the frontend, through which all ServerEvents
/// are communicated.
/// </summary>
public class ServerEventEmitter //<T> where T: INetworkProbeBase
{
    private readonly Channel<PingAnomaly> _pingAnomalyChannel;
    private readonly Channel<PingOnOffToggle> _pingOnOffToggleChannel;
    private readonly Channel<PingAgentError> _pingAgentErrorChannel;
    private readonly Channel<PingInfo> _pingInfoChannel;

    //private readonly Func<INetworkProbeBase, channels> channelFactory;
    private readonly ILogger<ServerEventEmitter> _logger;

    public ServerEventEmitter(
        Channel<PingAnomaly> anomalyErrorChannel,
        Channel<PingOnOffToggle> pingOnOffToggleChannel,
        Channel<PingAgentError> pingAgentErrorChannel,
        Channel<PingInfo> pingInfoChannel,
        ILogger<ServerEventEmitter> logger
    )
    {
        _pingAnomalyChannel = anomalyErrorChannel;
        _pingOnOffToggleChannel = pingOnOffToggleChannel;
        _pingAgentErrorChannel = pingAgentErrorChannel;
        _pingInfoChannel = pingInfoChannel;
        _logger = logger;
    }

    /// <summary>
    /// An RPC that the backend can use to send a PingOnOffToggle ServerEvent through the stream
    /// </summary>
    /// <param name="target">The domain or IPAddress that has now either started or stopped being pinged.</param>
    /// <param name="status">A boolean value indicating whether the pinging is active or not</param>
    /// <param name="caller">The function that invoked this function, used for logging purposes</param>
    /// <exception cref="InvalidOperationException"></exception>
    public void IndicatePingOnOffToggle(string target, bool status, string caller)
    {
        try
        {
            var writeSuccess = _pingOnOffToggleChannel.Writer.TryWrite(
                new PingOnOffToggle
                {
                    PingTarget = new PingTarget { Target = target },
                    Active = status,
                }
            );

            if (!writeSuccess)
            {
                string message =
                    "An attempt to write to the PingOnOffToggle channel was unsuccessful.";
                throw (new InvalidOperationException(message));
            }
            _logger.LogDebug(
                $"ServerEventEmitter: IndicateChangedPingStatus: target{target}, status:{status}, caller:{caller}",
                target,
                status,
                caller
            );
        }
        catch (Exception e)
        {
            _logger.LogCritical(
                "ServerEventEmitter: IndicatePingOnOffToggle: ${caller}: ${eText}",
                caller,
                e.ToString()
            );
        }
    }

    /// <summary>
    /// An RPC that the backend can use to send a PingOnOffToggle ServerEvent through the stream
    /// </summary>
    /// <param name="target">The domain or IPAddress that has now either started or stopped being pinged.</param>
    /// <param name="description">A description of the PingAnomaly, ie, in which way(s) it is anomalous</param>
    /// <param name="caller">The function that invoked this function, used for logging purposes</param>
    /// <exception cref="InvalidOperationException"></exception>
    public void IndicatePingAnomaly(string target, string description, string caller)
    {
        try
        {
            var writeSuccess = _pingAnomalyChannel.Writer.TryWrite(
                new PingAnomaly()
                {
                    PingTarget = new PingTarget { Target = target },
                    AnomalyDescription = description,
                }
            );

            if (!writeSuccess)
            {
                string message =
                    "An attempt to write to the PingAnomalyIndicator channel was unsuccessful.";
                throw (new InvalidOperationException(message));
            }
            _logger.LogDebug(
                $"ServerEventEmitter: IndicatePingAnomaly: target{target}, anomalyDescription:{description}, caller:{caller}",
                target,
                caller
            );
        }
        catch (Exception e)
        {
            _logger.LogCritical(
                "ServerEventEmitter: IndicatePingAnomaly: ${caller}: ${eText}",
                caller,
                e.ToString()
            );
        }
    }

    /// <summary>
    /// An RPC that the backend can use to send a PingOnOffToggle ServerEvent through the stream
    /// </summary>
    /// <param name="target">The domain or IPAddress that has now either started or stopped being pinged.</param>
    /// <param name="caller">The function that invoked this function, used for logging purposes</param>
    /// <exception cref="InvalidOperationException"></exception>
    public void IndicatePingInfo(string target, string caller)
    {
        try
        {
            var writeSuccess = _pingInfoChannel.Writer.TryWrite(
                new PingInfo() { PingTarget = new PingTarget { Target = target } }
            );

            if (!writeSuccess)
            {
                string message =
                    "An attempt to write to the PingAnomalyIndicator channel was unsuccessful.";
                throw (new InvalidOperationException(message));
            }
            _logger.LogDebug(
                $"ServerEventEmitter: IndicatePingInfo: target{target}, caller:{caller}",
                target,
                caller
            );
        }
        catch (Exception e)
        {
            _logger.LogCritical(
                "ServerEventEmitter: IndicatePingInfo: ${caller}: ${eText}",
                caller,
                e.ToString()
            );
        }
    }

    /// <summary>
    /// An RPC that the backend can use to send a PingOnOffToggle ServerEvent through the stream
    /// </summary>
    /// <param name="target">The domain or IPAddress that has now either started or stopped being pinged.</param>
    /// <param name="caller">The function that invoked this function, used for logging purposes</param>
    /// <exception cref="InvalidOperationException"></exception>
    public void IndicatePingAgentError(string target, string caller)
    {
        try
        {
            var writeSuccess = _pingAgentErrorChannel.Writer.TryWrite(
                new PingAgentError() { PingTarget = new PingTarget { Target = target } }
            );

            if (!writeSuccess)
            {
                string message =
                    "An attempt to write to the PingAnomalyIndicator channel was unsuccessful.";
                throw (new InvalidOperationException(message));
            }
            _logger.LogDebug(
                $"ServerEventEmitter: IndicatePingInfo: target{target}, caller:{caller}",
                target,
                caller
            );
        }
        catch (Exception e)
        {
            _logger.LogCritical(
                "ServerEventEmitter: IndicatePingInfo: ${caller}: ${eText}",
                caller,
                e.ToString()
            );
        }
    }
}
