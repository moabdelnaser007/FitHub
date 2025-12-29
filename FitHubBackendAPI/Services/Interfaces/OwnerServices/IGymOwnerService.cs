using FitHubBackendAPI.DTOs.OwnerWallet;
using FitHubBackendAPI.ViewModels;

namespace FitHubBackendAPI.Services.Interfaces.OwnerServices
{
    public interface IGymOwnerService
    {
        Task<int> GetCountBranchVisits(int userId);
        Task<int> GetCountOwnerSubscriptions(int userId);
        Task<int> GetCountOwnerBranches(int userId);
        Task<ResponseViewModel<OwnerDashBoardDto>> GetOwnerDashboardData(int userId);
    }
}
