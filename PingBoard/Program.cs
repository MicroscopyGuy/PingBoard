using System.Diagnostics;
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

        // Database-related classes
        builder.Services.AddTransient<DatabaseConstants>();
        builder.Services.AddTransient<DatabaseStatementsGenerator>();
        builder.Services.AddTransient<DatabaseHelper>();
        builder.Services.AddTransient<SqliteConnection>();

        // Service-related information
        //builder.Services.AddHostedService<PingMonitoringService>();
        builder.Services.AddSingleton<PingMonitoringJobManager>();
        builder.Services.AddHostedService<PingMonitoringJobManager>((svc)
            => svc.GetRequiredService<PingMonitoringJobManager>());

        //builder.Services.AddTransient<CancellationTokenSource>();
        builder.Services.AddSingleton<Func<string, PingMonitoringJobRunner>>((svc) =>
        {
            return (str) =>
            {
                var groupPinger = svc.GetRequiredService<IGroupPinger>();
                var pingingBehavior = svc.GetRequiredService<IOptions<PingingBehaviorConfig>>();
                var pingingThresholds = svc.GetRequiredService<IOptions<PingingThresholdsConfig>>();
                var behaviorConfigValidator = svc.GetRequiredService<PingingBehaviorConfigValidator>();
                var pingingThresholdsConfigValidator = svc.GetRequiredService<PingingThresholdsConfigValidator>();
                var databaseHelper = svc.GetRequiredService<DatabaseHelper>();
                var cancellationTokenSource = new CancellationTokenSource();
                var logger = svc.GetRequiredService<ILogger<IGroupPinger>>();
                logger.LogDebug(
                    "Program.cs: Registering PingMonitoringJobRunner factory method: CancellationTokenSourceHash: {ctsHash}",
                    cancellationTokenSource.GetHashCode());

                return new PingMonitoringJobRunner(
                    groupPinger, pingingBehavior, pingingThresholds,
                    behaviorConfigValidator, pingingThresholdsConfigValidator, databaseHelper,
                    new CancellationTokenSource(), str, logger);
            };
        });

        //Enable Cors Support
        var allowCorsPolicy = "_allowCorsPolicy";
        builder.Services.AddCors(options =>
        {
            options.AddPolicy(name: allowCorsPolicy,
                policy =>
                {
                    policy.WithOrigins("http://localhost:5173")
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
        });
        
        builder.Logging.AddConsole();
        /***********************************************Build the application *****************************************/
        var app = builder.Build();
        app.UseCors(allowCorsPolicy);
        
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        //app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();

    }
}
