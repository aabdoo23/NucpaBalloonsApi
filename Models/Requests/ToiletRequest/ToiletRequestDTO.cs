namespace NucpaBalloonsApi.Models.Requests.ToiletRequest
{
    public class ToiletRequestDTO
    {
        public string TeamId { get; set; } = string.Empty;
        public bool IsMale { get; set; }
        public bool IsUrgent { get; set; }
        public string Comment { get; set; } = string.Empty;
        public string? StatusChangedBy { get; set; } = string.Empty;
    }
}
