using NucpaBalloonsApi.Interfaces.Repositories;
using NucpaBalloonsApi.Interfaces.Services;
using NucpaBalloonsApi.Models.Requests.Rooms;
using NucpaBalloonsApi.Models.SystemModels;

namespace NucpaBalloonsApi.Services
{
    public class RoomsService(IRoomRepository roomsRepository, IAdminSettingsService adminSettingsService) : IRoomsService
    {
        private readonly IRoomRepository _roomsRepository = roomsRepository
            ?? throw new ArgumentNullException(nameof(roomsRepository));
        private readonly IAdminSettingsService _adminSettingsService = adminSettingsService
            ?? throw new ArgumentNullException(nameof(adminSettingsService));

        public async Task<Room> CreateAsync(RoomCreateRequestDTO room)
        {
            // Check if a room with this name already exists
            var existingRoom = await _roomsRepository.GetByRoomName(room.Name);
            if (existingRoom != null)
            {
                throw new InvalidOperationException($"A room with the name '{room.Name}' already exists.");
            }

            var adminSettings = await _adminSettingsService.GetActiveAdminSettings();
            var newRoom = new Room
            {
                Id = Guid.NewGuid().ToString(),
                Name = room.Name,
                Capacity = room.Capacity,
                IsAvailable = true,
                AdminSettingsId = adminSettings.Id
            };
            return await _roomsRepository.InsertAsync(newRoom);
        }

        public async Task<List<Room>> GetAllAsync()
        {
            var activeAdminSettings = await _adminSettingsService.GetActiveAdminSettings();
            return (await _roomsRepository.GetAllAsync()).Where(x => x.AdminSettingsId == activeAdminSettings.Id).ToList();
        }

        public Task<Room?> GetByIdAsync(string id)
        {
            return _roomsRepository.GetByIdAsync(id);
        }

        public async Task<Room?> UpdateAsync(Room room)
        {
            return await _roomsRepository.UpdateAsync(room);
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
