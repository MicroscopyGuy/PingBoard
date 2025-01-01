namespace PingBoard.Probes.NetworkProbes;

using System.Net;
using System.Net.NetworkInformation;
using PingBoard.Database.Models;
using PingBoard.Pinging;

public class PingProbe
    : INetworkProbeBase<PingProbeInvocationParams, ProbeStatusChange, PingProbeResult>,
        INetworkProbeBase
{
    public static string Name => "Ping";
    private IIndividualPinger _pinger;

    public PingProbe(IIndividualPinger pinger)
    {
        _pinger = pinger;
    }

    bool INetworkProbeBase.ShouldContinue(ProbeResult result)
    {
        return ShouldContinue((PingProbeResult)result);
    }

    /// <summary>
    /// Takes in the previous PingProbeResult and indicates if continuation is possible
    /// </summary>
    /// <param name="pingProbeRes"></param>
    /// <returns></returns>
    public bool ShouldContinue(PingProbeResult pingProbeRes)
    {
        if (pingProbeRes.IpStatus == null)
        {
            return false;
        }

        var responseIp = IPAddress.Parse(pingProbeRes.ReplyAddress);
        return pingProbeRes.IpStatus.Value.GetInfo(responseIp).State
            != PingingStates.PingState.Halt;
    }

    /// <summary>
    /// Invokes the probing operation
    /// </summary>
    /// <param name="probeInvocationParams"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
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
        pResult.Ttl = (short)probeConfiguration.Ttl;
        pResult.Target = probeConfiguration.GetTarget();
        try
        {
            pResult.Id = Guid.CreateVersion7();
            pResult.Start = DateTime.UtcNow;
            var result = await _pinger.SendPingIndividualAsync(
                probeConfiguration,
                cancellationToken
            );
            pResult.End = DateTime.UtcNow;
            pResult.ReplyAddress = result.Address.ToString();
            pResult.Success = result.Status == IPStatus.Success;
            pResult.Rtt = result.RoundtripTime;
        }
        catch (Exception ex)
        {
            pResult.End = DateTime.UtcNow;
            pResult.Error = ex;
        }

        return pResult;
    }
}
