using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NucpaBalloonsApi.Interfaces.Services;
using NucpaBalloonsApi.Models.Requests;
using NucpaBalloonsApi.Models.SystemModels;

namespace NucpaBalloonsApi.Controllers
{
    [Authorize]
    [Route("api/admin/settings/team")]
    [ApiController]
    public class TeamController(ITeamsService teamsService) : ControllerBase
    {
        private readonly ITeamsService _teamsService = teamsService 
            ?? throw new ArgumentNullException(nameof(teamsService));

        [HttpGet("getById")]
        public async Task<IActionResult> GetById(string teamId)
        {
            return Ok(await _teamsService.GetTeamById(teamId));
        }

        [HttpPost("createTeam")]
        public async Task<IActionResult> CreateTeam(TeamCreateRequestDTO team)
        {
            return Ok(await _teamsService.CreateTeam(team)); //call here
        }

        [HttpPost("deleteTeam")]
        public async Task<IActionResult> DeleteTeamById(string teamId)
        {
            await _teamsService.DeleteTeamById(teamId);
            return Ok();
        }

        [HttpPost("updateTeamRoom")]
        public async Task<IActionResult> UpdateTeamRoom(string teamId, string roomId)
        {
            return Ok(await _teamsService.UpdateTeamRoom(teamId, roomId));
        }
    }
}
