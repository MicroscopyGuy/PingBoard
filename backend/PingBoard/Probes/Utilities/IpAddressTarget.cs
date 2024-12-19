namespace PingBoard.Probes.Utilities;

using NetworkProbes;

public class IpAddressTarget : INetworkProbeTarget
{
    public string IpAddress { get; set; }

    public IpAddressTarget(string ipAddress)
    {
        IpAddress = ipAddress;
    }
}
