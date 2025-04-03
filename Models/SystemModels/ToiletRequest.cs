using System.ComponentModel.DataAnnotations.Schema;
using NucpaBalloonsApi.Models.Common;

namespace NucpaBalloonsApi.Models.SystemModels
{
    public class ToiletRequest : StatusableEntity
    {
        public string TeamId { get; set; }
        public virtual Team? Team { get; set; }
        public ToiletRequestStatus Status { get; set; } = ToiletRequestStatus.Pending;
        public bool IsMale { get; set; }
        public bool IsUrgent { get; set; }
        public string Comment { get; set; } = string.Empty;
        [NotMapped]
        public string? RoomName { get => Team?.RoomName; }
    }

    public enum ToiletRequestStatus
    {
        Pending,
        InProgress,
        Completed
    }
}
