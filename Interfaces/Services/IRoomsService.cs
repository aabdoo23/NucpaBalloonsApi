using NucpaBalloonsApi.Models.Requests.Rooms;
using NucpaBalloonsApi.Models.SystemModels;

namespace NucpaBalloonsApi.Interfaces.Services
{
    public interface IRoomsService
    {
        Task<List<Room>> GetAllAsync();
        Task<Room?> GetByIdAsync(string id);
        Task<Room> CreateAsync(RoomCreateRequestDTO room);
        Task DeleteAsync(string roomId);
    }
}
