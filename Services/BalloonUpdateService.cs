using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using NucpaBalloonsApi.Hubs;
using NucpaBalloonsApi.Interfaces.Services;
using NucpaBalloonsApi.Models.DTOs;

namespace NucpaBalloonsApi.Services;

public class BalloonUpdateService : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<BalloonUpdateService> _logger;

    public BalloonUpdateService(
        IServiceScopeFactory serviceScopeFactory,
        ILogger<BalloonUpdateService> logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var codeforcesApiService = scope.ServiceProvider.GetRequiredService<ICodeforcesApiService>();
                    var balloonService = scope.ServiceProvider.GetRequiredService<IBalloonService>();
                    var hubContext = scope.ServiceProvider.GetRequiredService<IHubContext<BalloonHub>>();
                    var adminSettingsService = scope.ServiceProvider.GetRequiredService<IAdminSettingsService>();

                    var settings = await adminSettingsService.GetActiveAdminSettings();
                    if (settings.IsEnabled && !string.IsNullOrEmpty(settings.ContestId))
                    {
                        var contestId = int.Parse(settings.ContestId);
                        await codeforcesApiService.FetchNewSubmissions(contestId);

                        var pendingBalloons = await balloonService.GetPendingBalloonsAsync();
                        Console.WriteLine(pendingBalloons.Count);
                        await hubContext.Clients.All.SendAsync("ReceiveBalloonUpdates", pendingBalloons, stoppingToken);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating balloons");
            }

            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
        }
    }
} 