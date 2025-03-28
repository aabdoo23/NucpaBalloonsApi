using NucpaBalloonsApi.Models.Codeforces;
using NucpaBalloonsApi.Models.DTOs;
using NucpaBalloonsApi.Models.SystemModels;

namespace NucpaBalloonsApi.Interfaces.Services
{
    public interface IBalloonService
    {
        Task<BalloonRequest> CreateBalloonRequestAsync(string teamName, string problemSolved, string balloonColor);
        Task<BalloonRequest?> UpdateBalloonStatusAsync(int id, BalloonStatus status, string? deliveredBy = null);
        Task<List<BalloonRequest>> GetPendingBalloonsAsync();
        Task<List<BalloonRequest>> GetDeliveredBalloonsAsync();
        Task<BalloonStatisticsDTO> GetStatisticsAsync();
        Task ProcessNewSubmissions(List<Submission> submissions);
    }
} 