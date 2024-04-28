using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<Pinger>();
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
