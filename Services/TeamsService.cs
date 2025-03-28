using NucpaBalloonsApi.Interfaces.Repositories;
using NucpaBalloonsApi.Interfaces.Services;
using NucpaBalloonsApi.Models.SystemModels;

namespace NucpaBalloonsApi.Services
{
    public class TeamsService(ITeamRepository repository, IAdminSettingsService adminSettingsService) : ITeamsService
    {
        private readonly ITeamRepository _repository = repository ??
            throw new ArgumentNullException(nameof(repository));
        private readonly IAdminSettingsService _adminSettingsService = adminSettingsService ??
            throw new ArgumentNullException(nameof(adminSettingsService));

        public async Task<Team> CreateTeam(Team team)
        {
            team.Id = Guid.NewGuid().ToString();
            team.AdminSettingsId = (await _adminSettingsService.GetActiveAdminSettings()).Id;
            return await _repository.InsertAsync(team);
        }

        public async Task<IList<string>> InsertBulkTeams(IList<Team> teams)
        {
            foreach (var team in teams)
            {
                team.Id = Guid.NewGuid().ToString();
                team.AdminSettingsId = (await _adminSettingsService.GetActiveAdminSettings()).Id;
            }
            return await _repository.InsertBulkAsync(teams);
        }

        public async Task DeleteTeamById(string teamId)
        {
            await _repository.DeleteAsync(teamId);
        }

        public async Task<Team> UpdateTeamRoom(string teamId, string roomId)
        {
            var team = await _repository.GetByIdAsync(teamId);
            if (team == null)
                throw new ArgumentNullException(nameof(team));
            team.RoomId = roomId;
            return await _repository.UpdateAsync(team);
        }

        public async Task<Team> GetTeamById(string teamId)
        {
            return await _repository.GetByIdAsync(teamId);
        }
    }
}
