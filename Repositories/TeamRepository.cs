﻿using Microsoft.EntityFrameworkCore;
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
            //_ = LoadTeamCacheAsync();
        }

        public override async Task<IEnumerable<Team>> GetAllAsync()
        {
            return await _context.Teams
                .AsNoTracking()
                .Include(t => t.Room)
                .ToListAsync();
        }

        public override async Task<Team?> GetByIdAsync(string id)
        {
            return await _context.Teams
                .AsNoTracking()
                .Include(t => t.Room)
                .FirstOrDefaultAsync(t => t.Id == id);
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
                .Include(t => t.Room)
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
    }
}