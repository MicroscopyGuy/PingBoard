using PingBoard.Database.Models;

namespace PingBoard.Probes;
using PingBoard.Services;
using PingBoard.Pinging;
using PingBoard.Probes.Services;

using System.Net;

public class DnsProbe : INetworkProbeBase
{
    public string Name { get; } = "DnsLookup"; 
    
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