using NucpaBalloonsApi.Models.Requests.ProblemBalloonMaps;
using NucpaBalloonsApi.Models.SystemModels;

namespace NucpaBalloonsApi.Interfaces.Services
{
    public interface IProblemBalloonMapService
    {
        Task<ProblemBalloonMap> CreateAsync(ProblemBalloonMapCreateRequestDTO problemBalloonMap);
        Task DeleteAsync(string id);
        Task<List<ProblemBalloonMap>> GetAllAsync();
        Task<ProblemBalloonMap?> GetByIdAsync(string id);
        Task<ProblemBalloonMap> UpdateAsync(ProblemBalloonMap problemBalloonMap);
    }
}