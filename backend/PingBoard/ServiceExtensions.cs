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
using PingBoard.Database.Utilities;
using Pinging;
using Probes;
using Probes.NetworkProbes;
using Probes.NetworkProbes.Ping;
using Protos;
using Serilog;
using Services;

public static class ServiceExtensions
{
    private const string _allowCorsPolicy = "_allowCorsPolicy";
    private const Environment.SpecialFolder _folder = Environment
        .SpecialFolder
        .LocalApplicationData;

    private static readonly string _path = Environment.GetFolderPath(_folder);
    private static readonly string _appDataPath = System.IO.Path.Join(_path, "PingBoard");
    private static readonly List<Type> _probeTypes = typeof(Program)
        .Assembly.GetTypes()
        .Where(T => T.IsAssignableTo(typeof(INetworkProbeBase)))
        .Where(T => T.IsClass)
        .ToList();

    private static List<string> _probeNames = _probeTypes
        .Select(T => T.GetProperty("Name"))
        .Select(p => p.GetValue(null) as string)
        .Where(i => i != null)
        .ToList();

    private static string ProbeName(this Type probeType)
    {
        return probeType.GetProperty("Name").GetValue(null) as string;
    }

    private static readonly Dictionary<string, Type> _probes = _probeTypes.ToDictionary(
        pt => pt.ProbeName(),
        pt => pt
    );

    public static void ConfigureWebServer(this WebApplicationBuilder builder)
    {
        builder.WebHost.ConfigureKestrel(
            (c) =>
            {
                c.ListenLocalhost(
                    Int32.Parse(Environment.GetEnvironmentVariable("SERVER_PORT")!),
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

            builder.Services.AddSingleton(
                typeof(Channel<>).MakeGenericType(serverEventType),
                channel
            );
        }
    }

    /*
    public static void AddServerEventChannels(this WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<Channel<ServerEvent.Types.PingAgentError>>();
        builder.Services.AddSingleton<Channel<ServerEvent.Types.PingAnomaly>>();
        builder.Services.AddSingleton<Channel<ServerEvent.Types.PingInfo>>();
        builder.Services.AddSingleton<Channel<ServerEvent.Types.DnsAgentError>>();
        builder.Services.AddSingleton<Channel<ServerEvent.Types.DnsAnomaly>>();
        builder.Services.AddSingleton<Channel<ServerEvent.Types.DnsInfo>>();
        builder.Services.AddSingleton<Channel<ServerEvent.Types.TracerouteAgentError>>();
        builder.Services.AddSingleton<Channel<ServerEvent.Types.TracerouteAnomaly>>();
        builder.Services.AddSingleton<Channel<ServerEvent.Types.TracerouteInfo>>();
    }(*/

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

        builder.Services.AddDbContextFactory<ProbeResultsContext>(options =>
            options.UseSqlite(connectionString)
        );

        // Database-related classes
        builder.Services.AddTransient<DatabaseConstants>();
        builder.Services.AddTransient<SqliteConnection>();
        builder.Services.AddTransient<CrudOperations>();
    }

    public static void AddPingingClasses(this WebApplicationBuilder builder)
    {
        builder.Services.AddTransient<IIndividualPinger, IndividualPinger>();
        builder.Services.AddTransient<Ping>();
        builder.Services.AddTransient<IProbeScheduler, ProbeScheduler>();
    }

    public static void AddProbes(this WebApplicationBuilder builder)
    {
        builder.Services.AddTransient<PingProbe>();
        //builder.Services.AddTransient<DnsProbe>();
        //builder.Services.AddTransient<TracerouteProbe>();
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

    public static void AddNetworkProbeLiaisonFactory(this WebApplicationBuilder builder)
    {
        builder.Services.AddTransient<Func<string, IProbeInvocationParams, NetworkProbeLiaison>>(
            (svc) =>
            {
                return (string probeName, IProbeInvocationParams invocParams) =>
                {
                    var crudOperations = svc.GetRequiredService<CrudOperations>();
                    var cancellationTokenSource = new CancellationTokenSource();
                    var serverEventEmitter = svc.GetRequiredService<ServerEventEmitter>();
                    var probeScheduler = svc.GetRequiredService<ProbeScheduler>();
                    var logger = svc.GetRequiredService<Logger<NetworkProbeLiaison>>();
                    logger.LogDebug(
                        "Program.cs: Registering PingMonitoringJobRunner factory method: CancellationTokenSourceHash: {ctsHash}",
                        cancellationTokenSource.GetHashCode()
                    );

                    return new NetworkProbeLiaison(
                        (svc.GetRequiredService(_probes[probeName]) as INetworkProbeBase)!,
                        crudOperations,
                        new CancellationTokenSource(),
                        serverEventEmitter,
                        invocParams,
                        probeScheduler,
                        logger
                    );
                };
            }
        );
    }

    public static void AddServiceLayerTypes(this WebApplicationBuilder builder)
    {
        //builder.Services.AddSingleton<NetworkProbeManager>();
        builder.Services.AddSingleton<ProbeOperationsCenter>();
        builder.AddNetworkProbeLiaisonFactory();
    }

    public static void AddServices(this WebApplicationBuilder builder)
    {
        builder.ConfigureWebServer();
        builder.AddGrpc();
        builder.AddPingingClasses();
        builder.AddProbes();
        builder.AddDatabase();
        builder.AddServerEventClasses();
        builder.AddServiceLayerTypes();
        builder.ConfigureCorsPolicy();
        builder.AddLogging();
    }

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

    public static void Configure(this WebApplication app)
    {
        app.UsePermissiveCorsPolicy();
        app.ConfigureHttpRequestPipeline();
        app.UseGrpc();
    }
}
