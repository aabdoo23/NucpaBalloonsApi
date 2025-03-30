using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NucpaBalloonsApi.Interfaces.Services;
using NucpaBalloonsApi.Models.Requests.Rooms;
using NucpaBalloonsApi.Models.SystemModels;

namespace NucpaBalloonsApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/admin/settings/[controller]")]
    public class RoomController(IRoomsService roomsService) : ControllerBase
    {
        private readonly IRoomsService _roomsService = roomsService
            ?? throw new ArgumentNullException(nameof(roomsService));

        [HttpGet("getAll")]
        public async Task<IActionResult> GetAll()
        {
            var rooms = await _roomsService.GetAllAsync();
            return Ok(rooms);
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] RoomCreateRequestDTO room)
        {
            try
            {
                var newRoom = await _roomsService.CreateAsync(room);
                return Ok(newRoom);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { error = "An error occurred while creating the room." });
            }
        }

        [HttpPost("delete")]
        public async Task Update(string RoomId)
        {
            await _roomsService.DeleteAsync(RoomId);
        }

    }
}
