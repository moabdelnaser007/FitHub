using FitHubBackendAPI.DTOs.Reviews;
using FitHubBackendAPI.ViewModels;

namespace FitHubBackendAPI.Services.Interfaces.UserServices
{
    public interface IReviewService
    {
        Task<ResponseViewModel<bool>> LeaveReviewAsync(int userId, LeaveReviewDto dto);
        Task<IEnumerable<GetAllBranchRevewsDto>> GetAllBrancheReviewsAsync(int branchId);
    }
}