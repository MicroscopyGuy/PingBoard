using System.Net;
using System.Runtime;
using PingBoard.Database.Models;
using PingBoard.Pinging;
using PingBoard.Probes.Services;
using PingBoard.Services;

namespace PingBoard.Probes;

public class PingProbe : INetworkProbeBase
{
    public string Name { get; } = "PingProbe";
    private IIndividualPinger _pinger;
    // private DnsProbe _dnsProbe; perhaps later, for now use SendPingAsync function with DNS
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
    
    PingProbe(IIndividualPinger pinger)
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
        pResult.Id = Guid.CreateVersion7();

        var result = await _pinger.SendPingIndividualAsync(probeTarget.Target.ToString(), cancellationToken);

        return pResult;
    } 
        
        
}