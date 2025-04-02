using System.Text.Json.Serialization;
using NucpaBalloonsApi.Models.Common;

namespace NucpaBalloonsApi.Models.SystemModels
{
    public class ProblemBalloonMap : BaseEntity
    {
        public string AdminSettingsId { get; set; }
        [JsonIgnore]
        public AdminSettings? AdminSettings { get; set; } 
        public string ProblemIndex { get; set; } = string.Empty;
        public string BalloonColor { get; set; } = string.Empty;
    }
}
