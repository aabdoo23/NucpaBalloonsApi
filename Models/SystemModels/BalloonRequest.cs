using Microsoft.EntityFrameworkCore;
using NucpaBalloonsApi.Models.Common;

namespace NucpaBalloonsApi.Models.SystemModels
{
    [Index(nameof(SubmissionId), IsUnique = true)]
    public class BalloonRequest : StatusableEntity
    {
        public long SubmissionId { get; set; }
        public int ContestId { get; set; }
        public string TeamId { get; set; }
        public Team Team { get; set; }
        public char ProblemIndex { get; set; } 
        public string BalloonColor { get; set; }
        public BalloonStatus Status { get; set; }
    }

    public enum BalloonStatus
    {
        Pending,
        ReadyForPickup,
        PickedUp,
        Delivered
    }
}