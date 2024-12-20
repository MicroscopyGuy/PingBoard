namespace PingBoard.Tests.ProbeTests;

using Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Probes.NetworkProbes;
using Probes.Services;

public class PingProbeTests
{
    /*
        [Fact]
        public async Task CanPingWithPingProbe()
        {
            var services = new ServiceCollection();
            var serviceProvider = services.BuildServiceProvider();
    
            var tokenSource = new CancellationTokenSource();
            var probe = serviceProvider.GetRequiredService<PingProbe>();
            
            var params = new PingProbeInvocationParams()
            {
                PacketPayload = "This is a test",
                TimeoutMs = 500,
                Ttl = 64,
                Target = new HostnameTarget()
            }
            var result = await probe.ProbeAsync();
        }
     */
}
