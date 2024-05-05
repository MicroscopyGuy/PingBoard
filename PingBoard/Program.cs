using System.Net.NetworkInformation;
using Microsoft.Extensions.Options;
using Pingboard.Pinging;
using PingBoard.Pinging;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<IGroupPinger, GroupPinger>();
builder.Services.AddTransient<PingQualification>();
builder.Services.AddTransient<IIndividualPinger, IndividualPinger>();
builder.Services.AddTransient<Ping>();
builder.Services.AddTransient<PingOptions>();
builder.Services.AddHostedService<NetworkMonitoringService>();

builder.Services.Configure<PingBoard.Pinging.PingingBehaviorConfig>(
    builder.Configuration.GetSection("PingingBehavior"));

builder.Services.Configure<PingBoard.Pinging.PingingThresholdsConfig>(
    builder.Configuration.GetSection("PingingThresholds"));


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
