using System.Net;
using PingBoard.Pinging;
using PingBoard.Probes.Services;
using PingBoard.Services;

namespace PingBoard.Probes;

public class PingProbe : INetworkProbeBase
{
    private IndividualPinger _pinger; 
    public List<Type> SupportedTypes => new List<Type>()
    { 
        typeof (Hostname), typeof (IPAddress)
    };

    PingProbe(IndividualPinger pinger)
    {
        _pinger = pinger;
    }

    public async Task<ProbeResult> ProbeAsync(INetworkProbeTarget probeTarget, CancellationToken cancellationToken)
    {
        var result = await _pinger.SendPingIndividualAsync(IPAddress.Parse(probeTarget.Target));
    } 
        
        
}