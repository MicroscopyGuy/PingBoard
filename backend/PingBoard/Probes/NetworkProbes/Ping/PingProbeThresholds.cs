namespace PingBoard.Probes.NetworkProbes.Ping;

using System.Net.NetworkInformation;
using PingBoard.Probes.Common;

public class PingProbeThresholds : IProbeThresholds
{
    public IPStatus acceptableStatus = IPStatus.Success;
    public long maxAllowedRtt;

    public PingProbeThresholds(long allowedRtt)
    {
        maxAllowedRtt = allowedRtt;
    }
}
