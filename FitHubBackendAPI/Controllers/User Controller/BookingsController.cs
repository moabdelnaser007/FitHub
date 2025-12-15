using FitHubBackendAPI.DTOs.Bookings;
using FitHubBackendAPI.Services.Interfaces.UserServices; // اتأكد ان ده المسار الصح للانترفيس بتاعك
using FitHubBackendAPI.ViewModels; // عشان ResponseViewModel
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FitHubBackendAPI.Controllers.User_Controller
{
    [Route("api/bookings")]
    [ApiController]
    [Authorize] // 🔒 لازم يكون مسجل دخول عشان يشوف حجوزاته
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingsController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        // ==========================================================
        // 1. إنشاء حجز جديد
        // URL: POST /api/bookings
        // ==========================================================
        [HttpPost]
        public async Task<IActionResult> CreateBooking([FromBody] CreateBookingDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();

            int userId = int.Parse(userIdClaim.Value);

            var result = await _bookingService.CreateBookingAsync(userId, dto);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        // ==========================================================
        // 2. عرض سجل حجوزات اليوزر (My Bookings)
        // URL: GET /api/bookings/my
        // ==========================================================
        [HttpGet("my")]
        public async Task<IActionResult> GetMyBookings()
        {
            // 1. نجيب الايدي من التوكن
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();

            int userId = int.Parse(userIdClaim.Value);

            // 2. نكلم السيرفيس تجيب الداتا
            var result = await _bookingService.GetUserBookingsAsync(userId);

            // 3. نرد بالنتيجة
            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }
    }
}