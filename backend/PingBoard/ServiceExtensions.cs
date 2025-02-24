namespace PingBoard;

using System.Collections.Immutable;
using System.Net.NetworkInformation;
using System.Threading.Channels;
using Apis.Probes;
using Database.Models;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using PingBoard.Database.Utilities;
using Pinging;
using Probes;
using Probes.NetworkProbes;
using Probes.NetworkProbes.Common;
using Probes.NetworkProbes.Ping;
using Probes.Utilities;
using Scalar.AspNetCore;
using Serilog;
using Services;
using Services.ServerEvents;

public static class ServiceExtensions
{
    private const string _allowCorsPolicy = "_allowCorsPolicy";
    private const Environment.SpecialFolder _folder = Environment
        .SpecialFolder
        .LocalApplicationData;

    private static readonly string _path = Environment.GetFolderPath(_folder);
    public static readonly string PingBoardFiles = System.IO.Path.Join(_path, "PingBoard");
    public static readonly string DatabasePath = System.IO.Path.Join(
        PingBoardFiles,
        DatabaseConstants.DatabaseName
    );
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

    // csharpier-ignore-start
    public static void ConfigureWebServer(this WebApplicationBuilder builder)
    {
        builder.WebHost.ConfigureKestrel(
            (c) =>
            {
                c.ListenLocalhost(
                    int.TryParse(Environment.GetEnvironmentVariable("SERVER_PORT"), out var servPort)
                        ? servPort
                        : 5245,
                    listenOptions =>
                    {
                        listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
                    }
                );
            }
        );
    }

    public static void ConfigureHttpJson(this WebApplicationBuilder builder)
    {
        builder.Services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
        });
    }
    // csharpier-ignore-end

    public static void AddServerEventChannels(this WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<Channel<IServerEvent>>();
    }

    public static void AddServerEventClasses(this WebApplicationBuilder builder)
    {
        builder.AddServerEventChannels();
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
        if (!Directory.Exists(PingBoardFiles))
        {
            Directory.CreateDirectory(PingBoardFiles);
        }

        var connectionString = new SqliteConnectionStringBuilder()
        {
            Mode = SqliteOpenMode.ReadWriteCreate,
            DataSource = DatabasePath,
        };

        builder.Services.AddSingleton<SqliteConnectionStringBuilder>(connectionString);

        builder.Services.AddDbContextFactory<ProbeResultsContext>(options =>
            options.UseSqlite(connectionString.ConnectionString)
        );

        // Database-related classes
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
        builder.Services.AddTransient<ProbeScheduler>();
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

    public static void AddOpenApi(this WebApplicationBuilder builder)
    {
        builder.Services.AddOpenApi();
    }

    public static void AddEventHandling(this WebApplicationBuilder builder)
    {
        builder.Services.AddSignalR();
    }

    // csharpier-ignore-start
    public static void AddNetworkProbeLiaisonFactory(this WebApplicationBuilder builder)
    {
        builder.Services.AddTransient<Logger<NetworkProbeLiaison>>();
        builder.Services.AddTransient<Logger<PingProbe>>();

        builder.Services.AddTransient<
            Func<ProbeConfigAggregate, NetworkProbeLiaison>>((svc) =>
            {
                return (ProbeConfigAggregate probeConfig) =>
                {
                    var liaisonConfig = new NetworkProbeLiaison.Configuration() with
                    {
                        BaseNetworkProbe = (svc.GetRequiredService(_probes[probeConfig.ProbeType]) as INetworkProbeBase)!,
                        CancellationTokenSource = new CancellationTokenSource(),
                        CrudOperations = svc.GetRequiredService<CrudOperations>(),
                        ServerEventEmitter = svc.GetRequiredService<ServerEventEmitter>(),
                        ProbeBehavior = probeConfig.Behavior,
                        ProbeThresholds = probeConfig.Thresholds,
                        ProbeSchedule = probeConfig.Schedule,
                        ProbeScheduler = svc.GetRequiredService<ProbeScheduler>(),
                        Logger = svc.GetRequiredService<Logger<NetworkProbeLiaison>>()
                    };

                    return new NetworkProbeLiaison(liaisonConfig);
                };
            }
        );
    }
    // csharpier-ignore-end

    public static void AddServiceLayerTypes(this WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<ProbeOperationsCenter>();
        builder.AddNetworkProbeLiaisonFactory();
    }

    public static void AddServices(this WebApplicationBuilder builder)
    {
        builder.ConfigureWebServer();
        builder.ConfigureHttpJson();
        builder.AddPingingClasses();
        builder.AddProbes();
        builder.AddDatabase();
        builder.AddServerEventClasses();
        builder.AddServiceLayerTypes();
        builder.ConfigureCorsPolicy();
        builder.AddLogging();
        builder.AddOpenApi();
        builder.AddEventHandling();
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
        }
    }

    public static void UseOpenApi(this WebApplication app)
    {
        app.MapOpenApi();
        app.MapScalarApiReference();
    }

    public static void Configure(this WebApplication app)
    {
        app.UsePermissiveCorsPolicy();
        app.ConfigureHttpRequestPipeline();
        app.UseOpenApi();
        app.MapProbingApis();
    }
}
