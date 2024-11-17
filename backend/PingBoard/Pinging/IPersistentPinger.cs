using System.Net;
using PingBoard.Database.Models;

namespace PingBoard.Pinging;

public interface IPersistentPinger
{
    public IAsyncEnumerable<PingResult> SendPingsAsync(IPAddress target, CancellationToken stoppingToken = default(CancellationToken));
}