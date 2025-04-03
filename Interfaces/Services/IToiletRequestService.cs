using NucpaBalloonsApi.Models.DTOs;
using NucpaBalloonsApi.Models.Requests.ToiletRequest;
using NucpaBalloonsApi.Models.SystemModels;

namespace NucpaBalloonsApi.Interfaces.Services
{
    public interface IToiletRequestService
    {
        Task<ToiletResponseDTO> CreateToiletRequestAsync(ToiletRequestDTO toiletRequest);
        Task<ToiletResponseDTO> GetToiletRequestByIdAsync(string id);
        Task<ToiletResponseDTO> UpdateToiletRequestAsync(string id, ToiletRequestDTO toiletRequest);
        Task<bool> DeleteToiletRequestAsync(string id);
        Task<IList<ToiletResponseDTO>> GetAllToiletRequestsAsync();
        Task<IList<ToiletResponseDTO>> GetAllPendingToiletRequestsAsync();
        Task<IList<ToiletResponseDTO>> GetAllInProgressToiletRequestsAsync();
        Task<IList<ToiletResponseDTO>> GetAllCompletedToiletRequestsAsync();
        Task<ToiletResponseDTO> UpdateToiletRequestStatusAsync(ToiletRequestStatusUpdateDTO toiletRequest);
    }
}
