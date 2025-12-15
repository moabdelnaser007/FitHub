using FitHubBackendAPI.DTOs.Bookings;
using FitHubBackendAPI.ViewModels;

namespace FitHubBackendAPI.Services.Interfaces.UserServices
{
    public interface IBookingService
    {
        // دالة الحجز: بتاخد داتا الحجز ورقم اليوزر، وبترجع رقم الحجز (Booking Code)
        Task<ResponseViewModel<string>> CreateBookingAsync(int userId, CreateBookingDto dto);

        // for Booking History
        Task<ResponseViewModel<IEnumerable<BookingHistoryDto>>> GetUserBookingsAsync(int userId);
    }
}
