using System.Text.Json.Serialization;

namespace NucpaBalloonsApi.Models.Codeforces;

public class Problem
{
    public int ContestId { get; set; }
    public string? ProblemsetName { get; set; }
    public string Index { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public decimal? Points { get; set; }
    public int? Rating { get; set; }
    public List<string> Tags { get; set; } = new();
}