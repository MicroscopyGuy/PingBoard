using System.Net;
using System.Net.NetworkInformation;

namespace PingBoard.Pinging{
    public interface IGroupPinger{
        public Task<PingGroupSummary> SendPingGroupAsync(IPAddress target, int numToSend);

    }
}