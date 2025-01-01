using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using PingBoard.Database.Models;
using PingBoard.Database.Utilities;
using PingBoard.Probes;
using PingBoard.Probes.NetworkProbes;
using PingBoard.TestUtilities.PingingTestingUtilities;

namespace PingBoard.IntegrationTests;

using PingBoard.Services;
using Probes.Utilities;

public class NetworkProbeLiaisonTests
{
    /*
    [Fact]
    public async Task NetworkProbeLiason_CanProbe_WithPingProbe()
    {
        var pingProbe = new PingProbe(new IndividualPingerStub());

        // Get the factory and create a context
        var probeContextFactory = serviceProvider.GetRequiredService<
            IDbContextFactory<ProbeResultsContext>
        >();

        var contextFactory = new IDbContextFactory<ProbeResultsContext>();

        var crudOperations = new CrudOperations(
            probeContextFactory,
            NullLogger<CrudOperations>.Instance
        );

        var target = new IpAddressTarget("8.8.8.8");

        var probeParams = new PingProbeInvocationParams(target, 64, 500, "This is a test");
        var networkProbeLiason = new NetworkProbeLiaison(
            pingProbe,
            crudOperations,
            new CancellationTokenSource(),
            serviceProvider.GetRequiredService<ServerEventEmitter>(),
            probeParams,
            new NullLogger<NetworkProbeLiaison>()
        );

        var testTime = DateTime.UtcNow;
        networkProbeLiason.StartProbingAsync();
        await Task.Delay(20);
    } */
}
