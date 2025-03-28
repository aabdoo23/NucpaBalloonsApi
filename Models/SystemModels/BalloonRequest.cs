using Microsoft.EntityFrameworkCore;
using NucpaBalloonsApi.Models.Common;

namespace NucpaBalloonsApi.Models.SystemModels
{
    [Index(nameof(SubmissionId), IsUnique = true)]
    public class BalloonRequest : BaseEntity
    {
        public long SubmissionId { get; set; }
        public string TeamId { get; set; }
        public Team Team { get; set; }
        public char ProblemIndex { get; set; } 
        public string BalloonColor { get; set; }
        public DateTime Timestamp { get; set; }
        public BalloonStatus Status { get; set; }
        public DateTime? PickedUpAt { get; set; }
        public string? PickedUpBy { get; set; }
        public DateTime? DeliveredAt { get; set; }
        public string? DeliveredBy { get; set; }
    }

    public enum BalloonStatus
    {
        Pending,
        PickedUp,
        Delivered
    }
}