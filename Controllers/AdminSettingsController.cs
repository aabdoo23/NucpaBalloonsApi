using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NucpaBalloonsApi.Interfaces.Services;
using NucpaBalloonsApi.Models.Requests;

namespace NucpaBalloonsApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("admin/settings")]
    public class AdminSettingsController(IAdminSettingsService adminSettingsService) : ControllerBase
    {
        private readonly IAdminSettingsService _adminSettingsService = adminSettingsService
            ?? throw new ArgumentNullException(nameof(adminSettingsService));

        [HttpGet("getAll")]
        public async Task<IActionResult> GetAll()
        {
            var settings = await _adminSettingsService.GetAllAsync();
            return Ok(settings);
        }

        [HttpPost("enable")]
        public async Task<IActionResult> SetActive(string id)
        {
            await _adminSettingsService.SetActiveAsync(id);
            return Ok();
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] AdminSettingsCreateRequestDTO adminSettings)
        {
            var settings = await _adminSettingsService.CreateAsync(adminSettings);
            return Ok(settings);
        }

        [HttpPost("update")]
        public async Task<IActionResult> Update([FromBody] AdminSettingsUpdateRequestDTO adminSettings)
        {
            var settings = await _adminSettingsService.UpdateAsync(adminSettings);
            return Ok(settings);
        }

        [HttpGet("getActive")]
        public async Task<IActionResult> GetActive()
        {
            var settings = await _adminSettingsService.GetActiveAdminSettings();
            return Ok(AdminSettingsResponseDTO.FromEntity(settings));
        }
    }
}
