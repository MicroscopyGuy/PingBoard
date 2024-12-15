namespace PingBoard.Probes.NetworkProbes;

public class IpOrHostnameTarget : INetworkProbeTarget<IpOrHostnameTarget>
{
    private readonly string _name;
    private readonly object _target;
    private readonly Type _targetType;

    public string Name => _name;

    public object Target => _target;

    public Type TargetType => _targetType;

    public IpOrHostnameTarget(string name, object target)
    {
        _name = name;
        _targetType = target.GetType();
        _target = target;
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