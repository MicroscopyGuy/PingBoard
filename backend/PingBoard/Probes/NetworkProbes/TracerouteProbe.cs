using System.Net;
using PingBoard.Pinging;
using PingBoard.Probes.Services;

namespace PingBoard.Probes.NetworkProbes;


/*
public class TracerouteProbe : INetworkProbeBase
{
    public string Name => _name;

    private IIndividualPinger _pinger;

    private readonly string _name = "TracerouteProbe";

    // private DnsProbe _dnsProbe; perhaps later, for now use SendPingAsync function with DNS
    public List<Type> SupportedTargetTypes => new List<Type>()
    {
        typeof (Hostname), typeof (IPAddress)
    };

    public async Task<ProbeResult> ProbeAsync(INetworkProbeTarget target, CancellationToken cancellationToken)
    {
        // Step 0: Run reverse DNS request on the Hostname, if provided
        // Step 1: Ping final destination for reachability and for TTL
        // Step 2: Continue pinging starting from TTL of 1 until the final destination is reached,
        // Step 3: Run a reverse DNS on the first IPAddress retrieved from Step 2 to build map.
        // Step 4: ttl++ then step 2, then step 3 as needed until either final destination is reached or the TTL > step 1
        throw new NotImplementedException("StartProbingAsync is not yet implemented for the TracerouteProbe.");
        
    }

    public bool ShouldContinue(ProbeResult result)
    {
        throw new NotImplementedException("ShouldContinue is not yet implemented for the TracerouteProbe.");
    }
}*/
