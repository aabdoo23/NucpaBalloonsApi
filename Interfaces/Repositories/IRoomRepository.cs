using NucpaBalloonsApi.Interfaces.Repositories.Common;
using NucpaBalloonsApi.Models.SystemModels;

namespace NucpaBalloonsApi.Interfaces.Repositories
{
    public interface IRoomRepository : IBaseRepository<Room>
    {
        Task<Room?> GetByRoomName(string name);
    }
}
