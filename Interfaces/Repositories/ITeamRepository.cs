﻿using NucpaBalloonsApi.Interfaces.Repositories.Common;
using NucpaBalloonsApi.Models.SystemModels;

namespace NucpaBalloonsApi.Interfaces.Repositories
{
    public interface ITeamRepository : IBaseRepository<Team>
    {
        Task LoadTeamCacheAsync();
        Task<string?> GetTeamIdByCodeforcesHandleAsync(string codeforcesHandle);
    }
}