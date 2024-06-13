using Microsoft.Extensions.Options;

namespace PingBoard;
using Microsoft.Data.Sqlite;
using PingBoard.Pinging;
using PingBoard.Monitoring.Configuration;
using PingBoard.Pinging.Configuration;
using PingBoard.DatabaseUtilities;
using PingBoard.Services;
using System.Diagnostics.CodeAnalysis;
using System.Net.NetworkInformation;



[ExcludeFromCodeCoverage]
public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        // Pinging-related classes
        builder.Services.AddTransient<IGroupPinger, GroupPinger>();
        builder.Services.AddTransient<PingQualification>();
        builder.Services.AddTransient<IIndividualPinger, IndividualPinger>();
        builder.Services.AddTransient<Ping>();
        builder.Services.AddTransient<PingOptions>();
        builder.Services.AddTransient<IPingScheduler, PingScheduler>();

        // Pinging configuration-related classes
        builder.Services.AddTransient<PingingBehaviorConfigValidator>();
        builder.Services.AddTransient<PingingThresholdsConfigValidator>();
        builder.Services.AddTransient<PingingBehaviorConfigLimits>();
        builder.Services.AddTransient<PingingThresholdsConfigLimits>();
        builder.Services.Configure<PingingBehaviorConfig>(
            builder.Configuration.GetSection("PingingBehavior"));

        builder.Services.Configure<PingingThresholdsConfig>(
            builder.Configuration.GetSection("PingingThresholds"));

        // Monitoring configuration-related classes
        builder.Services.AddTransient<MonitoringBehaviorConfig>();
        builder.Services.AddTransient<MonitoringBehaviorConfigLimits>();
        builder.Services.AddTransient<MonitoringBehaviorConfigValidator>();
        builder.Services.AddHostedService<PingMonitoringService>();

        // Database-related classes
        builder.Services.AddTransient<DatabaseConstants>();
        builder.Services.AddTransient<DatabaseStatementsGenerator>();
        builder.Services.AddTransient<DatabaseHelper>();
        builder.Services.AddTransient<SqliteConnection>();

        // Service-related information
        builder.Services.AddTransient<Func<PingMonitoringService>>((svc) => {
                var type1 = svc.GetRequiredService<IGroupPinger>();
                var type2 = svc.GetRequiredService<IOptions<PingingBehaviorConfig>>();
                var type3 = svc.GetRequiredService<IOptions<PingingThresholdsConfig>>();
                var type4 = svc.GetRequiredService<PingingBehaviorConfigValidator>();
                var type5 = svc.GetRequiredService<PingingThresholdsConfigValidator>();
                var type6 = svc.GetRequiredService<DatabaseHelper>();
                var type7 = svc.GetRequiredService<ILogger<IGroupPinger>>();

                return () => new PingMonitoringService(type1, type2, type3, type4, type5, type6, type7);
            });
        
        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();

    }
}
