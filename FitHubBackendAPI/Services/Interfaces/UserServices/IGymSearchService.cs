using FitHubBackendAPI.DTOs.UserDTOs;

namespace FitHubBackendAPI.Services.Interfaces.UserServices
{
    public interface IGymSearchService
    {
        Task<List<GymSearchResultDto>> SearchAsync(GymSearchQueryDto query);

    }
}
