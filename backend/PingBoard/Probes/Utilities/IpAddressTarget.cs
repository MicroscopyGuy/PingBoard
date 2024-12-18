namespace PingBoard.Probes.Utilities;

public class IpAddressTarget
{
    public string IpAddress { get; set; }

    public IpAddressTarget(string ipAddress)
    {
        IpAddress = ipAddress;
    }
}
