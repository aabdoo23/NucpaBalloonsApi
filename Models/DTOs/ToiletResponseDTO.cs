namespace NucpaBalloonsApi.Models.DTOs
{
    public class ToiletResponseDTO
    {
        public string Id { get; set; }
        public string TeamId { get; set; }
        public string TeamName { get; set; }
        public string RoomName { get; set; }
        public string Status { get; set; }
        public bool IsMale { get; set; }
        public bool IsUrgent { get; set; }
        public string Comment { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public DateTime? StatusChangedAt { get; set; }
        public string? StatusChangedBy { get; set; }
    }
}
