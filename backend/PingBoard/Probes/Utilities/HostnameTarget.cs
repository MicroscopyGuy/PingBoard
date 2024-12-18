namespace PingBoard.Probes.Services;

using System.Text.RegularExpressions;

public class HostnameTarget
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
