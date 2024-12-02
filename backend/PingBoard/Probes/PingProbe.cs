using System.Net;
using PingBoard.Database.Models;
using PingBoard.Pinging;
using PingBoard.Probes.Services;
using PingBoard.Services;

namespace PingBoard.Probes;

public class PingProbe : INetworkProbeBase
{
    public string Name { get; } = "PingProbe";
    private IndividualPinger _pinger;
    private DnsProbe _dnsProbe; 
    public List<Type> SupportedTargetTypes => new List<Type>()
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

    public bool ShouldContinue(ProbeResult pingProbeResult)
    {
        var pProbeRes = (PingProbeResult)pingProbeResult;
        if (pProbeRes.IpStatus == null)
        {
            return false;
        }
        return pProbeRes.IpStatus.Value.GetInfo().State != PingingStates.PingState.Halt;
    }
    
    public async Task<ProbeResult> ProbeAsync(INetworkProbeTarget probeTarget, CancellationToken cancellationToken)
    {
        var pResult = new PingProbeResult();
        //pResult.Id = Guid.CreateVersion7(); requires .NET 9
        if (probeTarget.TargetType != typeof (IPAddress))
        {
            var dnsResult = await _dnsProbe.ProbeAsync(probeTarget, cancellationToken);
            pResult.dnsProbeResult = (DnsProbeResult) dnsResult;
        }
        
        var result = await _pinger.SendPingIndividualAsync(IPAddress.Parse(probeTarget.Target!.ToString()));

        return pResult;
    } 
        
        
}