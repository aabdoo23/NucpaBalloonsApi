using Microsoft.AspNetCore.SignalR;
using NucpaBalloonsApi.Models.DTOs;

namespace NucpaBalloonsApi.Hubs;

public class BalloonHub : Hub
{
    public async Task UpdateBalloons(object updates)
    {
        Console.WriteLine(updates.ToString());
        await Clients.All.SendAsync("ReceiveBalloonUpdates", updates);
    }

    public async Task UpdateStatus(object updates)
    {
        Console.WriteLine(updates.ToString());
        await Clients.All.SendAsync("BalloonStatusChanged", updates);
    }
} 