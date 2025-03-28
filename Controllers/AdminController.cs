using Microsoft.AspNetCore.Mvc;
using NucpaBalloonsApi.Interfaces.Services;
using NucpaBalloonsApi.Models.DTOs.Admin;

namespace NucpaBalloonsApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdminController(IAdminService adminService, ILogger<AdminController> logger) : ControllerBase
{
    private readonly IAdminService _adminService = adminService
        ?? throw new ArgumentNullException(nameof(adminService));
    private readonly ILogger<AdminController> _logger = logger
        ?? throw new ArgumentNullException(nameof(logger));

    [HttpPost("login")]
    public ActionResult<LoginResponseDTO> Login([FromBody] LoginRequestDTO request)
    {
        try
        {
            if (_adminService.ValidateCredentials(request.Username, request.Password))
            {
                var token = _adminService.GenerateJwtToken(request.Username);
                return Ok(new LoginResponseDTO
                {
                    Token = token,
                    Message = "Login successful"
                });
            }

            return Unauthorized(new LoginResponseDTO
            {
                Message = "Invalid credentials"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login attempt");
            return StatusCode(500, new LoginResponseDTO
            {
                Message = "Internal server error"
            });
        }
    }
}