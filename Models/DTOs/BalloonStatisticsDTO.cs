namespace NucpaBalloonsApi.Models.DTOs
{
    public class BalloonStatisticsDTO
    {
        public int TotalPending { get; set; }
        public int TotalDelivered { get; set; }
        public Dictionary<string, int> ColorCounts { get; set; } = new();
    }
}