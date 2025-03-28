namespace NucpaBalloonsApi.Models.Codeforces;

public class ContestStandings
{
    public Contest Contest { get; set; } = new();
    public List<Problem> Problems { get; set; } = new();
    public List<RanklistRow> Rows { get; set; } = new();
    public List<Submission> NewSubmissions { get; set; } = new();
}
