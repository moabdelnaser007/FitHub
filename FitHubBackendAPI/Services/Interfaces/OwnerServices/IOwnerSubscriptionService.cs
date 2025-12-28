using FitHubBackendAPI.DTOs.Bookings;
using FitHubBackendAPI.DTOs.Subscriptions;
using FitHubBackendAPI.ViewModels;

namespace FitHubBackendAPI.Services.Interfaces.OwnerServices
{
    public interface IOwnerSubscriptionService
    {
        Task<ResponseViewModel<List<SubscriptionForOwnerDto>>> GetOwnerSubscriptionsAsync(int ownerId);
        Task<ResponseViewModel<List<BookingForOwnerDto>>> GetOwnerBookingsAsync(int ownerId);
    }
}
