namespace PingBoard.IntegrationTests;

using System.Net.NetworkInformation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using PingBoard.Database.Models;
using PingBoard.Database.Utilities;
using PingBoard.Probes;
using PingBoard.Probes.NetworkProbes;
using PingBoard.Services;
using PingBoard.TestUtilities.PingingTestingUtilities;
using Probes.NetworkProbes.Common;
using Probes.NetworkProbes.Ping;
using Probes.Utilities;
using TestUtilities;

public class NetworkProbeLiaisonTests
{
    [Fact]
    public async Task NetworkProbeLiaison_CanProbe_WithPingProbe_WithoutThrowingException()
    {
        var _appFixture = new AppFixture();

        // Get the factory and create a context
        var contextFactory = _appFixture.App.Services.GetRequiredService<
            IDbContextFactory<ProbeResultsContext>
        >();

        var crudOperations = new CrudOperations(
            contextFactory,
            NullLogger<CrudOperations>.Instance // to avoid scenario where test writes to logs
        );

        var pingerStub = new IndividualPingerStub();
        var t = "0.0.0.0";

        List<long> rtts = [0, 0];
        List<IPStatus> statuses = [IPStatus.Success, IPStatus.Success];
        List<byte[]> buffers =
        [
            [],
            [],
        ];
        List<string> addresses = [t, t];
        List<int> ttls = [64, 64];
        pingerStub.PrepareStubbedPingReplies(rtts, statuses, buffers, addresses, ttls);

        var target = new IpAddressTarget("8.8.8.8");
        var behavior = new PingProbeBehavior(target, 64, 500, "This is a test");

        var liaisonConfig = new NetworkProbeLiaison.Configuration() with
        {
            //BaseNetworkProbe = _appFixture.App.Services.GetRequiredService<PingProbe>(),
            BaseNetworkProbe = new PingProbe(pingerStub),
            CancellationTokenSource = new CancellationTokenSource(),
            CrudOperations = crudOperations,
            ServerEventEmitter = _appFixture.App.Services.GetRequiredService<ServerEventEmitter>(),
            ProbeBehavior = behavior,
            ProbeThresholds = new PingProbeThresholds(50),
            ProbeSchedule = new ProbeSchedule(), // presently unused in NetworkProbeLiaison class, change here after used
            ProbeScheduler = _appFixture.App.Services.GetRequiredService<ProbeScheduler>(),
            Logger = NullLogger<NetworkProbeLiaison>.Instance,
        };

        var networkProbeLiaison = new NetworkProbeLiaison(liaisonConfig);
        var testTime = DateTime.UtcNow;
        networkProbeLiaison.StartProbingAsync();
        await Task.Delay(30);
        await networkProbeLiaison.StopProbingAsync();
        Assert.True(true);
    }
}
