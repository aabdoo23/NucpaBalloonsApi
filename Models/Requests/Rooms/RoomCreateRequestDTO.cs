namespace NucpaBalloonsApi.Models.Requests.Rooms
{
    public class RoomCreateRequestDTO
    {
        public string Name { get; set; } = string.Empty;
        public int? Capacity { get; set; }
        public bool? IsAvailable { get; set; }
    }
}