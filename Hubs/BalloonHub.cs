using Microsoft.AspNetCore.SignalR;
using NucpaBalloonsApi.Models.DTOs;

namespace NucpaBalloonsApi.Hubs;

public class BalloonHub : Hub
{
    public async Task UpdateBalloons(List<BalloonRequestDTO> balloons)
    {
        await Clients.All.SendAsync("ReceiveBalloonUpdates", balloons);
    }

    public async Task UpdateStatus()
    {
        await Clients.All.SendAsync("BalloonStatusChanged");
    }
} 