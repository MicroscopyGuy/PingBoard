namespace PingBoard.Probes.NetworkProbes.Ping;

using System.Net.NetworkInformation;
using PingBoard.Database.Models;

public static class PingResultExtensions
{
    public static bool PingQualityWithinThresholds(
        this PingProbeResult pingProbeResult,
        int rttThreshold
    )
    {
        return (pingProbeResult.Rtt < rttThreshold && pingProbeResult.IpStatus == IPStatus.Success);
    }
}
