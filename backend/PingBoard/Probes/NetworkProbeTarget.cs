using System.Net;
using PingBoard.Probes.Services;
using PingBoard.Services;

namespace PingBoard.Probes;

public class PingProbeTarget : INetworkProbeTarget
{
    public string Name { get; }
    public object Target { get; }
    public Type TargetType { get; }

    public PingProbeTarget(string name, object target)
    {
        Name = name;
        TargetType = target.GetType();
        Target = target;
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