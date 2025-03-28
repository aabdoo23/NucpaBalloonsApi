using Microsoft.EntityFrameworkCore;
using NucpaBalloonsApi.Interfaces.Repositories;
using NucpaBalloonsApi.Models.SystemModels;
using NucpaBalloonsApi.Repositories.Common;

namespace NucpaBalloonsApi.Repositories
{
    public class TeamRepository: BaseRepository<Team>, ITeamRepository
    {
        private readonly NucpaDbContext _context;
        private static Dictionary<string, string> _teamCache = [];
        public TeamRepository(NucpaDbContext context) : base(context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _ = LoadTeamCacheAsync();
        }

        public async Task LoadTeamCacheAsync()
        {
            _teamCache = await _context.Teams
                .ToDictionaryAsync(t => t.CodeforcesHandle, t => t.Id);
        }

        public async Task<string?> GetTeamIdByCodeforcesHandleAsync(string codeforcesHandle)
        {
            if (_teamCache.TryGetValue(codeforcesHandle, out var teamId))
            {
                return teamId;
            }

            var team = await _context.Teams
                .AsNoTracking() 
                .FirstOrDefaultAsync(t => t.CodeforcesHandle == codeforcesHandle);

            if (team == null)
            {
                return null;
            }

            _teamCache[codeforcesHandle] = team.Id;
            return team.Id;
        }
       
        public override async Task<Team> InsertAsync(Team entity)
        {
            var team = await base.InsertAsync(entity);
            _teamCache[team.CodeforcesHandle] = team.Id;
            return team;
        }
       
        public override async Task<Team> UpdateAsync(Team entity)
        {
            var existingTeam = await _context.Teams.FindAsync(entity.Id);
            if (existingTeam == null)
            {
                throw new KeyNotFoundException("Team not found.");
            }

            if (existingTeam.CodeforcesHandle != entity.CodeforcesHandle)
            {
                _teamCache.Remove(existingTeam.CodeforcesHandle);
            }

            await base.UpdateAsync(entity);
            _teamCache[entity.CodeforcesHandle] = entity.Id;
            return entity;
        }

        public override async Task DeleteAsync(string id)
        {
            var team = await _context.Teams.FindAsync(id);
            if (team == null)
            {
                throw new KeyNotFoundException("Team not found.");
            }

            await base.DeleteAsync(id);
            _teamCache.Remove(team.CodeforcesHandle);
        }

        public async Task RefreshCacheAsync()
        {
            await LoadTeamCacheAsync();
        }

        public async Task<IList<string>> InsertBulkAsync(IList<Team> teams)
        {
            var teamIds = new List<string>();
            foreach (var team in teams)
            {
                teamIds.Add(team.Id);
                _teamCache[team.CodeforcesHandle] = team.Id;
            }
            await _context.Teams.AddRangeAsync(teams);
            await _context.SaveChangesAsync();
            return teamIds;
        }
    }
}