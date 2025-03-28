using Microsoft.EntityFrameworkCore;
using NucpaBalloonsApi.Interfaces.Services;
using NucpaBalloonsApi.Models.DTOs;
using NucpaBalloonsApi.Models.SystemModels;
using NucpaBalloonsApi.Models.Codeforces;

namespace NucpaBalloonsApi.Services
{
    public class BalloonService(NucpaDbContext context, IAdminSettingsService adminSettingsService) : IBalloonService
    {
        private readonly NucpaDbContext _context = context ?? throw new ArgumentNullException(nameof(context));
        private readonly IAdminSettingsService _adminSettingsService = adminSettingsService
            ?? throw new ArgumentNullException(nameof(adminSettingsService));
        private long _lastProcessedSubmissionId = 0;

        public async Task<BalloonRequest> CreateBalloonRequestAsync(string teamName, string problemSolved, string balloonColor)
        {
            var request = new BalloonRequest
            {
                BalloonColor = balloonColor,
                Status = BalloonStatus.Pending,
                Timestamp = DateTime.UtcNow
            };

            _context.BalloonRequests.Add(request);
            await _context.SaveChangesAsync();
            return request;
        }

        public async Task ProcessNewSubmissions(List<Submission> submissions)
        {
            if (!submissions.Any()) return;

            var activeSettings = await _adminSettingsService.GetActiveAdminSettings();
            var problemBalloonMaps = activeSettings.ProblemBalloonMaps.ToDictionary(x => x.ProblemIndex, x => x.BalloonColor);

            foreach (var submission in submissions.OrderBy(s => s.Id))
            {
                if (submission.Id <= _lastProcessedSubmissionId) continue;

                if (submission.Verdict != "OK") continue;

                var team = await _context.Teams
                    .FirstOrDefaultAsync(t => t.CodeforcesHandle == submission.Author.Members.FirstOrDefault().Handle);

                if (team == null) {
                    Console.WriteLine("team issue");
                    continue; 
                }

                if (!problemBalloonMaps.TryGetValue(submission.Problem.Index, out var balloonColor)) continue;

                var existingRequest = await _context.BalloonRequests
                    .FirstOrDefaultAsync(b => b.SubmissionId == submission.Id);

                if (existingRequest != null) continue;

                var request = new BalloonRequest
                {
                    Id = Guid.NewGuid().ToString(),
                    SubmissionId = submission.Id,
                    TeamId = team.Id,
                    ProblemIndex = submission.Problem.Index[0],
                    BalloonColor = balloonColor,
                    Status = BalloonStatus.Pending,
                    Timestamp = DateTime.UtcNow
                };

                _context.BalloonRequests.Add(request);
            }

            await _context.SaveChangesAsync();
            _lastProcessedSubmissionId = submissions.Max(s => s.Id);
        }

        public async Task<BalloonRequest?> UpdateBalloonStatusAsync(int id, BalloonStatus status, string? deliveredBy = null)
        {
            var request = await _context.BalloonRequests.FindAsync(id);
            if (request == null) return null;

            request.Status = status;
            if (status == BalloonStatus.Delivered)
            {
                request.DeliveredAt = DateTime.UtcNow;
                request.DeliveredBy = deliveredBy;
            }

            await _context.SaveChangesAsync();
            return request;
        }

        public async Task<List<BalloonRequest>> GetPendingBalloonsAsync()
        {
            return await _context.BalloonRequests
                .Include(b => b.Team)
                .Where(b => b.Status == BalloonStatus.Pending)
                .OrderByDescending(b => b.Timestamp)
                .ToListAsync();
        }

        public async Task<List<BalloonRequest>> GetDeliveredBalloonsAsync()
        {
            return await _context.BalloonRequests
                .Include(b => b.Team)
                .Where(b => b.Status == BalloonStatus.Delivered)
                .OrderByDescending(b => b.DeliveredAt)
                .ToListAsync();
        }

        public async Task<BalloonStatisticsDTO> GetStatisticsAsync()
        {
            var stats = new BalloonStatisticsDTO
            {
                TotalPending = await _context.BalloonRequests.CountAsync(b => b.Status == BalloonStatus.Pending),
                TotalDelivered = await _context.BalloonRequests.CountAsync(b => b.Status == BalloonStatus.Delivered)
            };

            var colorCounts = await _context.BalloonRequests
                .Where(b => b.Status == BalloonStatus.Pending)
                .GroupBy(b => b.BalloonColor)
                .Select(g => new { Color = g.Key, Count = g.Count() })
                .ToListAsync();

            stats.ColorCounts = colorCounts.ToDictionary(x => x.Color, x => x.Count);
            return stats;
        }
    }
} 