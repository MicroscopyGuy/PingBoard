namespace PingBoard.Pinging;
using System.Net;
using System.Net.NetworkInformation;


public interface IIndividualPinger
{
    
    public Task<PingReply> SendPingIndividualAsync(string target, CancellationToken stoppingToken = default(CancellationToken));
    public int GetTtl();
    public void SetTtl(int newTtl);
    public int GetTimeout();
    public void SetTimeout(int newTimeoutMs);
    

}

