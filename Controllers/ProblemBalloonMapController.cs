using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NucpaBalloonsApi.Interfaces.Services;
using NucpaBalloonsApi.Models.Requests.ProblemBalloonMaps;
using NucpaBalloonsApi.Models.SystemModels;

namespace NucpaBalloonsApi.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("api/admin/settings/[controller]")]
    public class ProblemBalloonMapController(IProblemBalloonMapService problemBalloonMapService) : ControllerBase
    {
        private readonly IProblemBalloonMapService _problemBalloonMapService = problemBalloonMapService
            ?? throw new ArgumentNullException(nameof(problemBalloonMapService));

        [HttpGet("getAll")]
        public async Task<IActionResult> GetAll()
        {
            var settings = await _problemBalloonMapService.GetAllAsync();
            return Ok(settings);
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] ProblemBalloonMapCreateRequestDTO problemBalloonMap)
        {
            var settings = await _problemBalloonMapService.CreateAsync(problemBalloonMap);
            return Ok(settings);
        }

        [HttpPost("update")]
        public async Task<IActionResult> Update([FromBody] ProblemBalloonMap problemBalloonMap)
        {
            var settings = await _problemBalloonMapService.UpdateAsync(problemBalloonMap);
            return Ok(settings);
        }

        [HttpPost("delete")]
        public async Task<IActionResult> Delete(string id)
        {
            await _problemBalloonMapService.DeleteAsync(id);
            return Ok();
        }
    }
}
