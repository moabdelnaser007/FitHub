using FitHubBackendAPI.DTOs.Bookings;
using FitHubBackendAPI.Services.Interfaces;
using FitHubBackendAPI.Services.Interfaces.UserServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims; // ✅ ضرورية عشان ClaimTypes

namespace FitHubBackendAPI.Controllers.User_Controller
{
    [Route("api/bookings")]
    [ApiController]
    [Authorize] // 🔒 لازم يكون مسجل دخول عشان يحجز
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingsController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        // ==========================================================
        // Endpoint: إنشاء حجز جديد
        // URL: POST /api/bookings
        // ==========================================================
        [HttpPost]
        public async Task<IActionResult> CreateBooking([FromBody] CreateBookingDto dto)
        {
            // 1. استخراج الـ User ID من التوكن
            
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            // لو التوكن سليم بس مفيش فيه ID (حالة نادرة جداً)
            if (userIdClaim == null)
                return Unauthorized();

            // تحويل الـ ID لرقم
            int userId = int.Parse(userIdClaim.Value);

            // 2. استدعاء السيرفيس لتنفيذ الحجز
            var result = await _bookingService.CreateBookingAsync(userId, dto);

            // 3. الرد حسب النتيجة
            if (!result.IsSuccess)
                return BadRequest(result); // لو فيه مشكلة (رصيد غير كافي، فرع غلط، إلخ)

            return Ok(result); // الحجز تم ورجعنا كود الحجز
        }
    }
}
