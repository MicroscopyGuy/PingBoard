using System.Net.NetworkInformation;
using PingBoard.Database.Models;

namespace PingBoard.Pinging;

public static class PingResultExtensions
{
    public static bool PingQualityWithinThresholds(this PingResult pingResult, int rttThreshold)
    {
        return (pingResult.Rtt < rttThreshold && pingResult.IpStatus == IPStatus.Success);
    }
}