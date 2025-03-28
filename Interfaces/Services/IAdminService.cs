namespace NucpaBalloonsApi.Interfaces.Services;

public interface IAdminService
{
    bool ValidateCredentials(string username, string password);
    string GenerateJwtToken(string username);
}
