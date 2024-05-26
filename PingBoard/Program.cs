using System.Net.NetworkInformation;
using FluentValidation;
using Microsoft.Extensions.Options;
using Pingboard.Pinging;
using PingBoard.Monitoring.Configuration;
using PingBoard.Pinging;
using PingBoard.Pinging.Configuration;

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
builder.Services.AddTransient<PingScheduler>();

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
builder.Services.AddHostedService<NetworkMonitoringService>();


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
