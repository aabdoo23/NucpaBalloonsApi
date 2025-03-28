using System.Text.Json.Serialization;

namespace NucpaBalloonsApi.Models.Codeforces;

public class Submission
{
    public long Id { get; set; }
    public int ContestId { get; set; }
    public long CreationTimeSeconds { get; set; }
    public long RelativeTimeSeconds { get; set; }
    public Problem Problem { get; set; } = new();
    public Party Author { get; set; } = new();
    public string ProgrammingLanguage { get; set; } = string.Empty;
    public string Verdict { get; set; } = string.Empty;
    public string Testset { get; set; } = string.Empty;
    public int PassedTestCount { get; set; }
    public int TimeConsumedMillis { get; set; }
    public int MemoryConsumedBytes { get; set; }

    //for frontend
    [JsonIgnore]
    public string Team => Author.TeamName ?? Author.Members.FirstOrDefault()?.Handle ?? string.Empty;

    [JsonIgnore]
    public string ProblemIndex => Problem.Index;

    [JsonIgnore]
    public DateTime Timestamp => DateTimeOffset.FromUnixTimeSeconds(CreationTimeSeconds).DateTime;
} 