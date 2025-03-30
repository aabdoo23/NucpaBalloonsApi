using NucpaBalloonsApi.Interfaces.Repositories.Common;
using NucpaBalloonsApi.Interfaces.Services;
using NucpaBalloonsApi.Models.Requests;
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
            return (await _problemBalloonMapRepository.GetAllAsync()).ToList();
        }

        public Task<ProblemBalloonMap?> GetByIdAsync(string id)
        {
            return _problemBalloonMapRepository.GetByIdAsync(id);
        }

        public async Task<ProblemBalloonMap> UpdateAsync(ProblemBalloonMap problemBalloonMap)
        {
            return await _problemBalloonMapRepository.UpdateAsync(problemBalloonMap);
        }

        public async Task DeleteAsync(string id)
        {
            await _problemBalloonMapRepository.DeleteAsync(id);
        }
    }
}
