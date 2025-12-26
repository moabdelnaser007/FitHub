using FitHubBackendAPI.DTOs.Bookings;
using FitHubBackendAPI.ViewModels;

namespace FitHubBackendAPI.Services.Interfaces.UserServices
{
    public interface IBookingService
    {
        Task<ResponseViewModel<string>> CreateBookingAsync(int userId, CreateBookingDto dto);
        Task<ResponseViewModel<IEnumerable<BookingHistoryDto>>> GetMyBookingsAsync(int userId);
        Task<ResponseViewModel<BookingDetailsDto>> GetBookingDetailsAsync(int userId, int bookingId);
        Task<ResponseViewModel<bool>> CancelBookingAsync(int userId, int bookingId);
    }
}
