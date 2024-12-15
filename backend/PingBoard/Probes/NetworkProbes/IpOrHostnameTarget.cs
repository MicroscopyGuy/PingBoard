namespace PingBoard.Probes.NetworkProbes;

using System.Net;
using Services;

public class IpOrHostnameTarget : INetworkProbeTarget<IpOrHostnameTarget>
{
    public string Name => "IpOrHostname";

    public object Target { get; }

    public Type TargetType { get; }

    public IpOrHostnameTarget(IPAddress ipAddress)
    {
        TargetType = ipAddress.GetType();
        Target = ipAddress;
    }

    public IpOrHostnameTarget(Hostname hostname)
    {
        TargetType = hostname.GetType();
        Target = hostname;
    }

    public override string ToString()
    {
        return Target.ToString()!;
    }

    public static bool TryParse(string input, out IpOrHostnameTarget? target)
    {
        target = null;
        if (IPAddress.TryParse(input, out var ip))
        {
            target = new IpOrHostnameTarget(ip);
            return true;
        }
        else if (Hostname.TryParse(input, out var hostname))
        {
            target = new IpOrHostnameTarget(hostname!);
            return true;
        }

        return false;
    }

    /*
    public static bool Validate()
    {
        IPAddress ipMaybe;
        bool validIp = IPAddress.TryParse(Target.ToString(), out ipMaybe);

        if (!validIp)
        {
            try
            {
                var hostnameMaybe = new Hostname(Target.ToString());
            }
            catch
            {
                return false;
            }
        }
        else
        {
            return true;
        }

        return false;
    }*/
}
