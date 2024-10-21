using System.Collections.Immutable;
using System.Threading.Channels;

namespace PingBoard;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PingBoard.Database.Models;
using PingBoard.Database.Utilities;
using PingBoard.Pinging;
using PingBoard.Monitoring.Configuration;
using PingBoard.Pinging.Configuration;
using PingBoard.Services;
using PingBoard.Endpoints;
using System.Diagnostics.CodeAnalysis;
using System.Net.NetworkInformation;


[ExcludeFromCodeCoverage]
public class Program
{
    public static void Main(string[] args)
    {
        
        
        
        var builder = WebApplication.CreateBuilder(args);
        builder.WebHost.ConfigureKestrel((c) =>
        {
            c.ListenLocalhost(5245);
        });
        /* commented out to move from Rest to GRPC API design
        // Add services to the container.
        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        */
        builder.Services.AddGrpc();
        builder.Services.AddGrpcReflection();
        //builder.Services.AddGrpcHealthChecks()
            //.AddCheck("InsertCheckNameHere", () => HealthCheckResult.Healthy());
        
        
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
        builder.Services.AddTransient<CrudOperations>();

        // Service-related information
        builder.Services.AddSingleton<DatabaseInitializer>();
        builder.Services.AddHostedService<DatabaseInitializer>((svc) =>
            svc.GetRequiredService<DatabaseInitializer>());
        
        builder.Services.AddSingleton<Channel<ServerEvent.Types.PingOnOffToggle>>(
            Channel.CreateBounded<ServerEvent.Types.PingOnOffToggle>(
                new BoundedChannelOptions(100))
        );
        
        builder.Services.AddSingleton<Channel<ServerEvent.Types.PingAnomaly>>(
            Channel.CreateBounded<ServerEvent.Types.PingAnomaly>(
                new BoundedChannelOptions(100)
            )
        );
        
        builder.Services.AddSingleton<Channel<ServerEvent.Types.PingAgentError>>(
            Channel.CreateBounded<ServerEvent.Types.PingAgentError>(
                new BoundedChannelOptions(100)
            )
        );

        builder.Services.AddKeyedSingleton<IImmutableList<IChannelReaderAdapter>>("ServerEventChannelReaders", (svc, _) =>
        {
            var channelList = new List<IChannelReaderAdapter>
            {
                new ChannelReaderAdapter<ServerEvent.Types.PingOnOffToggle>(
                    svc.GetRequiredService<Channel<ServerEvent.Types.PingOnOffToggle>>().Reader
                ),
                new ChannelReaderAdapter<ServerEvent.Types.PingAnomaly>(
                    svc.GetRequiredService<Channel<ServerEvent.Types.PingAnomaly>>().Reader
                ),
                new ChannelReaderAdapter<ServerEvent.Types.PingAgentError>(
                    svc.GetRequiredService<Channel<ServerEvent.Types.PingAgentError>>().Reader
                )
            };
            return channelList.ToImmutableList();
        });
          
        
        builder.Services.AddSingleton<ServerEventEmitter>();
        
        builder.Services.AddSingleton<PingMonitoringJobManager>();
        builder.Services.AddHostedService<PingMonitoringJobManager>((svc)
            => svc.GetRequiredService<PingMonitoringJobManager>());
        
        
        
        builder.Services.AddSingleton<Func<string, PingMonitoringJobRunner>>((svc) =>
        {
            return (str) =>
            {
                var groupPinger = svc.GetRequiredService<IGroupPinger>();
                var pingingBehavior = svc.GetRequiredService<IOptions<PingingBehaviorConfig>>();
                var pingingThresholds = svc.GetRequiredService<IOptions<PingingThresholdsConfig>>();
                var behaviorConfigValidator = svc.GetRequiredService<PingingBehaviorConfigValidator>();
                var pingingThresholdsConfigValidator = svc.GetRequiredService<PingingThresholdsConfigValidator>();
                var crudOperations = svc.GetRequiredService<CrudOperations>();
                var pingQualifier = svc.GetRequiredService<PingQualification>();
                var cancellationTokenSource = new CancellationTokenSource();
                var serverEventEmitter = svc.GetRequiredService<ServerEventEmitter>();
                var logger = svc.GetRequiredService<ILogger<PingMonitoringJobRunner>>();
                logger.LogDebug(
                    "Program.cs: Registering PingMonitoringJobRunner factory method: CancellationTokenSourceHash: {ctsHash}",
                    cancellationTokenSource.GetHashCode());

                return new PingMonitoringJobRunner(
                    groupPinger, pingingBehavior, pingingThresholds,
                    behaviorConfigValidator, pingingThresholdsConfigValidator, crudOperations,
                    pingQualifier, new CancellationTokenSource(), serverEventEmitter, str, logger);
            };
        });

        //EFCore Framework related information
        var databaseName = "Summaries";
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        var appDataPath = System.IO.Path.Join(path, "PingBoard");
        
        
        if (!Directory.Exists(appDataPath))
        {
            Directory.CreateDirectory(appDataPath);
        }
        var dbPath = System.IO.Path.Join(appDataPath, databaseName);
        

        var connectionString = new SqliteConnectionStringBuilder()
        {
            Mode = SqliteOpenMode.ReadWriteCreate,
            DataSource = dbPath
        }.ToString();
        
        builder.Services.AddDbContextFactory<PingInfoContext>(
            options =>
                options.UseSqlite(connectionString));
        
        
        //Enable Cors Support
        var allowCorsPolicy = "_allowCorsPolicy";
        builder.Services.AddCors(options =>
        {
            options.AddPolicy(name: allowCorsPolicy, 
                policy =>
                {
                    /*
                    policy.WithOrigins("http://localhost:5173")
                        .AllowAnyHeader()
                        .AllowAnyMethod();*/
                    policy.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .WithExposedHeaders("Grpc-Status", "Grpc-Message", "Grpc-Encoding", "Grpc-Accept-Encoding");
                });
        });
        
        builder.Logging.AddConsole();
        /***********************************************Build the application *****************************************/
        var app = builder.Build();
        app.UseCors(allowCorsPolicy);
        
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            //app.UseSwagger();
            //app.UseSwaggerUI();
            app.MapGrpcReflectionService();
            
            
        }

        //app.UseHttpsRedirection();

        //app.UseAuthorization();

        //app.MapControllers();
        app.UseGrpcWeb(new GrpcWebOptions { DefaultEnabled = true });
        app.MapGrpcService<Services.PingBoardService>();
        app.MapGet("/", () => "This gRPC service is gRPC-Web enabled and is callable from browser apps using the gRPC-Web protocol");
        
        
        app.Run();

    }
}
