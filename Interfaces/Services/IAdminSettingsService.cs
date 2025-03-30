using NucpaBalloonsApi.Models.Requests.AdminSettings;
using NucpaBalloonsApi.Models.SystemModels;

namespace NucpaBalloonsApi.Interfaces.Services
{
    public interface IAdminSettingsService
    {
        Task<List<AdminSettings>> GetAllAsync();
        Task<AdminSettings> UpdateAsync(AdminSettingsUpdateRequestDTO settings);
        Task<AdminSettings> CreateAsync(AdminSettingsCreateRequestDTO settings);
        Task SetActiveAsync(string id);
        Task<AdminSettings> GetActiveAdminSettings();
    }
}
