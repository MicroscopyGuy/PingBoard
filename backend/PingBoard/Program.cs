namespace PingBoard;

using System.Diagnostics.CodeAnalysis;
using PingBoard.Services;

[ExcludeFromCodeCoverage]
public class Program
{
    public static async Task Main(string[] args)
    {
        var tokenSource = new CancellationTokenSource();
        await RunAppAsync(args, tokenSource.Token);
    }

    public static async Task RunAppAsync(
        string[] args,
        CancellationToken cancellationToken = default
    )
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.AddServices();
        /*********************** Build the application ******************/
        var app = builder.Build();
        app.Configure();
        await app.RunAsync(cancellationToken);
    }
}
