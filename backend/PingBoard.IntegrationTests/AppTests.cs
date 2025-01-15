namespace PingBoard.IntegrationTests;

using System.Net.NetworkInformation;
using System.Threading;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Probes.NetworkProbes;
using Probes.NetworkProbes.Common;
using Protos;

public class AppTests
{
    [Fact]
    public void DI_Can_Create_NetworkProbeLiaisonFactory()
    {
        var builder = WebApplication.CreateSlimBuilder();
        builder.AddServices();
        var app = builder.Build();
        var networkProbeLiasonFactory = app.Services.GetRequiredService<
            Func<string, IProbeBehavior, INetworkProbeTarget, NetworkProbeLiaison>
        >();

        Assert.NotNull(networkProbeLiasonFactory);
    }
}
