namespace PingBoard.Probes.NetworkProbes.Ping;

using System.Net.NetworkInformation;

public class PingProbeInvocationThresholds : IProbeInvocationThresholds
{
    public IPStatus acceptableStatus = IPStatus.Success;
    public long maxAllowedRtt;

    public PingProbeInvocationThresholds(long allowedRtt)
    {
        maxAllowedRtt = allowedRtt;
    }
}
