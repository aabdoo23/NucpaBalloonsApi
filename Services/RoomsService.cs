using NucpaBalloonsApi.Interfaces.Repositories.Common;
using NucpaBalloonsApi.Interfaces.Services;
using NucpaBalloonsApi.Models.Requests.Rooms;
using NucpaBalloonsApi.Models.SystemModels;

namespace NucpaBalloonsApi.Services
{
    public class RoomsService(IBaseRepository<Room> roomsRepository, IAdminSettingsService adminSettingsService) : IRoomsService
    {
        private readonly IBaseRepository<Room> _roomsRepository = roomsRepository
            ?? throw new ArgumentNullException(nameof(roomsRepository));
        private readonly IAdminSettingsService _adminSettingsService = adminSettingsService
            ?? throw new ArgumentNullException(nameof(adminSettingsService));

        public async Task<Room> CreateAsync(RoomCreateRequestDTO room)
        {
            // Check if a room with this name already exists
            var existingRoom = await _roomsRepository.GetByIdAsync(room.Name);
            if (existingRoom != null)
            {
                throw new InvalidOperationException($"A room with the name '{room.Name}' already exists.");
            }

            var adminSettings = await _adminSettingsService.GetActiveAdminSettings();
            var newRoom = new Room
            {
                Id = room.Name,
                Capacity = room.Capacity,
                IsAvailable = true,
                AdminSettingsId = adminSettings.Id
            };
            return await _roomsRepository.InsertAsync(newRoom);
        }

        public async Task<List<Room>> GetAllAsync()
        {
            return (await _roomsRepository.GetAllAsync()).ToList();
        }

        public Task<Room?> GetByIdAsync(string id)
        {
            return _roomsRepository.GetByIdAsync(id);
        }

        public async Task DeleteAsync(string roomId)
        {
            var room = await _roomsRepository.GetByIdAsync(roomId);
            if (room == null)
            {
                throw new InvalidOperationException($"A room with the id '{roomId}' does not exist.");
            }
            await _roomsRepository.DeleteAsync(roomId);
        }
    }
}
