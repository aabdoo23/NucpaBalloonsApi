namespace NucpaBalloonsApi.Models.Common
{
    public abstract class StatusableEntity : BaseEntity
    {
        public DateTime? StatusChangedAt { get; set; }
        public string? StatusChangedBy { get; set; }
        public DateTime Timestamp { get; set; }

    }
}
