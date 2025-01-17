namespace PingBoard.Probes.Services;

using System.Text.RegularExpressions;
using NetworkProbes;
using NetworkProbes.Common;

public record HostnameTarget : INetworkProbeTarget
{
    public string Hostname { get; private set; }

    public HostnameTarget(string hostname)
    {
        Hostname = hostname;
    }

    public override string ToString()
    {
        return Hostname;
    }
}
