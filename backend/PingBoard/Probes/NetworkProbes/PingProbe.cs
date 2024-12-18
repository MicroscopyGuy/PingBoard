using System.Net;
using PingBoard.Database.Models;
using PingBoard.Pinging;
using PingBoard.Probes.Services;

namespace PingBoard.Probes.NetworkProbes;

public class PingProbe
    : INetworkProbeBase<PingProbeInvocationParams, ProbeStatusChange, PingProbeResult>,
        INetworkProbeBase
{
    public string Name => _name;

    private IIndividualPinger _pinger;

    private readonly string _name = "PingProbe";

    // private DnsProbe _dnsProbe; perhaps later, for now use SendPingAsync function with DNS
    public List<Type> SupportedTargetTypes =>
        new List<Type>() { typeof(Hostname), typeof(IPAddress) };

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

    public PingProbe(IIndividualPinger pinger)
    {
        _pinger = pinger;
    }

    bool INetworkProbeBase.ShouldContinue(ProbeResult result)
    {
        return ShouldContinue((PingProbeResult)result);
    }

    public bool ShouldContinue(PingProbeResult pingProbeRes)
    {
        //var pProbeRes = (PingProbeResult) pingProbeResult;
        if (pingProbeRes.IpStatus == null)
        {
            return false;
        }
        return pingProbeRes.IpStatus.Value.GetInfo().State != PingingStates.PingState.Halt;
    }

    async Task<ProbeResult> INetworkProbeBase.ProbeAsync(
        IProbeInvocationParams probeInvocationParams,
        CancellationToken cancellationToken
    )
    {
        return await ProbeAsync(
            (PingProbeInvocationParams)probeInvocationParams,
            cancellationToken
        );
    }

    public async Task<PingProbeResult> ProbeAsync(
        PingProbeInvocationParams probeConfiguration,
        CancellationToken cancellationToken
    )
    {
        var pResult = new PingProbeResult();
        pResult.Id = Guid.CreateVersion7();

        var result = await _pinger.SendPingIndividualAsync(
            probeConfiguration.Target.ToString(),
            cancellationToken
        );

        return pResult;
    }
}
