namespace PingBoard.Pinging;
using System.Net;
using System.Net.NetworkInformation;


public interface IIndividualPinger{
    public Task<PingReply> SendPingIndividualAsync(IPAddress target, CancellationToken stoppingToken);
}

