using FitHubBackendAPI.DTOs.VisitDTOs;
using FitHubBackendAPI.ViewModels;

namespace FitHubBackendAPI.Services.Interfaces.UserServices
{
    public interface IVisitService
    {
        Task<ResponseViewModel<bool>> CheckInAsync(int staffUserId, CheckInVisitDto dto);
        Task<ResponseViewModel<IEnumerable<VisitHistoryDto>>> GetMyVisitsAsync(int userId);
        Task<ResponseViewModel<IEnumerable<VisitHistoryDto>>> GetBranchVisitsAsync(int branchId, int requesterUserId);
    }
}

