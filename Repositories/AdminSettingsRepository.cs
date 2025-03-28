using Microsoft.EntityFrameworkCore;
using NucpaBalloonsApi.Interfaces.Repositories;
using NucpaBalloonsApi.Models.SystemModels;
using NucpaBalloonsApi.Repositories.Common;

namespace NucpaBalloonsApi.Repositories
{
    public class AdminSettingsRepository(NucpaDbContext dbContext) : BaseRepository<AdminSettings>(dbContext), IAdminSettingsRepository
    {
        private readonly NucpaDbContext _context = dbContext
                ?? throw new ArgumentNullException(nameof(dbContext));
        public Task<AdminSettings> GetActiveAdminSettings()
        {
            return _context.AdminSettings
                .Include(x => x.Rooms)
                .Include(x => x.Teams)
                .Include(x => x.ProblemBalloonMaps)
                .FirstOrDefaultAsync(x => x.IsEnabled);
        }
    }
}
