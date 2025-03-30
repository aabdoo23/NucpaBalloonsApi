using Microsoft.AspNetCore.Mvc;
using NucpaBalloonsApi.Interfaces.Services;
using NucpaBalloonsApi.Models.DTOs;
using NucpaBalloonsApi.Models.Requests.BalloonRequest;
using NucpaBalloonsApi.Models.SystemModels;

//just for testing
namespace NucpaBalloonsApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BalloonController(IBalloonService balloonService) : ControllerBase
    {
        private readonly IBalloonService _balloonService = balloonService;

        [HttpPut("status")]
        public async Task<ActionResult<BalloonRequest>> UpdateStatus(BalloonStatusUpdateRequest request)
        {
            var requeste = await _balloonService.UpdateBalloonStatusAsync(request.Id, request.Status, request.DeliveredBy);
            if (requeste == null) return NotFound();
            
            return Ok(requeste);
        }

        [HttpGet("pending")]
        public async Task<ActionResult<List<BalloonRequestDTO>>> GetPendingBalloons()
        {
            return await _balloonService.GetPendingBalloonsAsync();
        }

        [HttpGet("picked-up")]
        public async Task<ActionResult<List<BalloonRequestDTO>>> GetPickedUpBalloons()
        {
            return await _balloonService.GetPickedUpBalloonsAsync();
        }

        [HttpGet("delivered")]
        public async Task<ActionResult<List<BalloonRequestDTO>>> GetDeliveredBalloons()
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
        }
    }
} 