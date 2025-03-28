using Microsoft.IdentityModel.Tokens;
using NucpaBalloonsApi.Interfaces.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace NucpaBalloonsApi.Services;

public class AdminService(IConfiguration configuration) : IAdminService
{
    private readonly IConfiguration _configuration = configuration;

    public bool ValidateCredentials(string username, string password)
    {
        var adminUsername = _configuration["AdminSettings:Username"] ?? "admin";
        var adminPassword = _configuration["AdminSettings:Password"] ?? "admin";

        return username == adminUsername && password == adminPassword;
    }

    public string GenerateJwtToken(string username)
    {
        var key = Encoding.ASCII.GetBytes(_configuration["AdminSettings:JwtSecret"] ?? "your-secret-key-here");
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, "Admin")
            }),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
} 