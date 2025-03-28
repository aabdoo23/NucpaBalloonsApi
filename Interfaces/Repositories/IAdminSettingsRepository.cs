using NucpaBalloonsApi.Interfaces.Repositories.Common;
using NucpaBalloonsApi.Models.SystemModels;

namespace NucpaBalloonsApi.Interfaces.Repositories
{
    public interface IAdminSettingsRepository : IBaseRepository<AdminSettings>
    {
        Task<AdminSettings> GetActiveAdminSettings();
    }
}
