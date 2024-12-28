namespace PingBoard.IntegrationTests;

using System.Net.NetworkInformation;
using System.Threading;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Probes.NetworkProbes;
using Protos;

public class AppTests
{
    [Fact]
    public void DI_Can_Create_NetworkProbeLiasonFactory()
    {
        var builder = WebApplication.CreateSlimBuilder();
        builder.AddServices();
        var app = builder.Build();
        var networkProbeLiasonFactory = app.Services.GetRequiredService<
            Func<string, IProbeInvocationParams, INetworkProbeTarget, NetworkProbeLiason>
        >();

        Assert.NotNull(networkProbeLiasonFactory);
    }
}
