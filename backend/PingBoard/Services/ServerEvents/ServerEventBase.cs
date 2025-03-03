namespace PingBoard.Services.ServerEvents;

using System.Text.Json.Serialization;

[JsonDerivedType(typeof(AdminError))]
[JsonDerivedType(typeof(ProbeAnomaly))]
[JsonDerivedType(typeof(ProbeError))]
[JsonDerivedType(typeof(ProbeInfo))]
[JsonDerivedType(typeof(ProbeStatus))]
public record ServerEventBase : IServerEvent
{
    [JsonInclude]
    public Guid EventId { get; }

    [JsonInclude]
    public DateTime EventTime { get; }

    public ServerEventBase()
    {
        EventId = Guid.CreateVersion7();
        EventTime = DateTime.UtcNow;
    }
}
