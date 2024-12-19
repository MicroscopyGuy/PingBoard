namespace PingBoard.Probes.Services;

using System.Text.RegularExpressions;
using NetworkProbes;

public class HostnameTarget : INetworkProbeTarget
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
