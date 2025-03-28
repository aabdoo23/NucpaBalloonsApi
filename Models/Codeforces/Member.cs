namespace NucpaBalloonsApi.Models.Codeforces;

public class Member
{
    public string Handle { get; set; } = string.Empty;
    public string? Name { get; set; }
    public string? Rank { get; set; }
    public int? Rating { get; set; }
    public string? Organization { get; set; }
}
