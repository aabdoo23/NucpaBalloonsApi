using Microsoft.AspNetCore.SignalR;
using NucpaBalloonsApi.Hubs;
using NucpaBalloonsApi.Interfaces.Services;

namespace NucpaBalloonsApi.Services;

public class BalloonUpdateService(
    IServiceScopeFactory serviceScopeFactory,
    ILogger<BalloonUpdateService> logger) : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory 
        ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
    private readonly ILogger<BalloonUpdateService> _logger = logger 
        ?? throw new ArgumentNullException(nameof(logger));

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
                    if (settings.IsEnabled && settings.ContestId!=0)
                    {
                        await codeforcesApiService.FetchNewSubmissions(settings.ContestId);

                        var pendingBalloons = await balloonService.GetPendingBalloonsAsync();
                        var pickedUpBalloons = await balloonService.GetPickedUpBalloonsAsync();
                        var readyForPickupBalloons = await balloonService.GetReadyForPickupBalloonsAsync();
                        var deliveredBalloons = await balloonService.GetDeliveredBalloonsAsync();
                        Console.WriteLine("Sending to websocket now {0} pending, {1} picked up, {2} delivered, {3} readyForPickup", pendingBalloons.Count, pickedUpBalloons.Count, deliveredBalloons.Count, readyForPickupBalloons.Count);

                        await hubContext.Clients.All.SendAsync("ReceiveBalloonUpdates", new
                        {
                            Pending = pendingBalloons,
                            ReadyForPickup = readyForPickupBalloons,
                            PickedUp = pickedUpBalloons,
                            Delivered = deliveredBalloons
                        }, stoppingToken);
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