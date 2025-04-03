using NucpaBalloonsApi.Models.Common;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace NucpaBalloonsApi.Models.SystemModels
{
    public class Team : BaseEntity
    {
        public string CodeforcesHandle { get; set; }
        public string RoomId { get; set; }

        [JsonIgnore]
        public virtual Room Room { get; set; }
        [NotMapped]
        public string RoomName => Room?.Name;
        public string AdminSettingsId { get; set; }

        [JsonIgnore]
        public virtual AdminSettings AdminSettings { get; set; }
    }
}