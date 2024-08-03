using System.Net;
using System.Net.NetworkInformation;
using PingBoard.Database.Models;

namespace PingBoard.Pinging{
    public interface IGroupPinger{
        public Task<PingGroupSummary> SendPingGroupAsync(IPAddress target, CancellationToken stoppingToken = default(CancellationToken));

    }
}