namespace NucpaBalloonsApi.Models.Requests
{
    public class RoomCreateRequestDTO
    {
        public string Name { get; set; } = string.Empty;
        public int? Capacity { get; set; }
        public bool? IsAvailable { get; set; }
    }
}