namespace NucpaBalloonsApi.Models.DTOs
{
    public class BalloonRequestDTO
    {
        public string Id { get; set; }
        public long SubmissionId { get; set; }
        public int ContestId { get; set; }
        public string TeamId { get; set; }
        public string TeamName { get; set; }
        public char ProblemIndex { get; set; }
        public string BalloonColor { get; set; }
        public string Status { get; set; }
        public DateTime Timestamp { get; set; }
        public DateTime? StatusChangedAt { get; set; }
        public string? StatusChangedBy { get; set; }
    }
}