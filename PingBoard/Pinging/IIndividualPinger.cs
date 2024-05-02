using System.Net;
using System.Net.NetworkInformation;

namespace PingBoard.Pinging{
    public interface IIndividualPinger{
        public Task<PingReply> SendPingIndividualAsync(IPAddress target);
    }
}
