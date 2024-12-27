using System.Net;
using PingBoard.Database.Models;
using PingBoard.Pinging;
using PingBoard.Probes.Services;

namespace PingBoard.Probes.NetworkProbes;

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
        pResult.Id = Guid.CreateVersion7();

        var result = await _pinger.SendPingIndividualAsync(probeConfiguration, cancellationToken);

        return pResult;
    }
}
