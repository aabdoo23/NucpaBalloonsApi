using NucpaBalloonsApi.Models.SystemModels;

namespace NucpaBalloonsApi.Interfaces.Services
{
    public interface IToiletRequestService
    {
        Task<ToiletRequest> CreateToiletRequestAsync(ToiletRequestDTO toiletRequest);
        Task<ToiletRequest> GetToiletRequestByIdAsync(string id);
        Task<ToiletRequest> UpdateToiletRequestAsync(string id, ToiletRequestDTO toiletRequest);
        Task<bool> DeleteToiletRequestAsync(string id);
        Task<IList<ToiletRequest>> GetAllToiletRequestsAsync();
        Task<ToiletRequest> UpdateToiletRequestStatus(string id, ToiletRequestStatusUpdateDTO toiletRequest);
    }
}
