using NucpaBalloonsApi.Models.SystemModels;

namespace NucpaBalloonsApi.Models.Requests.ToiletRequest
{
    public class ToiletRequestStatusUpdateDTO
    {
        public string Id { get; set; }
        public ToiletRequestStatus Status { get; set; }
        public string StatusUpdatedBy { get; set; }
        public string Comment { get; set; } = string.Empty;
    }
}
