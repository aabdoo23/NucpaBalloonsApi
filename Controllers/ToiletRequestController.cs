using Microsoft.AspNetCore.Mvc;
using NucpaBalloonsApi.Interfaces.Services;
using NucpaBalloonsApi.Models.Requests.ToiletRequest;

namespace NucpaBalloonsApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ToiletRequestController(IToiletRequestService toiletRequestService) : ControllerBase
    {
        private readonly IToiletRequestService _toiletRequestService = toiletRequestService
            ?? throw new ArgumentNullException(nameof(toiletRequestService));

        [HttpPost]
        public async Task<IActionResult> CreateToiletRequest([FromBody] ToiletRequestDTO toiletRequest)
        {
            if (toiletRequest == null)
            {
                return BadRequest("Toilet request cannot be null.");
            }
            var createdToiletRequest = await _toiletRequestService.CreateToiletRequestAsync(toiletRequest);
            return Ok(createdToiletRequest);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetToiletRequestById(string id)
        {
            var toiletRequest = await _toiletRequestService.GetToiletRequestByIdAsync(id);
            if (toiletRequest == null)
            {
                return NotFound($"Toilet request with ID {id} not found.");
            }
            return Ok(toiletRequest);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateToiletRequest(string id, [FromBody] ToiletRequestDTO toiletRequest)
        {
            if (toiletRequest == null)
            {
                return BadRequest("Toilet request cannot be null.");
            }
            var updatedToiletRequest = await _toiletRequestService.UpdateToiletRequestAsync(id, toiletRequest);
            if (updatedToiletRequest == null)
            {
                return NotFound($"Toilet request with ID {id} not found.");
            }
            return Ok(updatedToiletRequest);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteToiletRequest(string id)
        {
            var result = await _toiletRequestService.DeleteToiletRequestAsync(id);
            if (!result)
            {
                return NotFound($"Toilet request with ID {id} not found.");
            }
            return NoContent();
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllToiletRequests()
        {
            var toiletRequests = await _toiletRequestService.GetAllToiletRequestsAsync();
            return Ok(toiletRequests);
        }

        [HttpGet("pending")]
        public async Task<IActionResult> GetAllPendingToiletRequests()
        {
            var toiletRequests = await _toiletRequestService.GetAllPendingToiletRequestsAsync();
            return Ok(toiletRequests);
        }

        [HttpGet("in-progress")]
        public async Task<IActionResult> GetAllInProgressToiletRequests()
        {
            var toiletRequests = await _toiletRequestService.GetAllInProgressToiletRequestsAsync();
            return Ok(toiletRequests);
        }

        [HttpGet("completed")]
        public async Task<IActionResult> GetAllCompletedToiletRequests()
        {
            var toiletRequests = await _toiletRequestService.GetAllCompletedToiletRequestsAsync();
            return Ok(toiletRequests);
        }

        [HttpPut("status")]
        public async Task<IActionResult> UpdateToiletRequestStatus([FromBody] ToiletRequestStatusUpdateDTO toiletRequest)
        {
            if (toiletRequest == null)
            {
                return BadRequest("Toilet request cannot be null.");
            }
            var updatedToiletRequest = await _toiletRequestService.UpdateToiletRequestStatusAsync(toiletRequest);
            if (updatedToiletRequest == null)
            {
                return NotFound($"Toilet request with ID {toiletRequest.Id} not found.");
            }
            return Ok(updatedToiletRequest);
        }
    }
}
