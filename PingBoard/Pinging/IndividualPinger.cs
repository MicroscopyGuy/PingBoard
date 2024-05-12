using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using PingBoard.Pinging;
using PingBoard.Pinging.Configuration;
using Microsoft.AspNetCore.SignalR.Protocol;
using Microsoft.Extensions.Options;
using System.Runtime.CompilerServices;
using System.Diagnostics;

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
            Stopwatch timer = Stopwatch.StartNew();

            PingReply response = await _pinger.SendPingAsync(
                target, 
                _pingBehavior.TimeoutMs,
                Encoding.ASCII.GetBytes(_pingBehavior.PayloadStr!), 
                _pingOptions
            );
            
            timer.Stop();
            long elapsedMicroseconds = timer.ElapsedTicks/(Stopwatch.Frequency / (1000L * 1000L));
            float elapsedMilliseconds = (float) elapsedMicroseconds/ 1000L;
            if (elapsedMilliseconds < 1){
                Console.WriteLine($"Elapsed Time: {elapsedMilliseconds} milliseconds");
            }
            

            return response;
        }
    }

}
