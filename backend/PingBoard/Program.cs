using System.Collections.Immutable;
using System.Reflection;
using System.Text.Json;
using System.Threading.Channels;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore.Internal;
using PingBoard.Probes;
using Serilog;

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
            c.ListenLocalhost(5245, listenOptions =>
            {
                listenOptions.Protocols = HttpProtocols.Http2;
            });
        }); 
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        var appDataPath = System.IO.Path.Join(path, "PingBoard");
        
        builder.Services.AddGrpc();
        builder.Services.AddGrpcReflection();
        //builder.Services.AddGrpcHealthChecks()
            //.AddCheck("InsertCheckNameHere", () => HealthCheckResult.Healthy());
        
        
        // Pinging-related classes
        //builder.Services.AddTransient<IGroupPinger, GroupPinger>();
        builder.Services.AddTransient<PingGroupQualifier>();
        builder.Services.AddTransient<IIndividualPinger, IndividualPinger>();
        builder.Services.AddTransient<Ping>();
        builder.Services.AddTransient<IProbeScheduler, ProbeScheduler>();

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
        builder.Services.AddTransient<SqliteConnection>();
        builder.Services.AddTransient<CrudOperations>();

        // Probe-related information
        //builder.Services.AddTransient<INetworkProbe, >
        
        // Service-related information

        void RegisterServerEventChannels()
        {
            var typeNames = Enum.GetNames<ServerEvent.ServerEventOneofCase>().ToHashSet();
            var serverEventTypes = typeof(ServerEvent).GetProperties()
                .Select(prop => prop.PropertyType)
                .Where(pt => typeNames.Contains(pt.Name))
                .ToList();
            
            foreach (Type serverEventType in serverEventTypes)
            {
                var channel = typeof(Channel).GetMethods(BindingFlags.Static | BindingFlags.Public)
                    .Where(m => m.Name.StartsWith("CreateBounded"))
                    .Where(m => m.GetGenericArguments().Length == 1)
                    .First(m => m.GetParameters()[0].ParameterType == typeof(BoundedChannelOptions))
                    .MakeGenericMethod(serverEventType)
                    .Invoke(null, new object?[]
                    {
                        new BoundedChannelOptions(100)
                    });
            }
            /*
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
            
            builder.Services.AddSingleton<Channel<ServerEvent.Types.PingInfo>>(
                Channel.CreateBounded<ServerEvent.Types.PingInfo>(
                    new BoundedChannelOptions(100)
                )
            );*/
        }

        RegisterServerEventChannels();
        

        builder.Services.AddKeyedSingleton<IImmutableList<IChannelReaderAdapter>>("ServerEventChannelReaders", (svc, _) =>
        {
            var channelList = new List<IChannelReaderAdapter>
            {
                new ChannelReaderAdapter<ServerEvent.Types.PingAgentError>(
                    svc.GetRequiredService<Channel<ServerEvent.Types.PingAgentError>>().Reader
                ),
                
                new ChannelReaderAdapter<ServerEvent.Types.PingAnomaly>(
                    svc.GetRequiredService<Channel<ServerEvent.Types.PingAnomaly>>().Reader
                ),
                
                new ChannelReaderAdapter<ServerEvent.Types.PingInfo>(
                    svc.GetRequiredService<Channel<ServerEvent.Types.PingInfo>>().Reader
                ),
                
                new ChannelReaderAdapter<ServerEvent.Types.PingOnOffToggle>(
                    svc.GetRequiredService<Channel<ServerEvent.Types.PingOnOffToggle>>().Reader
                )
            };
            return channelList.ToImmutableList();
        });
          
        
        builder.Services.AddSingleton<ServerEventEmitter>();
        
        builder.Services.AddSingleton<PingMonitoringJobManager>();
        /*builder.Services.AddHostedService<PingMonitoringJobManager>((svc)
            => svc.GetRequiredService<PingMonitoringJobManager>());*/
        
        /*
        builder.Services.AddTransient<Func<string, IndividualPingMonitoringJobRunner>>((svc) =>
        {
            return (str) =>
            {
                var groupPinger = svc.GetRequiredService<IIndividualPinger>();
                var pingingBehavior = svc.GetRequiredService<IOptions<PingingBehaviorConfig>>();
                var pingingThresholds = svc.GetRequiredService<IOptions<PingingThresholdsConfig>>();
                var behaviorConfigValidator = svc.GetRequiredService<PingingBehaviorConfigValidator>();
                var pingingThresholdsConfigValidator = svc.GetRequiredService<PingingThresholdsConfigValidator>();
                var crudOperations = svc.GetRequiredService<CrudOperations>();
                var pingQualifier = svc.GetRequiredService<PingGroupQualifier>();
                var cancellationTokenSource = new CancellationTokenSource();
                var serverEventEmitter = svc.GetRequiredService<ServerEventEmitter>();
                var logger = svc.GetRequiredService<ILogger<GroupPinger>>();
                logger.LogDebug(
                    "Program.cs: Registering PingMonitoringJobRunner factory method: CancellationTokenSourceHash: {ctsHash}",
                    cancellationTokenSource.GetHashCode());

                return new GroupPinger(
                    groupPinger, pingingBehavior, pingingThresholds,
                    behaviorConfigValidator, pingingThresholdsConfigValidator, crudOperations,
                    pingQualifier, new CancellationTokenSource(), serverEventEmitter, str, logger);
            };
        });*/

        //EFCore Framework related information
        var databaseName = "Summaries";
        
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

        builder.Services.AddDbContextFactory<ProbeResultsContext>(
            options =>
                options.UseSqlite(connectionString));

        // ONLY used for testing
        builder.Services.AddTransient<Func<IDbContextFactory<ProbeResultsContext>>>((svc) =>
        {
            var dbContextFactory = svc.GetRequiredService<IDbContextFactory<ProbeResultsContext>>();
    
            // Return a lambda that provides this factory
            return () => dbContextFactory;
        });
        
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
        
        //builder.Logging.AddConsole();

        if (Environment.GetEnvironmentVariable("LOG_TO_FILE") == "true")
        {
            builder.Services.AddSerilog((services, lc) => lc
                .ReadFrom.Configuration(builder.Configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext()
                .WriteTo.File("serverLogs.txt", rollingInterval: RollingInterval.Day)
                .WriteTo.Console());
        }
        else
        {
            builder.Logging.AddConsole();
        }
        
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
        app.MapGrpcService<Services.PingBoardService>();
        app.MapGet("/", () => "This gRPC service is gRPC-Web enabled and is callable from browser apps using the gRPC-Web protocol");
        
        
        app.Run();

    }
}