using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using NucpaBalloonsApi.Hubs;
using NucpaBalloonsApi.Interfaces.Services;
using NucpaBalloonsApi.Models.DTOs;
using NucpaBalloonsApi.Models.Requests.ToiletRequest;
using NucpaBalloonsApi.Models.SystemModels;

namespace NucpaBalloonsApi.Services
{
    public class ToiletRequestService(NucpaDbContext dbContext, IHubContext<BalloonHub> hubContext, IServiceScopeFactory serviceScopeFactory) : IToiletRequestService
    {
        private readonly NucpaDbContext _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        private readonly IHubContext<BalloonHub> _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
        private readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));

        public async Task<ToiletResponseDTO> CreateToiletRequestAsync(ToiletRequestDTO toiletRequest)
        {
            var newToiletRequest = new ToiletRequest
            {
                Id = Guid.NewGuid().ToString(),
                TeamId = toiletRequest.TeamId,
                IsMale = toiletRequest.IsMale,
                IsUrgent = toiletRequest.IsUrgent,
                Comment = toiletRequest.Comment,
                Status = ToiletRequestStatus.Pending,
                StatusChangedAt = DateTime.UtcNow,
                StatusChangedBy = toiletRequest.StatusChangedBy,
                Timestamp = DateTime.UtcNow
            };
            await _dbContext.ToiletRequests.AddAsync(newToiletRequest);
            await _dbContext.SaveChangesAsync();

            await NotifyToiletRequestStatusChange();

            return MapToDTO(newToiletRequest);
        }

        public async Task<bool> DeleteToiletRequestAsync(string id)
        {
            var toiletRequest = await _dbContext.ToiletRequests.FindAsync(id);
            if (toiletRequest == null)
            {
                return false;
            }
            _dbContext.ToiletRequests.Remove(toiletRequest);
            await _dbContext.SaveChangesAsync();

            await NotifyToiletRequestStatusChange();

            return true;
        }

        public async Task<IList<ToiletResponseDTO>> GetAllToiletRequestsAsync()
        {
            var toiletRequests = await _dbContext.ToiletRequests
                .Include(tr => tr.Team)
                .ThenInclude(t => t.Room)
                .ToListAsync();
            return toiletRequests.Select(MapToDTO).ToList();

        }

        public async Task<IList<ToiletResponseDTO>> GetAllPendingToiletRequestsAsync()
        {
            var toiletRequests = await _dbContext.ToiletRequests
                .Include(tr => tr.Team)
                .ThenInclude(t => t.Room)
                .Where(tr => tr.Status == ToiletRequestStatus.Pending)
                .ToListAsync();
            return toiletRequests.Select(MapToDTO).ToList();

        }

        public async Task<IList<ToiletResponseDTO>> GetAllInProgressToiletRequestsAsync()
        {
            var toiletRequests = await _dbContext.ToiletRequests
                .Include(tr => tr.Team)
                .ThenInclude(t => t.Room)
                .Where(tr => tr.Status == ToiletRequestStatus.InProgress)
                .ToListAsync();
            return toiletRequests.Select(MapToDTO).ToList();

        }

        public async Task<IList<ToiletResponseDTO>> GetAllCompletedToiletRequestsAsync()
        {
            var toiletRequests = await _dbContext.ToiletRequests
                .Include(tr => tr.Team)
                .ThenInclude(t => t.Room)
                .Where(tr => tr.Status == ToiletRequestStatus.Completed)
                .ToListAsync();

            return toiletRequests.Select(MapToDTO).ToList();

        }

        public async Task<ToiletResponseDTO> GetToiletRequestByIdAsync(string id)
        {
            var toiletRequest = await _dbContext.ToiletRequests
                .Include(tr => tr.Team)
                .ThenInclude(t => t.Room)
                .FirstOrDefaultAsync(tr => tr.Id == id);
            if (toiletRequest == null)
            {
                return null;
            }
            return MapToDTO(toiletRequest);
        }

        public async Task<ToiletResponseDTO> UpdateToiletRequestAsync(string id, ToiletRequestDTO toiletRequest)
        {
            var existingToiletRequest = await _dbContext.ToiletRequests.FindAsync(id);
            if (existingToiletRequest == null)
            {
                return null;
            }
            existingToiletRequest.IsMale = toiletRequest.IsMale;
            existingToiletRequest.IsUrgent = toiletRequest.IsUrgent;
            existingToiletRequest.Comment = toiletRequest.Comment;
            existingToiletRequest.StatusChangedBy = toiletRequest.StatusChangedBy;

            await _dbContext.SaveChangesAsync();

            await NotifyToiletRequestStatusChange();
            return MapToDTO(existingToiletRequest);
        }

        public async Task<ToiletResponseDTO> UpdateToiletRequestStatusAsync(ToiletRequestStatusUpdateDTO toiletRequest)
        {
            var existingToiletRequest = await _dbContext.ToiletRequests.FindAsync(toiletRequest.Id);
            if (existingToiletRequest == null)
            {
                return null;
            }
            existingToiletRequest.Status = toiletRequest.Status;
            existingToiletRequest.StatusChangedAt = DateTime.UtcNow;
            existingToiletRequest.StatusChangedBy = toiletRequest.StatusUpdatedBy;
            existingToiletRequest.Comment = toiletRequest.Comment;

            await _dbContext.SaveChangesAsync();
            await NotifyToiletRequestStatusChange();
            return MapToDTO(existingToiletRequest);
        }

        private async Task NotifyToiletRequestStatusChange()
        {
            var pendingRequestsTask = Task.Run(async () =>
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<NucpaDbContext>();
                    var tr = await dbContext.ToiletRequests
                        .Include(tr => tr.Team)
                        .ThenInclude(t => t.Room)
                        .Where(tr => tr.Status == ToiletRequestStatus.Pending)
                        .ToListAsync();
                    return tr.Select(MapToDTO).ToList();
                }
            });

            var inProgressRequestsTask = Task.Run(async () =>
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<NucpaDbContext>();
                    var tr = await dbContext.ToiletRequests
                        .Include(tr => tr.Team)
                        .ThenInclude(t => t.Room)
                        .Where(tr => tr.Status == ToiletRequestStatus.InProgress)
                        .ToListAsync();
                    return tr.Select(MapToDTO).ToList();
                }
            });

            var completedRequestsTask = Task.Run(async () =>
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<NucpaDbContext>();
                    var tr = await dbContext.ToiletRequests
                        .Include(tr => tr.Team)
                        .ThenInclude(t => t.Room)
                        .Where(tr => tr.Status == ToiletRequestStatus.Completed)
                        .ToListAsync();
                    return tr.Select(MapToDTO).ToList();
                }
            });

            var pendingRequests = await pendingRequestsTask;
            var inProgressRequests = await inProgressRequestsTask;
            var completedRequests = await completedRequestsTask;

            await _hubContext.Clients.All.SendAsync("ReceiveToiletRequestUpdates", new
            {
                Pending = pendingRequests,
                InProgress = inProgressRequests,
                Completed = completedRequests
            });
        }

        private static ToiletResponseDTO MapToDTO(ToiletRequest tr)
        {
            return new ToiletResponseDTO
            {
                Id = tr.Id,
                TeamId = tr.TeamId,
                IsMale = tr.IsMale,
                IsUrgent = tr.IsUrgent,
                Comment = tr.Comment,
                Status = tr.Status.ToString(),
                StatusChangedAt = tr.StatusChangedAt,
                StatusChangedBy = tr.StatusChangedBy,
                Timestamp = tr.Timestamp,
                TeamName = tr.Team?.CodeforcesHandle,
                RoomName = tr.Team?.Room?.Name
            };
        }
    }
}
