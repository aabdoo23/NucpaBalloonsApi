using NucpaBalloonsApi.Models.Common;
using System.Text.Json.Serialization;

namespace NucpaBalloonsApi.Models.SystemModels
{
    public class Room : BaseEntity
    {
        public int? Capacity { get; set; }
        public bool? IsAvailable { get; set; }
        public string AdminSettingsId { get; set; }
        
        [JsonIgnore]
        public virtual AdminSettings AdminSettings { get; set; }
    }
}
