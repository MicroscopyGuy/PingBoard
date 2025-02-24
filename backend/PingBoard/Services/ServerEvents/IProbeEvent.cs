namespace PingBoard.Services.ServerEvents;

public interface IProbeEvent : IServerEvent
{
    Guid ProbeId { get; set; }
    string ProbeType { get; set; }

    string Target { get; set; }
}
