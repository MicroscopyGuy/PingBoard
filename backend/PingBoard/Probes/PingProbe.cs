using System.Net;
using PingBoard.Pinging;
using PingBoard.Probes.Services;
using PingBoard.Services;

namespace PingBoard.Probes;

public class PingProbe : INetworkProbeBase
{
    private IndividualPinger _pinger;
    private DnsProbe _dnsProbe;
    public List<Type> SupportedTypes => new List<Type>()
    { 
        typeof (Hostname), typeof (IPAddress)
    };

    public void SetTtl(int newTtl)
    {
        _pinger.SetTtl(newTtl);
        
    }

    public void SetTimeoutMs(int newTimeoutMs)
    {
        _pinger.SetTimeout(newTimeoutMs);
    }

    public int GetTtl()
    {
        return _pinger.GetTtl();
    }

    public int GetTimeoutMs()
    {
        return _pinger.GetTimeout();
    }
    
    PingProbe(DnsProbe dnsProbe, IndividualPinger pinger)
    {
        _pinger = pinger;
    }

    public async Task<ProbeResult> ProbeAsync(INetworkProbeTarget probeTarget, CancellationToken cancellationToken)
    {
        
        var result = await _pinger.SendPingIndividualAsync(IPAddress.Parse(probeTarget.Target));

        var pResult = new ProbeResult()
        {
            Id = Guid.CreateNew7(),
            
            
        }
        return result;
    } 
        
        
}