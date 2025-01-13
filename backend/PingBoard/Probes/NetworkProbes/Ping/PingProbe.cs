namespace PingBoard.Probes.NetworkProbes.Ping;

using System.Net;
using System.Net.NetworkInformation;
using Common;
using PingBoard.Database.Models;
using PingBoard.Probes.NetworkProbes;
using Probes.Common;

public class PingProbe
    : INetworkProbeBase<PingProbeBehavior, PingProbeThresholds, PingProbeResult>,
        INetworkProbeBase
{
    public static string Name => "Ping";
    private IIndividualPinger _pinger;

    public PingProbe(IIndividualPinger pinger)
    {
        _pinger = pinger;
    }

    public string GetName()
    {
        return Name;
    }

    ProbeResult INetworkProbeBase.NewResult()
    {
        return NewResult();
    }

    public PingProbeResult NewResult()
    {
        return new PingProbeResult();
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
    // csharpier-ignore-start
    public bool ShouldContinue(PingProbeResult pingProbeRes)
    {
        return pingProbeRes.IpStatus == null
            || pingProbeRes
                   .IpStatus
                   .Value
                   .GetInfo(IPAddress.Parse(pingProbeRes.ReplyAddress))
                   .State
                != PingingStates.PingState.Halt;
    }
    // csharpier-ignore-end

    public bool IsAnomaly(ProbeResult result, IProbeThresholds thresholds)
    {
        bool success = ((PingProbeResult)result).IpStatus == IPStatus.Success;
        bool allowedRtt =
            ((PingProbeResult)result).Rtt <= ((PingProbeThresholds)thresholds).MaxAllowedRtt;
        return !success || !allowedRtt;
    }

    bool INetworkProbeBase.IsAnomaly(ProbeResult pingProbeRes, IProbeThresholds thresholds)
    {
        var res = (PingProbeResult)pingProbeRes;
        return IsAnomaly(pingProbeRes, thresholds);
    }

    /// <summary>
    /// Invokes the probing operation
    /// </summary>
    /// <param name="probeInvocationParams"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    async Task<ProbeResult> INetworkProbeBase.ProbeAsync(
        IProbeBehavior probeBehavior,
        CancellationToken cancellationToken
    )
    {
        return await ProbeAsync(probeBehavior! as PingProbeBehavior, cancellationToken);
    }

    public async Task<PingProbeResult> ProbeAsync(
        PingProbeBehavior probeConfiguration,
        CancellationToken cancellationToken
    )
    {
        var pResult = new PingProbeResult();
        pResult.Ttl = (short)probeConfiguration.Ttl;
        pResult.Target = probeConfiguration.GetTarget();
        pResult.ProbeType = Name;
        pResult.Start = DateTime.UtcNow;
        try
        {
            var result = await _pinger.SendPingIndividualAsync(
                probeConfiguration,
                cancellationToken
            );

            pResult.End = DateTime.UtcNow;
            pResult.IpStatus = result.Status;
            pResult.ReplyAddress = result.Address.ToString();
            pResult.Success = result.Status == IPStatus.Success;
            pResult.Rtt = result.RoundtripTime;
        }
        catch (Exception ex)
        {
            pResult.End = DateTime.UtcNow;
        }

        return pResult;
    }
}
