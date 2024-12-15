using System.Net;
using PingBoard.Database.Models;
using PingBoard.Probes.Services;

namespace PingBoard.Probes.NetworkProbes;

public class DnsProbe : INetworkProbeBase
{
    private readonly string _name = "DnsLookup";

    public string Name => _name;

    public List<Type> SupportedTargetTypes => new List<Type>()
    { 
        typeof (Hostname)
    };

    private bool IsSupportedTarget(Type targetType)
    {
        foreach (Type supportedType in SupportedTargetTypes)
        {
            if (targetType == supportedType)
            {
                return true;
            }
        }

        return false;
    }
    
    
    public async Task<ProbeResult> ProbeAsync(INetworkProbeTarget probeTarget, CancellationToken cancellationToken)
    {
        var probeResult = new DnsProbeResult();

        if (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                if (!IsSupportedTarget(probeTarget.TargetType))
                {
                    throw new ArgumentException("Provided target type is not supported by DnsProbe");
                }
                var uri = new Uri(probeTarget.Target.ToString()!);
                var host = uri.Host;
                var hostEntry = await Dns.GetHostEntryAsync(host);
                
                probeResult.IpAddresses = hostEntry.AddressList;
                probeResult.Aliases = hostEntry.Aliases;
                probeResult.PrimaryHostName = hostEntry.HostName;
            }
        
            catch (Exception e)
            {
                probeResult.Success = false;
                probeResult.Error = e;
            }
        }
        return probeResult;

    }

    public bool ShouldContinue(ProbeResult lastResult)
    {
        return true;
    }

}