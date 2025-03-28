using NucpaBalloonsApi.Models.Common;
using System.Text.Json.Serialization;

namespace NucpaBalloonsApi.Models.SystemModels
{
    public class Team : BaseEntity
    {
        public string CodeforcesHandle { get; set; }
        public string RoomId { get; set; }
        
        [JsonIgnore]
        public virtual Room Room { get; set; }
        
        public string AdminSettingsId { get; set; }
        
        [JsonIgnore]
        public virtual AdminSettings AdminSettings { get; set; }
    }
}
