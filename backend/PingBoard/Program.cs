namespace PingBoard;

using System.Diagnostics.CodeAnalysis;
using PingBoard.Services;

[ExcludeFromCodeCoverage]
public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.ConfigureWebServer();
        builder.AddGrpc();
        builder.AddPingingClasses();
        builder.AddMonitoringClasses();
        builder.AddProbes();
        builder.AddDatabase();
        builder.AddServerEventClasses();
        builder.Services.AddSingleton<PingMonitoringJobManager>();
        builder.ConfigureCorsPolicy();
        builder.AddLogging();

        /*********************** Build the application ******************/
        var app = builder.Build();
        app.UsePermissiveCorsPolicy();
        app.ConfigureHttpRequestPipeline();
        app.UseGrpc();
        app.Run();
    }
}
