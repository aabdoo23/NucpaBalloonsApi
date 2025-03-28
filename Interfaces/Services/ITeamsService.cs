using NucpaBalloonsApi.Models.SystemModels;

namespace NucpaBalloonsApi.Interfaces.Services
{
    public interface ITeamsService
    {
        Task<Team> CreateTeam(Team team);
        Task<Team> UpdateTeamRoom(string teamId, string roomId);
        Task DeleteTeamById(string teamId);
        Task<Team> GetTeamById(string teamId);
    }
}
