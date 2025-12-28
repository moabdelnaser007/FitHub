using FitHubBackendAPI.DTOs.Subscriptions;
using FitHubBackendAPI.ViewModels;

namespace FitHubBackendAPI.Services.Interfaces.UserServices
{
    public interface ISubscriptionService
    {
        Task<ResponseViewModel<bool>> CreateAsync(int userId, CreateSubscriptionDto dto);
        Task<ResponseViewModel<List<SubscriptionListDto>>> GetMyAsync(int userId);
        Task<ResponseViewModel<SubscriptionDetailsDto>> GetByIdAsync(int userId, int subscriptionId);
        Task<ResponseViewModel<bool>> CancelAsync(int userId, int subscriptionId);
        Task<ResponseViewModel<IEnumerable<SubscriptionListDto>>> GetActiveSubscriptionsAsync(int userId, int branchId);
        Task<ResponseViewModel<IEnumerable<SubscriptionListDto>>> GetBranchSubscriptionsAsync(int branchId);
    }
}
