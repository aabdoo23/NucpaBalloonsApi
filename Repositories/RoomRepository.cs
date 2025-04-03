using Microsoft.EntityFrameworkCore;
using NucpaBalloonsApi.Interfaces.Repositories;
using NucpaBalloonsApi.Models.SystemModels;
using NucpaBalloonsApi.Repositories.Common;

namespace NucpaBalloonsApi.Repositories
{
    public class RoomRepository(NucpaDbContext context) : BaseRepository<Room>(context), IRoomRepository
    {
        private readonly NucpaDbContext _context = context
            ?? throw new ArgumentNullException(nameof(context));
        public Task<Room?> GetByRoomName(string name)
        {
            return _context.Rooms
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Name == name);
        }
    }
}
