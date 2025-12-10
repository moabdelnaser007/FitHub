using FitHubBackendAPI.DTOs.Subscriptions;
using FitHubBackendAPI.ViewModels;

namespace FitHubBackendAPI.Services.Interfaces.UserServices
{
    public interface ISubscriptionService
    {
        // شراء اشتراك جديد
        Task<ResponseViewModel<bool>> PurchaseSubscriptionAsync(int userId, PurchaseSubscriptionDto dto);

        // إلغاء اشتراك
        Task<ResponseViewModel<bool>> CancelSubscriptionAsync(int userId, int subscriptionId);

        // عرض اشتراكاتي
        Task<ResponseViewModel<List<MySubscriptionDto>>> GetMySubscriptionsAsync(int userId);
    }
}
