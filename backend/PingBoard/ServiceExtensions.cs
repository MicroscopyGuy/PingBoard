namespace PingBoard;

using System.Collections.Immutable;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Threading.Channels;
using Database.Models;
using Endpoints;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Monitoring.Configuration;
using PingBoard.Database.Utilities;
using Pinging;
using Pinging.Configuration;
using Probes;
using Serilog;
using Services;

public static class ServiceExtensions
{
    private const string _allowCorsPolicy = "_allowCorsPolicy";
    private const Environment.SpecialFolder _folder = Environment
        .SpecialFolder
        .LocalApplicationData;
    private static string _path = Environment.GetFolderPath(_folder);
    private static string _appDataPath = System.IO.Path.Join(_path, "PingBoard");

    public static void ConfigureWebServer(this WebApplicationBuilder builder)
    {
        builder.WebHost.ConfigureKestrel(
            (c) =>
            {
                c.ListenLocalhost(
                    5245,
                    listenOptions =>
                    {
                        listenOptions.Protocols = HttpProtocols.Http2;
                    }
                );
            }
        );
    }

    public static void AddGrpc(this WebApplicationBuilder builder)
    {
        builder.Services.AddGrpc();
        builder.Services.AddGrpcReflection();
        //builder.Services.AddGrpcHealthChecks()
        //.AddCheck("InsertCheckNameHere", () => HealthCheckResult.Healthy());
    }

    public static void AddServerEventChannels(this WebApplicationBuilder builder)
    {
        var typeNames = Enum.GetNames<ServerEvent.ServerEventOneofCase>().ToHashSet();
        var serverEventTypes = typeof(ServerEvent)
            .GetProperties()
            .Select(prop => prop.PropertyType)
            .Where(pt => typeNames.Contains(pt.Name))
            .ToList();

        foreach (Type serverEventType in serverEventTypes)
        {
            var channel = typeof(Channel)
                .GetMethods(BindingFlags.Static | BindingFlags.Public)
                .Where(m => m.Name.StartsWith("CreateBounded"))
                .Where(m => m.GetGenericArguments().Length == 1)
                .First(m => m.GetParameters()[0].ParameterType == typeof(BoundedChannelOptions))
                .MakeGenericMethod(serverEventType)
                .Invoke(null, new object?[] { new BoundedChannelOptions(100) })!;
            builder.Services.AddSingleton(channel);
        }
    }

    public static void AddServerEventChannelReaders(this WebApplicationBuilder builder)
    {
        builder.Services.AddKeyedSingleton<IImmutableList<IChannelReaderAdapter>>(
            "ServerEventChannelReaders",
            (svc, _) =>
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
                    ),
                    new ChannelReaderAdapter<ServerEvent.Types.DnsAgentError>(
                        svc.GetRequiredService<Channel<ServerEvent.Types.DnsAgentError>>().Reader
                    ),
                    new ChannelReaderAdapter<ServerEvent.Types.DnsAnomaly>(
                        svc.GetRequiredService<Channel<ServerEvent.Types.DnsAnomaly>>().Reader
                    ),
                    new ChannelReaderAdapter<ServerEvent.Types.DnsInfo>(
                        svc.GetRequiredService<Channel<ServerEvent.Types.DnsInfo>>().Reader
                    ),
                    new ChannelReaderAdapter<ServerEvent.Types.DnsOnOffToggle>(
                        svc.GetRequiredService<Channel<ServerEvent.Types.DnsOnOffToggle>>().Reader
                    ),
                    new ChannelReaderAdapter<ServerEvent.Types.TracerouteAgentError>(
                        svc.GetRequiredService<
                            Channel<ServerEvent.Types.TracerouteAgentError>
                        >().Reader
                    ),
                    new ChannelReaderAdapter<ServerEvent.Types.TracerouteAnomaly>(
                        svc.GetRequiredService<
                            Channel<ServerEvent.Types.TracerouteAnomaly>
                        >().Reader
                    ),
                    new ChannelReaderAdapter<ServerEvent.Types.TracerouteInfo>(
                        svc.GetRequiredService<Channel<ServerEvent.Types.TracerouteInfo>>().Reader
                    ),
                    new ChannelReaderAdapter<ServerEvent.Types.TracerouteOnOffToggle>(
                        svc.GetRequiredService<
                            Channel<ServerEvent.Types.TracerouteOnOffToggle>
                        >().Reader
                    ),
                };
                return channelList.ToImmutableList();
            }
        );
    }

    public static void AddServerEventClasses(this WebApplicationBuilder builder)
    {
        builder.AddServerEventChannels();
        builder.AddServerEventChannelReaders();
        builder.Services.AddSingleton<ServerEventEmitter>();
    }

    public static void ConfigureCorsPolicy(this WebApplicationBuilder builder)
    {
        //Enable Cors Support
        var allowCorsPolicy = _allowCorsPolicy;
        builder.Services.AddCors(options =>
        {
            options.AddPolicy(
                name: allowCorsPolicy,
                policy =>
                {
                    /*
                    policy.WithOrigins("http://localhost:5173")
                        .AllowAnyHeader()
                        .AllowAnyMethod();*/
                    policy
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .WithExposedHeaders(
                            "Grpc-Status",
                            "Grpc-Message",
                            "Grpc-Encoding",
                            "Grpc-Accept-Encoding"
                        );
                }
            );
        });
    }

    public static void AddDatabase(this WebApplicationBuilder builder)
    {
        var databaseName = "Summaries";

        if (!Directory.Exists(_appDataPath))
        {
            Directory.CreateDirectory(_appDataPath);
        }
        var dbPath = System.IO.Path.Join(_appDataPath, databaseName);

        var connectionString = new SqliteConnectionStringBuilder()
        {
            Mode = SqliteOpenMode.ReadWriteCreate,
            DataSource = dbPath,
        }.ToString();

        builder.Services.AddDbContextFactory<PingInfoContext>(options =>
            options.UseSqlite(connectionString)
        );

        builder.Services.AddDbContextFactory<ProbeResultsContext>(options =>
            options.UseSqlite(connectionString)
        );

        // Database-related classes
        builder.Services.AddTransient<DatabaseConstants>();
        builder.Services.AddTransient<DatabaseStatementsGenerator>();
        builder.Services.AddTransient<SqliteConnection>();
        builder.Services.AddTransient<CrudOperations>();
    }

    public static void AddPingingClasses(this WebApplicationBuilder builder)
    {
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
            builder.Configuration.GetSection("PingingBehavior")
        );
        builder.Services.Configure<PingingThresholdsConfig>(
            builder.Configuration.GetSection("PingingThresholds")
        );
    }

    public static void AddMonitoringClasses(this WebApplicationBuilder builder)
    {
        builder.Services.AddTransient<MonitoringBehaviorConfig>();
        builder.Services.AddTransient<MonitoringBehaviorConfigLimits>();
        builder.Services.AddTransient<MonitoringBehaviorConfigValidator>();
    }

    public static void AddLogging(this WebApplicationBuilder builder)
    {
        if (Environment.GetEnvironmentVariable("LOG_TO_FILE") == "true")
        {
            builder.Services.AddSerilog(
                (services, lc) =>
                    lc
                        .ReadFrom.Configuration(builder.Configuration)
                        .ReadFrom.Services(services)
                        .Enrich.FromLogContext()
                        .WriteTo.File("serverLogs.txt", rollingInterval: RollingInterval.Day)
                        .WriteTo.Console()
            );
        }
        else
        {
            builder.Logging.AddConsole();
        }
    }

    //public void AddProbesMenu() { }

    /*builder.Services.AddHostedService<PingMonitoringJobManager>((svc)
            => svc.GetRequiredService<PingMonitoringJobManager>());*/


    /*
    INetworkProbeBase GetProbeForOperation(string operationName)
    {
        var svc = new ServiceCollection();
        switch (operationName)
        {
            case ("Ping"):
                return svc.BuildServiceProvider().GetService<PingProbe>() ?? throw new NullReferenceException("PingProbe doesn't exist");
            
            case ("Dns"):
                return svc.BuildServiceProvider().GetService<DnsProbe>() ??
                       throw new NullReferenceException("DnsProbe doesn't exist");
            case ("Traceroute"):
                return svc.BuildServiceProvider().GetService<TracerouteProbe>() ??
                       throw new NullReferenceException("TracerouteProbe doesn't exist");
        }
    }*/
    /*
     builder.Services.AddTransient<Func<string, INetworkProbeTarget, NetworkProbeLiason>>((svc) =>
     {
         return (str, networkProbeTarget) =>
         {
             switch (str)
             {
                 case "Ping":
                     
             }

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
     }); */


    /******************* Web Application related extensions *******************/

    public static void UsePermissiveCorsPolicy(this WebApplication app)
    {
        app.UseCors(_allowCorsPolicy);
    }

    public static void ConfigureHttpRequestPipeline(this WebApplication app)
    {
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            //app.UseSwagger();
            //app.UseSwaggerUI();
            app.MapGrpcReflectionService();
        }
    }

    public static void UseGrpc(this WebApplication app)
    {
        app.MapGrpcService<Services.PingBoardService>();
        app.MapGet(
            "/",
            () =>
                "This gRPC service is gRPC-Web enabled and is callable from browser apps using the gRPC-Web protocol"
        );
    }
}
