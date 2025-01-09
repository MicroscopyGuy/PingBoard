namespace PingBoard.Probes.NetworkProbes.Ping;

using System.Net.NetworkInformation;
using System.Text;
using Probes.NetworkProbes;

/// <summary>
/// Essentially a wrapper around C#'s Ping library.
/// </summary>

public class IndividualPinger : IIndividualPinger
{
    private readonly ILogger<IndividualPinger> _logger;
    private readonly Ping _pinger;
    private int _ttl;
    private int _timeoutMs;
    private string _payloadStr;
    private const bool _DONT_FRAGMENT = true;

    public IndividualPinger(Ping pinger, ILogger<IndividualPinger> logger)
    {
        _pinger = pinger;
        _logger = logger;
        // used when pinging functionality not provided more granular directions
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

    public async Task<PingReply> SendPingIndividualAsync(
        PingProbeBehavior behavior,
        CancellationToken cancellationToken
    )
    {
        _logger.LogTrace("IndividualPinger: Sending ping");

        var options = new PingOptions() { Ttl = behavior.Ttl, DontFragment = _DONT_FRAGMENT };

        PingReply response = await _pinger.SendPingAsync(
            behavior.GetTarget(),
            TimeSpan.FromMilliseconds(behavior.TimeoutMs),
            Encoding.ASCII.GetBytes(behavior.PacketPayload),
            options,
            cancellationToken
        );

        if (cancellationToken.IsCancellationRequested)
        {
            _logger.LogDebug("IndividualPinger: Pinging cancelled");
        }

        return response;
    }
}
