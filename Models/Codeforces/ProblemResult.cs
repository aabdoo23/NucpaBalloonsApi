using System.Text.Json.Serialization;

namespace NucpaBalloonsApi.Models.Codeforces;

public class ProblemResult
{
    public decimal Points { get; set; }
    public int RejectedAttemptCount { get; set; }
    public string Type { get; set; } = string.Empty;
    public long? BestSubmissionTimeSeconds { get; set; }
} 