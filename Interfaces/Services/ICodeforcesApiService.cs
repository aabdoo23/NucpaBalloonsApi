using NucpaBalloonsApi.Models.Codeforces;

namespace NucpaBalloonsApi.Interfaces.Services;

public interface ICodeforcesApiService
{
    Task<ContestStandings?> FetchContestStandings(int contestId);
    Task<List<Submission>> FetchNewSubmissions(int contestId);
}
