using NucpaBalloonsApi.Interfaces.Repositories.Common;
using NucpaBalloonsApi.Interfaces.Services;
using NucpaBalloonsApi.Models.Requests.ProblemBalloonMaps;
using NucpaBalloonsApi.Models.SystemModels;

namespace NucpaBalloonsApi.Services
{
    public class ProblemBalloonMapService(IBaseRepository<ProblemBalloonMap> problemBalloonMapRepository, IAdminSettingsService adminSettingsService) : IProblemBalloonMapService
    {
        private readonly IBaseRepository<ProblemBalloonMap> _problemBalloonMapRepository = problemBalloonMapRepository
                ?? throw new ArgumentNullException(nameof(problemBalloonMapRepository));
        private readonly IAdminSettingsService _adminSettingsService = adminSettingsService
                ?? throw new ArgumentNullException(nameof(adminSettingsService));

        public async Task<ProblemBalloonMap> CreateAsync(ProblemBalloonMapCreateRequestDTO problemBalloonMap)
        {
            var activeAdminSettings = await _adminSettingsService.GetActiveAdminSettings();
            var existingMaps = await _problemBalloonMapRepository.GetAllAsync();

            if (existingMaps.Any(map => map.BalloonColor == problemBalloonMap.BalloonColor && map.AdminSettingsId == activeAdminSettings.Id))
            {
                throw new InvalidOperationException("A ProblemBalloonMap with the same BalloonColor already exists.");
            }

            var newProblemBalloonMap = new ProblemBalloonMap
            {
                Id = Guid.NewGuid().ToString(),
                ProblemIndex = problemBalloonMap.ProblemIndex,
                BalloonColor = problemBalloonMap.BalloonColor,
                AdminSettingsId = activeAdminSettings.Id
            };
            return await _problemBalloonMapRepository.InsertAsync(newProblemBalloonMap);
        }

        public async Task<List<ProblemBalloonMap>> GetAllAsync()
        {
            var activeAdminSettings = await _adminSettingsService.GetActiveAdminSettings();
            return (await _problemBalloonMapRepository.GetAllAsync()).Where(x => x.AdminSettingsId == activeAdminSettings.Id).ToList();
        }

        public Task<ProblemBalloonMap?> GetByIdAsync(string id)
        {
            return _problemBalloonMapRepository.GetByIdAsync(id);
        }

        public async Task<ProblemBalloonMap> UpdateAsync(ProblemBalloonMap problemBalloonMap)
        {
            var activeAdminSettings = await _adminSettingsService.GetActiveAdminSettings();
            var existingMaps = await _problemBalloonMapRepository.GetAllAsync();

            if (existingMaps.Any(map => map.BalloonColor == problemBalloonMap.BalloonColor && map.AdminSettingsId == activeAdminSettings.Id && map.Id != problemBalloonMap.Id))
            {
                throw new InvalidOperationException("A ProblemBalloonMap with the same BalloonColor already exists.");
            }

            Console.WriteLine($"Updating problemBalloonMap with id {problemBalloonMap.Id} and problemIndex {problemBalloonMap.ProblemIndex} and Color {problemBalloonMap.BalloonColor}");
            return await _problemBalloonMapRepository.UpdateAsync(problemBalloonMap);
        }

        public async Task DeleteAsync(string id)
        {
            await _problemBalloonMapRepository.DeleteAsync(id);
        }
    }
}
