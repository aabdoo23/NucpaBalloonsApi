using Microsoft.AspNetCore.SignalR;

namespace NucpaBalloonsApi.Hubs;

public class BalloonHub : Hub
{
    public async Task UpdateBalloons(object updates)
    {
        await Clients.All.SendAsync("ReceiveBalloonUpdates", updates);
    }

    public async Task UpdateStatus(object updates)
    {
        await Clients.All.SendAsync("BalloonStatusChanged", updates);
    }

    public async Task UpdateToiletRequests(object updates)
    {
        await Clients.All.SendAsync("ReceiveToiletRequestUpdates", updates);
    }

    public async Task SendAnnouncement(string message)
    {
        await Clients.All.SendAsync("ReceiveAnnouncement", message);
    }
}