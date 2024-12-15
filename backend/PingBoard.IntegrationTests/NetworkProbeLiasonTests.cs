using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using PingBoard.Database.Models;
using PingBoard.Database.Utilities;
using PingBoard.Pinging.Configuration;
using PingBoard.Probes;
using PingBoard.Probes.NetworkProbes;
using PingBoard.TestUtilities.PingingTestingUtilities;

namespace PingBoard.IntegrationTests;
using PingBoard.Services;

public class NetworkProbeLiasonTests
{
    
    public async Task NetworkProbeLiason_CanProbe_WithPingProbe()
    {

        var pingProbe = new PingProbe(
            new IndividualPingerStub()
        );

        var services = new ServiceCollection();
        var serviceProvider = services.BuildServiceProvider();

        // Get the factory and create a context
        var probeContextFactory = serviceProvider.GetRequiredService<IDbContextFactory<ProbeResultsContext>>();
        var pingInfoFactory = serviceProvider.GetRequiredService<IDbContextFactory<PingInfoContext>>(); // will be removed soon, added here for compatibility

        PingingThresholdsConfig testThresholdsConfig = new PingingThresholdsConfig(){
            MinimumPingMs = 50,
            AveragePingMs = 60,
            MaximumPingMs = 100,
            JitterMs      = 15,
            PacketLossPercentage = 0
        };
            
        IOptions<PingingThresholdsConfig> testThresholdOptions = Options.Create(testThresholdsConfig);
        
        var crudOperations = new CrudOperations(
            pingInfoFactory,
            probeContextFactory,
            testThresholdOptions,
            NullLogger<CrudOperations>.Instance
        );

        var target = new IpOrHostnameTarget("google.com", "8.8.8.8");
        
        var networkProbeLiason = new NetworkProbeLiason(
            pingProbe,
            crudOperations,
            new CancellationTokenSource(),
            serviceProvider.GetRequiredService<ServerEventEmitter>(),
            target,
            new NullLogger<NetworkProbeLiason>()
        );

        var testTime = DateTime.UtcNow;
        await networkProbeLiason.StartProbingAsync();
        await Task.Delay(20);

    }
}