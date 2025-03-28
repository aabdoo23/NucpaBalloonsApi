using NucpaBalloonsApi.Models.Common;
using System.Text.Json.Serialization;

namespace NucpaBalloonsApi.Models.SystemModels
{
    public class AdminSettings : BaseEntity
    {
        public string AdminUsername { get; set; }
        public string ContestId { get; set; }
        public string? CodeforcesApiKey { get; set; }
        public string? CodeforcesApiSecret { get; set; }
        public bool IsEnabled { get; set; }

        [JsonIgnore]
        public virtual IList<Room> Rooms { get; set; } = new List<Room>();
        
        [JsonIgnore]
        public virtual IList<Team> Teams { get; set; } = new List<Team>();
        
        [JsonIgnore]
        public virtual IList<ProblemBalloonMap> ProblemBalloonMaps { get; set; } = new List<ProblemBalloonMap>();
    }
}
