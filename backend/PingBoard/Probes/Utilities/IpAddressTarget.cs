namespace PingBoard.Probes.Utilities;

using NetworkProbes;
using NetworkProbes.Common;

public record IpAddressTarget : INetworkProbeTarget
{
    public string IpAddress { get; set; }

    public IpAddressTarget(string ipAddress)
    {
        IpAddress = ipAddress;
    }

    public override string ToString()
    {
        return IpAddress;
    }
}
