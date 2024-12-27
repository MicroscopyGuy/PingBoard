namespace PingBoard;

using System.Diagnostics.CodeAnalysis;
using PingBoard.Services;

[ExcludeFromCodeCoverage]
public class Program
{
    public static async Task Main(string[] args)
    {
        var tokenSource = new CancellationTokenSource();
        RunAppAsync(args, tokenSource.Token);
    }

    public static async void RunAppAsync(
        string[] args,
        CancellationToken cancellationToken = default
    )
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.AddServices();
            /*********************** Build the application ******************/
            var app = builder.Build();
            app.Configure();
            var appRun = app.RunAsync(cancellationToken);
        }
    }
}
