using Microsoft.EntityFrameworkCore;
using NucpaBalloonsApi.Models.SystemModels;

namespace NucpaBalloonsApi
{
    public class NucpaDbContext(DbContextOptions<NucpaDbContext> options) : DbContext(options)
    {
        public DbSet<AdminSettings> AdminSettings { get; set; }
        public DbSet<BalloonRequest> BalloonRequests { get; set; }
        public DbSet<ProblemBalloonMap> ProblemBalloonMaps { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Team> Teams { get; set; }
    }
}
