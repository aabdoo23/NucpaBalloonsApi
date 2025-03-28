using System.Text.Json.Serialization;

namespace NucpaBalloonsApi.Models.Codeforces;

public class ApiResponse<T>
{
    public string Status { get; set; } = string.Empty;
    public T? Result { get; set; }
    public string? Comment { get; set; }
} 