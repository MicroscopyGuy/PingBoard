﻿using System.Net;
using PingBoard.Database.Models;
using PingBoard.Probes.Services;

namespace PingBoard.Probes.NetworkProbes;


/*
public class DnsProbe : INetworkProbeBase
{
    private readonly string _name = "DnsLookup";

    public string Name => _name;
    
    
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