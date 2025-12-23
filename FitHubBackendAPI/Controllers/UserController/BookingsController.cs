using FitHubBackendAPI.DTOs.Bookings;
using FitHubBackendAPI.Services.Interfaces.UserServices;
using FitHubBackendAPI.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FitHubBackendAPI.Controllers.User_Controller
{
    [Route("api/user/bookings")]
    [ApiController]
    [Authorize] // 🔒 أمان: لازم يكون مسجل دخول
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingsController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        // ==========================================
        // Helper: دالة لاستخراج ID اليوزر من التوكن
        // ==========================================
        private int GetUserId()
        {
            return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        }

        // ==========================================================
        // 1. إنشاء حجز جديد (Create Booking)
        // URL: POST /api/user/bookings
        // ==========================================================
        [HttpPost]
        public async Task<IActionResult> CreateBooking([FromBody] CreateBookingDto dto)
        {
            // 1. نجيب الايدي
            int userId = GetUserId();

            // 2. ننفذ الحجز (التحقق فقط بدون خصم حالياً حسب اللوجيك الجديد)
            var result = await _bookingService.CreateBookingAsync(userId, dto);

            // 3. لو فشل (رصيد غير كافي / اشتراك منتهي) نرجع Error
            if (!result.IsSuccess)
                return BadRequest(result);

            // 4. لو نجح نرجع الـ Code
            return Ok(result);
        }

        // ==========================================================
        // 2. عرض سجل حجوزات اليوزر (My Bookings)
        // URL: GET /api/user/bookings/my
        // ==========================================================
        [HttpGet("my")]
        public async Task<IActionResult> GetMyBookings()
        {
            int userId = GetUserId();

            var result = await _bookingService.GetUserBookingsAsync(userId);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        // ==========================================================
        // 3. عرض تفاصيل حجز معين
        // URL: GET /api/user/bookings/{id}
        // ==========================================================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBookingDetails(int id)
        {
            int userId = GetUserId();

            var result = await _bookingService.GetBookingDetailsAsync(userId, id);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        // ==========================================================
        // 4. إلغاء حجز
        // URL: PUT /api/user/bookings/cancel/{id}
        // ==========================================================
        [HttpPut("cancel/{id}")]
        public async Task<IActionResult> CancelBooking(int id)
        {
            int userId = GetUserId();

            var result = await _bookingService.CancelBookingAsync(userId, id);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }
    }
}