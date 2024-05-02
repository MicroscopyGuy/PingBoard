using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using PingBoard.Pinging;
using Microsoft.AspNetCore.SignalR.Protocol;
using Microsoft.Extensions.Options;
using System.Runtime.CompilerServices;

namespace Pingboard.Pinging{
    /// <summary>
    /// Essentially a wrapper around C#'s Ping library. 
    /// </summary>
    public class IndividualPinger : IIndividualPinger{
        private readonly PingingBehaviorConfig _pingBehavior;
        private readonly ILogger<IndividualPinger> _logger;
        private readonly PingOptions _pingOptions;
        private readonly Ping _pinger;

        public IndividualPinger(Ping pinger, PingOptions options, IOptions<PingingBehaviorConfig> pingBehavior, 
                                ILogger<IndividualPinger> logger){
            _pinger = pinger;
            _pingBehavior = pingBehavior.Value;
            _logger = logger;
            _pingOptions = options;
            _pingOptions.Ttl = _pingBehavior.Ttl;
            _pingOptions.DontFragment = true; // Crucial, and not configurable
        }

        public async Task<PingReply> SendPingIndividualAsync(IPAddress target){
            PingReply response = await _pinger.SendPingAsync(
                target, 
                _pingBehavior.WaitMs,
                Encoding.ASCII.GetBytes(_pingBehavior.PayloadStr!), 
                _pingOptions
            );

            return response;
        }
    }

}
