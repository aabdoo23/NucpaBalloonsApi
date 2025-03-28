using System.Text.Json.Serialization;

namespace NucpaBalloonsApi.Models.Codeforces;

public class RanklistRow
{
    public Party Party { get; set; } = new();
    public int Rank { get; set; }
    public decimal Points { get; set; }
    public int Penalty { get; set; }
    public int SuccessfulHackCount { get; set; }
    public int UnsuccessfulHackCount { get; set; }
    public List<ProblemResult> ProblemResults { get; set; } = new();
}
