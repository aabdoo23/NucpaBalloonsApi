using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using NucpaBalloonsApi.Hubs;
using NucpaBalloonsApi.Interfaces.Services;
using NucpaBalloonsApi.Models.DTOs;
using NucpaBalloonsApi.Models.SystemModels;

namespace NucpaBalloonsApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BalloonController(IBalloonService balloonService, IHubContext<BalloonHub> hubContext) : ControllerBase
    {
        private readonly IBalloonService _balloonService = balloonService;
        private readonly IHubContext<BalloonHub> _hubContext = hubContext;

        [HttpPost]
        public async Task<IActionResult> CreateBalloonRequest([FromBody] BalloonRequest request)
        {
            //var newRequest = await _balloonService.CreateBalloonRequestAsync(
            //    //request.TeamName,
            //    //request.ProblemSolved,
            //    request.BalloonColor
            //);

            //await _hubContext.Clients.All.SendAsync("BalloonRequestCreated", newRequest);
            await UpdateStatistics();
            
            return Ok();
        }

        [HttpPut("{id}/status")]
        public async Task<ActionResult<BalloonRequest>> UpdateStatus(
            int id,
            [FromBody] BalloonStatus status,
            [FromQuery] string? deliveredBy = null)
        {
            var request = await _balloonService.UpdateBalloonStatusAsync(id, status, deliveredBy);
            if (request == null) return NotFound();

            await _hubContext.Clients.All.SendAsync("BalloonStatusUpdated", request);
            await UpdateStatistics();
            
            return Ok(request);
        }

        [HttpGet("pending")]
        public async Task<ActionResult<List<BalloonRequest>>> GetPendingBalloons()
        {
            return await _balloonService.GetPendingBalloonsAsync();
        }

        [HttpGet("delivered")]
        public async Task<ActionResult<List<BalloonRequest>>> GetDeliveredBalloons()
        {
            return await _balloonService.GetDeliveredBalloonsAsync();
        }

        [HttpGet("statistics")]
        public async Task<ActionResult<BalloonStatisticsDTO>> GetStatistics()
        {
            return await _balloonService.GetStatisticsAsync();
        }

        private async Task UpdateStatistics()
        {
            var stats = await _balloonService.GetStatisticsAsync();
            await _hubContext.Clients.All.SendAsync("StatisticsUpdated", stats);
        }
    }
} 