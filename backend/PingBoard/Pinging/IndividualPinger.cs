namespace PingBoard.Pinging;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using PingBoard.Pinging.Configuration;
using Microsoft.Extensions.Options;
using System.Diagnostics.CodeAnalysis;


/// <summary>
/// Essentially a wrapper around C#'s Ping library. 
/// </summary>

public class IndividualPinger : IIndividualPinger{
    private readonly PingingBehaviorConfig _pingBehavior;
    private readonly ILogger<IndividualPinger> _logger;
    private readonly Ping _pinger;
    private int _ttl;
    private int _timeoutMs;
    private string _payloadStr;
    private bool _dontFragment;

    public IndividualPinger(Ping pinger, IOptions<PingingBehaviorConfig> pingBehavior, 
                            ILogger<IndividualPinger> logger){
        _pinger = pinger;
        _pingBehavior = pingBehavior.Value;
        _logger = logger;
        
        // used when not pinging functionality not provided more granular directions
        _ttl = _pingBehavior.Ttl;
        _dontFragment = true;
        _payloadStr = _pingBehavior.PayloadStr!;
        _timeoutMs = _pingBehavior.TimeoutMs;
    }

    public void SetTtl(int newTtl)
    {
        _ttl = newTtl;
    }

    public void IncrementTtl()
    {
        _ttl++;
    }
    
    public void SetTimeout(int newTimeoutMs)
    {
        _timeoutMs = newTimeoutMs;
    }

    public int GetTtl()
    {
        return _ttl;
    }

    public int GetTimeout()
    {
        return _timeoutMs;
    }
    

    public async Task<PingReply> SendPingIndividualAsync(IPAddress target, int timeoutMs, string payloadStr, int ttl,
        bool dontFragment, CancellationToken cancellationToken)
    {
        _logger.LogTrace("IndividualPinger: Sending ping");

        var options = new PingOptions()
        {
            Ttl = ttl,
            DontFragment = dontFragment
        };
        
        PingReply response = await _pinger.SendPingAsync(
            target, 
            TimeSpan.FromMilliseconds(timeoutMs),
            Encoding.ASCII.GetBytes(payloadStr), 
            options,
            cancellationToken
        );
        
        if (cancellationToken.IsCancellationRequested){
            _logger.LogDebug("IndividualPinger: Pinging cancelled");
        }

        return response;
    }
    public async Task<PingReply> SendPingIndividualAsync(IPAddress target, CancellationToken stoppingToken = default(CancellationToken))
    {
        var result = await SendPingIndividualAsync(
            target,
            _timeoutMs,
            _payloadStr!,
            _ttl,
            _dontFragment,
            stoppingToken
        );

        return result;
    }
    
    /*
    public async Task<PingReply> SendPingIndividualAsync(IPAddress target, CancellationToken stoppingToken = default(CancellationToken)){
        _logger.LogTrace("IndividualPinger: Sending ping");

        PingReply response = await _pinger.SendPingAsync(
            target, 
            TimeSpan.FromMilliseconds(_pingBehavior.TimeoutMs),
            Encoding.ASCII.GetBytes(_pingBehavior.PayloadStr!), 
            _pingOptions,
            stoppingToken
        );
        
        if (stoppingToken.IsCancellationRequested){
            _logger.LogDebug("IndividualPinger: Pinging cancelled");
        }

        return response;
    }*/
    
    
}

