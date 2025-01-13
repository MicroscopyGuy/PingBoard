namespace PingBoard.Probes.NetworkProbes.Ping;

using System.Net.NetworkInformation;
using PingBoard.Probes.Common;

public class PingProbeThresholds : IProbeThresholds
{
    public IPStatus AcceptableStatus = IPStatus.Success;
    public long MaxAllowedRtt;

    public PingProbeThresholds(long allowedRtt)
    {
        MaxAllowedRtt = allowedRtt;
    }
}
