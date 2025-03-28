namespace NucpaBalloonsApi.Models.Codeforces;

public class Party
{
    public int ContestId { get; set; }
    public List<Member> Members { get; set; } = new();
    public string ParticipantType { get; set; } = string.Empty;
    public int? TeamId { get; set; }
    public string? TeamName { get; set; }
    public bool Ghost { get; set; }
    public int? Room { get; set; }
    public long? StartTimeSeconds { get; set; }
}
