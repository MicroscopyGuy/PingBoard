namespace PingBoard.Probes.NetworkProbes.Ping;

using System.Net;

public static class IpAddressExtensions
{
    public static bool IsIPv4(this IPAddress ip)
    {
        return ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork;
    }
}
