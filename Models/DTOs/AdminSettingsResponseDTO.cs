using NucpaBalloonsApi.Models.SystemModels;

namespace NucpaBalloonsApi.Models.DTOs
{
    public class AdminSettingsResponseDTO
    {
        public string Id { get; set; }
        public string AdminUsername { get; set; }
        public int ContestId { get; set; }
        public string? CodeforcesApiKey { get; set; }
        public string? CodeforcesApiSecret { get; set; }
        public bool IsEnabled { get; set; }
        public IList<Room> Rooms { get; set; } = new List<Room>();
        public IList<Team> Teams { get; set; } = new List<Team>();
        public IList<ProblemBalloonMap> ProblemBalloonMaps { get; set; } = new List<ProblemBalloonMap>();

        public static AdminSettingsResponseDTO FromEntity(AdminSettings entity)
        {
            return new AdminSettingsResponseDTO
            {
                Id = entity.Id,
                AdminUsername = entity.AdminUsername,
                ContestId = entity.ContestId,
                CodeforcesApiKey = entity.CodeforcesApiKey,
                CodeforcesApiSecret = entity.CodeforcesApiSecret,
                IsEnabled = entity.IsEnabled,
                Rooms = entity.Rooms,
                Teams = entity.Teams,
                ProblemBalloonMaps = entity.ProblemBalloonMaps
            };
        }
    }
} 