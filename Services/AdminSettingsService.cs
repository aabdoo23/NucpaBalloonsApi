using Microsoft.EntityFrameworkCore;
using NucpaBalloonsApi.Interfaces.Repositories.Common;
using NucpaBalloonsApi.Interfaces.Services;
using NucpaBalloonsApi.Models.Requests;
using NucpaBalloonsApi.Models.SystemModels;

namespace NucpaBalloonsApi.Services
{
    public class AdminSettingsService(IBaseRepository<AdminSettings> adminSettingsRepository, NucpaDbContext nucpaDbContext) : IAdminSettingsService
    {
        private readonly IBaseRepository<AdminSettings> _adminSettingsRepository = adminSettingsRepository
                ?? throw new ArgumentNullException(nameof(adminSettingsRepository));

        private readonly NucpaDbContext _context = nucpaDbContext
                ?? throw new ArgumentNullException(nameof(nucpaDbContext));
        public async Task<List<AdminSettings>> GetAllAsync()
        {
            var all = await _adminSettingsRepository.GetAllAsync();
            return [.. all];
        }
        public async Task<AdminSettings> GetByIdAsync(string id)
        {
            return await _adminSettingsRepository.GetByIdAsync(id);
        }
        public async Task<AdminSettings> CreateAsync(AdminSettingsCreateRequestDTO adminSettings)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var existingSettings = await _adminSettingsRepository.GetAllAsync();
                foreach (var setting in existingSettings)
                {
                    if (setting.IsEnabled)
                    {
                        setting.IsEnabled = false;
                        await _adminSettingsRepository.UpdateAsync(setting);
                    }
                }

                var settings = new AdminSettings
                {
                    Id = Guid.NewGuid().ToString(),
                    AdminUsername = adminSettings.AdminUsername,
                    ContestId = adminSettings.ContestId,
                    CodeforcesApiKey = adminSettings.CodeforcesApiKey,
                    CodeforcesApiSecret = adminSettings.CodeforcesApiSecret,
                    IsEnabled = true
                };

                await _adminSettingsRepository.InsertAsync(settings);

                await transaction.CommitAsync();
                return settings;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }



        public async Task SetActiveAsync(string id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var settings = await _adminSettingsRepository.GetAllAsync();

                foreach (var setting in settings)
                {
                    if (_context.Entry(setting).State == EntityState.Detached)
                    {
                        _context.Attach(setting);
                    }
                    setting.IsEnabled = (setting.Id == id);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<AdminSettings> UpdateAsync(AdminSettingsUpdateRequestDTO settings)
        {
            var originalSettings = await _adminSettingsRepository.GetByIdAsync(settings.Id)
                ?? throw new InvalidOperationException("Settings not found");

            originalSettings.AdminUsername = settings.AdminUsername ?? originalSettings.AdminUsername;
            originalSettings.ContestId = settings.ContestId ?? originalSettings.ContestId;
            originalSettings.CodeforcesApiKey = settings.CodeforcesApiKey ?? originalSettings.CodeforcesApiKey;
            originalSettings.CodeforcesApiSecret = settings.CodeforcesApiSecret ?? originalSettings.CodeforcesApiSecret;
            originalSettings.IsEnabled = settings.IsEnabled ?? originalSettings.IsEnabled;

            return await _adminSettingsRepository.UpdateAsync(originalSettings);
        }

        public async Task<AdminSettings> GetActiveAdminSettings()
        {
            return await _context.AdminSettings
                .AsNoTracking()
                .Include(s => s.ProblemBalloonMaps)
                .Include(s => s.Rooms)
                .Include(s => s.Teams)
                .FirstOrDefaultAsync(s => s.IsEnabled);
        }

    }
}
