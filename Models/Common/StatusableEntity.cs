namespace NucpaBalloonsApi.Models.Common
{
    public abstract class StatusableEntity
    {
        public DateTime? StatusChangedAt { get; set; }
        public string? StatusChangedBy { get; set; }
        public DateTime TimeStamp { get; set; }

    }
}
