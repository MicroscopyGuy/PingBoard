namespace PingBoard.Probes.NetworkProbes.Dns;

using System.Net;
using PingBoard.Database.Models;
using PingBoard.Probes.Services;








/*
public class DnsProbe : INetworkProbeBase
{
    public static string Name =>  "DnsLookup";
    
    public async Task<ProbeResult> ProbeAsync(INetworkProbeTarget probeTarget, CancellationToken cancellationToken)
    {
        var probeResult = new DnsProbeResult();

        if (!cancellationToken.IsCancellationRequested)
        {
            try
            {
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

}*/
