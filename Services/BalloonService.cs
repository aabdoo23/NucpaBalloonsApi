using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using NucpaBalloonsApi.Hubs;
using NucpaBalloonsApi.Interfaces.Services;
using NucpaBalloonsApi.Models.Codeforces;
using NucpaBalloonsApi.Models.DTOs;
using NucpaBalloonsApi.Models.Requests.ProblemBalloonMaps;
using NucpaBalloonsApi.Models.SystemModels;

namespace NucpaBalloonsApi.Services
{
    public class BalloonService(NucpaDbContext context, IAdminSettingsService adminSettingsService, IHubContext<BalloonHub> hubContext, ILogger<BalloonService> logger, IProblemBalloonMapService problemBalloonMapService) : IBalloonService
    {
        private readonly NucpaDbContext _context = context ?? throw new ArgumentNullException(nameof(context));
        private readonly IAdminSettingsService _adminSettingsService = adminSettingsService
            ?? throw new ArgumentNullException(nameof(adminSettingsService));
        private readonly IProblemBalloonMapService _problemBalloonMapService = problemBalloonMapService ?? throw new ArgumentNullException(nameof(problemBalloonMapService));
        private readonly IHubContext<BalloonHub> _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
        private readonly ILogger<BalloonService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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
            Console.WriteLine("processign");
            if (!submissions.Any()) return;
            Console.WriteLine("some subs");
            var activeSettings = await _adminSettingsService.GetActiveAdminSettings();
            var problemBalloonMaps = activeSettings.ProblemBalloonMaps.ToDictionary(x => x.ProblemIndex, x => x.BalloonColor);

            foreach (var submission in submissions.OrderBy(s => s.Id))
            {
                if (submission.Id <= _lastProcessedSubmissionId) { Console.WriteLine("lastprocessed" + _lastProcessedSubmissionId); continue; }

                if (submission.Verdict != "OK") { Console.WriteLine("not true verdict"); continue; }

                var team = await _context.Teams
                    .FirstOrDefaultAsync(t => t.CodeforcesHandle == submission.Author.Members.FirstOrDefault().Handle);

                if (team == null)
                {
                    Console.WriteLine("new team, creating one");
                    using var transaction = await _context.Database.BeginTransactionAsync();
                    try
                    {
                        // Try to find the team again within the transaction
                        team = await _context.Teams
                            .FirstOrDefaultAsync(t => t.CodeforcesHandle == submission.Author.Members.FirstOrDefault().Handle);

                        if (team == null)
                        {
                            var room = await _context.Rooms.FirstOrDefaultAsync();
                            if (room == null)
                            {
                                var defaultRoom = new Room
                                {
                                    Id = "DefaultRoom",
                                    IsAvailable = true,
                                    Capacity = 27,
                                    AdminSettingsId = activeSettings.Id
                                };
                                _context.Rooms.Add(defaultRoom);
                                await _context.SaveChangesAsync();
                            }

                            team = new Team
                            {
                                Id = Guid.NewGuid().ToString(),
                                CodeforcesHandle = submission.Author.Members.FirstOrDefault().Handle,
                                RoomId = _context.Rooms.First().Id,
                                AdminSettingsId = activeSettings.Id
                            };
                            _context.Teams.Add(team);
                            await _context.SaveChangesAsync();
                        }
                        await transaction.CommitAsync();
                    }
                    catch (Exception)
                    {
                        await transaction.RollbackAsync();
                        // If the transaction failed, try to get the team one more time
                        team = await _context.Teams
                            .FirstOrDefaultAsync(t => t.CodeforcesHandle == submission.Author.Members.FirstOrDefault().Handle);
                        if (team == null)
                        {
                            throw; // If we still can't find the team, rethrow the exception
                        }
                    }
                }

                if (!problemBalloonMaps.TryGetValue(submission.Problem.Index, out var balloonColor)){
                    var defaultProblemBalloonMap = await _problemBalloonMapService.CreateAsync(new ProblemBalloonMapCreateRequestDTO
                    {
                        ProblemIndex = submission.Problem.Index[0].ToString(),
                        BalloonColor = "Red"
                    });
                    problemBalloonMaps[defaultProblemBalloonMap.ProblemIndex] = "Red";
                }

                var existingRequest = await _context.BalloonRequests
                    .FirstOrDefaultAsync(b => b.SubmissionId == submission.Id);

                if (existingRequest != null) { Console.WriteLine("already existing"); continue; }

                var request = new BalloonRequest
                {
                    Id = Guid.NewGuid().ToString(),
                    SubmissionId = submission.Id,
                    TeamId = team.Id,
                    ProblemIndex = submission.Problem.Index[0],
                    BalloonColor = balloonColor,
                    ContestId = submission.ContestId,
                    Status = BalloonStatus.Pending,
                    Timestamp = DateTime.UtcNow
                };
                Console.WriteLine("submission id");
                Console.WriteLine(request.SubmissionId);

                _context.BalloonRequests.Add(request);
            }

            await _context.SaveChangesAsync();
            _lastProcessedSubmissionId = submissions.Max(s => s.Id);
        }

        public async Task<BalloonRequest?> UpdateBalloonStatusAsync(string id, BalloonStatus status, string? deliveredBy = null)
        {
            try
            {
                var request = await _context.BalloonRequests.FindAsync(id);
                if (request == null) return null;

                request.Status = status;
                request.StatusChangedAt = DateTime.UtcNow;
                request.StatusChangedBy = deliveredBy;

                await _context.SaveChangesAsync();

                // Send real-time updates to all clients
                var pendingBalloons = await GetPendingBalloonsAsync();
                var pickedUpBalloons = await GetPickedUpBalloonsAsync();
                var deliveredBalloons = await GetDeliveredBalloonsAsync();

                try
                {
                    await _hubContext.Clients.All.SendAsync("BalloonStatusChanged", new
                    {
                        Pending = pendingBalloons,
                        PickedUp = pickedUpBalloons,
                        Delivered = deliveredBalloons
                    });
                    _logger.LogInformation("Successfully broadcasted balloon status update to all clients");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to broadcast balloon status update to clients");
                    // Don't throw here, as the database update was successful
                }

                return request;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update balloon status");
                throw;
            }
        }

        public async Task<List<BalloonRequestDTO>> GetPendingBalloonsAsync()
        {
            var activeSettings = await _adminSettingsService.GetActiveAdminSettings();

            var pending = await _context.BalloonRequests
                .Include(b => b.Team)
                .Where(b => b.ContestId == activeSettings.ContestId)
                .Where(b => b.Status == BalloonStatus.Pending)
                .OrderByDescending(b => b.Timestamp)
                .ToListAsync();
            Console.WriteLine("count of pending");
            Console.WriteLine(pending.Count);

            return pending.Select(b => new BalloonRequestDTO
            {
                Id = b.Id,
                TeamName = b.Team.CodeforcesHandle,
                ProblemIndex = b.ProblemIndex,
                BalloonColor = b.BalloonColor,
                ContestId = b.ContestId,
                Timestamp = b.Timestamp,
                Status = b.Status.ToString(),
                StatusChangedBy = b.StatusChangedBy,
                StatusChangedAt = b.StatusChangedAt,
                TeamId = b.TeamId,
                SubmissionId = b.SubmissionId
            }).ToList();

        }

        public async Task<List<BalloonRequestDTO>> GetDeliveredBalloonsAsync()
        {

            var activeSettings = await _adminSettingsService.GetActiveAdminSettings();
            var delivered = await _context.BalloonRequests
                .Include(b => b.Team)
                .Where(b => b.ContestId == activeSettings.ContestId)
                .Where(b => b.Status == BalloonStatus.Delivered)
                .OrderByDescending(b => b.StatusChangedAt)
                .ToListAsync();
            Console.WriteLine("count of delivered");
            Console.WriteLine(delivered.Count);

            return delivered.Select(b => new BalloonRequestDTO
            {
                Id = b.Id,
                TeamName = b.Team.CodeforcesHandle,
                ProblemIndex = b.ProblemIndex,
                BalloonColor = b.BalloonColor,
                ContestId = b.ContestId,
                Timestamp = b.Timestamp,
                Status = b.Status.ToString(),
                StatusChangedAt = b.StatusChangedAt,
                StatusChangedBy = b.StatusChangedBy,
                TeamId = b.TeamId,
                SubmissionId = b.SubmissionId
            }).ToList();
        }

        public async Task<List<BalloonRequestDTO>> GetPickedUpBalloonsAsync()
        {
            var activeSettings = await _adminSettingsService.GetActiveAdminSettings();

            var pickedUp = await _context.BalloonRequests
                .Include(b => b.Team)
                .Where(b => b.ContestId == activeSettings.ContestId)
                .Where(b => b.Status == BalloonStatus.PickedUp)
                .OrderByDescending(b => b.StatusChangedAt)
                .ToListAsync();

            Console.WriteLine("count of picked up");
            Console.WriteLine(pickedUp.Count);

            return pickedUp.Select(b => new BalloonRequestDTO
            {
                Id = b.Id,
                TeamName = b.Team.CodeforcesHandle,
                ProblemIndex = b.ProblemIndex,
                BalloonColor = b.BalloonColor,
                ContestId = b.ContestId,
                Timestamp = b.Timestamp,
                Status = b.Status.ToString(),
                StatusChangedBy = b.StatusChangedBy,
                StatusChangedAt = b.StatusChangedAt,
                TeamId = b.TeamId,
                SubmissionId = b.SubmissionId
            }).ToList();
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