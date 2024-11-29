using System.Net.NetworkInformation;
using PingBoard.Database.Models;

namespace PingBoard.Pinging;

public static class PingResultExtensions
{
    public static bool PingQualityWithinThresholds(this PingProbeResult pingProbeResult, int rttThreshold)
    {
        return (pingProbeResult.Rtt < rttThreshold && pingProbeResult.IpStatus == IPStatus.Success);
    }
}