using NucpaBalloonsApi.Models.SystemModels;

namespace NucpaBalloonsApi.Models.Requests.BalloonRequest;

public class BalloonStatusUpdateRequest
{
    public string Id { get; set; }
    public BalloonStatus Status { get; set; }
    public string? DeliveredBy { get; set; }
} 