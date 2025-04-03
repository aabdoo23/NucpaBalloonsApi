using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NucpaBalloonsApi.Interfaces.Services;
using NucpaBalloonsApi.Models.Requests.Teams;

namespace NucpaBalloonsApi.Controllers
{
    [Route("api/admin/settings/team")]
    [ApiController]
    public class TeamController(ITeamsService teamsService) : ControllerBase
    {
        private readonly ITeamsService _teamsService = teamsService
            ?? throw new ArgumentNullException(nameof(teamsService));
        
        [Authorize]
        [HttpGet("getById")]
        public async Task<IActionResult> GetById(string teamId)
        {
            return Ok(await _teamsService.GetTeamById(teamId));
        }

        [Authorize]
        [HttpPost("createTeam")]
        public async Task<IActionResult> CreateTeam(TeamCreateRequestDTO team)
        {
            return Ok(await _teamsService.CreateTeam(team)); //call here
        }

        [HttpGet("getAll")]
        public async Task<IActionResult> GetAllTeams()
        {
            return Ok(await _teamsService.GetAllTeams());
        }

        [Authorize]
        [HttpPost("deleteTeam")]
        public async Task<IActionResult> DeleteTeamById(string teamId)
        {
            await _teamsService.DeleteTeamById(teamId);
            return Ok();
        }

        [Authorize]
        [HttpPost("updateTeamRoom")]
        public async Task<IActionResult> UpdateTeamRoom(string teamId, string roomId)
        {
            return Ok(await _teamsService.UpdateTeamRoom(teamId, roomId));
        }
    }
}
