namespace PingBoard.Services.ServerEvents;

using Microsoft.AspNetCore.SignalR;

public class ServerEventHub : Hub<IServerEventReceiver>
{
    public async Task SendServerEventToGroup(string eventDetails) =>
        await Clients.Group("ServerEventListeners").ReceiveServerEvent(eventDetails);
}
