namespace PingBoard;

using System.Diagnostics.CodeAnalysis;
using PingBoard.Services;

[ExcludeFromCodeCoverage]
public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.AddServices();
        /*********************** Build the application ******************/
        var app = builder.Build();
        app.Configure();
        app.Run();
    }
}
