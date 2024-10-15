namespace PingBoard.Services;
using System.Threading.Channels;
using Grpc.Core;

using static ServerEvent.Types;

public class ServerEventEmitter
{
    private readonly Channel<PingAnomaly> _pingAnomalyChannel;
    private readonly Channel<PingOnOffToggle> _pingOnOffToggleChannel;
    private readonly Channel<PingAgentError> _pingAgentErrorChannel;
    private readonly ILogger<ServerEventEmitter> _logger;


    public ServerEventEmitter(Channel<PingAnomaly> anomalyErrorChannel, Channel<PingOnOffToggle> pingOnOffToggleChannel, 
         Channel<PingAgentError> pingAgentErrorChannel, ILogger<ServerEventEmitter> logger)
    {
        _pingAnomalyChannel = anomalyErrorChannel;
        _pingOnOffToggleChannel = pingOnOffToggleChannel;
        _pingAgentErrorChannel = pingAgentErrorChannel;
        _logger = logger;
    }

    // must rename this to IndicatePingOnOffToggle as soon as feasible
    public void IndicatePingOnOffToggle(string target, bool status, string caller)
    {
        try
        {
            var writeSuccess = _pingOnOffToggleChannel.Writer.TryWrite(new PingOnOffToggle
            {
                PingTarget = new PingTarget { Target = target },
                Active = status
            });
                
            if (!writeSuccess)
            {
                string message = "An attempt to write to the PingOnOffToggle channel was unsuccessful.";
                throw (new InvalidOperationException(message));
            }
            _logger.LogDebug($"ServerEventEmitter: IndicateChangedPingStatus: target{target}, status:{status}, caller:{caller}", target, status, caller);
        }
        
        catch (Exception e)
        {
            _logger.LogCritical("ServerEventEmitter: IndicateChangedPingStatus: ${caller}: ${eText}", caller, e.ToString());
        }
    }
    
    public void IndicatePingAnomaly(string target, string description, string caller)
    {
        try
        {
            var writeSuccess = _pingAnomalyChannel.Writer.TryWrite(new PingAnomaly()
            {
                PingTarget = new PingTarget { Target = target },
                AnomalyDescription = description
            });
                
            if (!writeSuccess)
            {
                string message = "An attempt to write to the PingAnomalyIndicator channel was unsuccessful.";
                throw (new InvalidOperationException(message));
            }
            _logger.LogDebug($"ServerEventEmitter: IndicatePingAnomaly: target{target}, anomalyDescription:{description}, caller:{caller}", target, caller);
        }
        
        catch (Exception e)
        {
            _logger.LogCritical("ServerEventEmitter: IndicateChangedPingStatus: ${caller}: ${eText}", caller, e.ToString());
        }
    }
}