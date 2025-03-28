using NucpaBalloonsApi.Interfaces.Services;
using NucpaBalloonsApi.Models.Codeforces;
using System.Security.Cryptography;
using System.Text;

namespace NucpaBalloonsApi.Services;

public class CodeforcesApiService : ICodeforcesApiService
{
    private readonly HttpClient _httpClient;
    private readonly IAdminSettingsService _adminSettingsService;
    private readonly IBalloonService _balloonService;
    private readonly ILogger<CodeforcesApiService> _logger;
    private string _apiKey = string.Empty;
    private string _apiSecret = string.Empty;
    private readonly string _baseUrl = "https://codeforces.com/api";

    public CodeforcesApiService(
        HttpClient httpClient,
        ILogger<CodeforcesApiService> logger,
        IAdminSettingsService adminSettingsService,
        IBalloonService balloonService)
    {
        _httpClient = httpClient;
        _adminSettingsService = adminSettingsService
            ?? throw new ArgumentNullException(nameof(adminSettingsService));
        _balloonService = balloonService
            ?? throw new ArgumentNullException(nameof(balloonService));
        _logger = logger;

        InitializeApiKeys().GetAwaiter().GetResult();
    }

    private async Task InitializeApiKeys()
    {
        var settings = await _adminSettingsService.GetActiveAdminSettings();
        _apiKey = settings.CodeforcesApiKey ?? throw new ArgumentNullException(nameof(settings.CodeforcesApiKey));
        _apiSecret = settings.CodeforcesApiSecret ?? throw new ArgumentNullException(nameof(settings.CodeforcesApiSecret));
    }

    private string GenerateRandomString(int length)
    {
        const string chars = "abcdefghijklmnopqrstuvwxyz0123456789";
        var random = new Random();
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }

    private string GenerateApiSignature(string method, Dictionary<string, string> parameters)
    {
        parameters["apiKey"] = _apiKey;
        parameters["time"] = ((int)DateTimeOffset.UtcNow.ToUnixTimeSeconds()).ToString();

        var sortedParams = string.Join("&", parameters.OrderBy(p => p.Key)
            .Select(p => $"{p.Key}={p.Value}"));

        var randomStr = GenerateRandomString(6);

        var stringToSign = $"{randomStr}/{method}?{sortedParams}#{_apiSecret}";
        var hashBytes = SHA512.HashData(Encoding.UTF8.GetBytes(stringToSign));
        var hash = Convert.ToHexStringLower(hashBytes);

        return $"{randomStr}{hash}";
    }

    private async Task<T?> MakeApiRequest<T>(string method, Dictionary<string, string> parameters)
    {
        try
        {
            var apiSig = GenerateApiSignature(method, parameters);
            parameters["apiSig"] = apiSig;

            var queryString = string.Join("&", parameters.Select(p => $"{Uri.EscapeDataString(p.Key)}={Uri.EscapeDataString(p.Value)}"));
            var url = $"{_baseUrl}/{method}?{queryString}";

            _logger.LogInformation("Making API request to: {Url}", url);

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<ApiResponse<T>>();
            if (result?.Status != "OK")
            {
                _logger.LogError("API request failed with status: {Status}, comment: {Comment}",
                    result?.Status, result?.Comment);
                return default;
            }
            return result.Result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error making API request to {Method}", method);
            throw;
        }
    }

    public async Task<ContestStandings?> FetchContestStandings(int contestId)
    {
        try
        {
            var parameters = new Dictionary<string, string>
            {
                ["contestId"] = contestId.ToString()
            };

            var standings = await MakeApiRequest<ContestStandings>("contest.standings", parameters);
            return standings;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching contest standings for contest {ContestId}", contestId);
            throw;
        }
    }

    public async Task<List<Submission>> FetchNewSubmissions(int contestId)
    {
        try
        {
            var parameters = new Dictionary<string, string>
            {
                ["contestId"] = contestId.ToString(),
                ["from"] = "1",
                ["count"] = "100"
            };

            var submissions = await MakeApiRequest<List<Submission>>("contest.status", parameters);
            if (submissions == null) return new List<Submission>();

            var correctSubmissions = submissions
                .Where(s => s.Verdict == "OK")
                .ToList();

            await _balloonService.ProcessNewSubmissions(correctSubmissions);

            return correctSubmissions;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching new submissions for contest {ContestId}", contestId);
            throw;
        }
    }
}
