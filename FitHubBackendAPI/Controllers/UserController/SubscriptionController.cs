using FitHubBackendAPI.DTOs.Subscriptions;
using FitHubBackendAPI.Services.Interfaces; 
using FitHubBackendAPI.Services.Interfaces.UserServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FitHubBackendAPI.Controllers.User_Controller
{
    [Route("api/user/subscriptions")]
    [ApiController]
    [Authorize]
    public class SubscriptionController : ControllerBase
    {
        private readonly ISubscriptionService _subService;

        public SubscriptionController(ISubscriptionService subService)
        {
            _subService = subService;
        }

        // دالة مساعدة لجلب الـ ID من التوكن
        private int GetUserId()
        {
            // ممكن تستخدم السطر ده لو بتجرب من غير توكن مؤقتاً
            // return 3; 
            return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        }

        // ====================================================
        // 1. شراء اشتراك جديد
        // POST: api/user/subscriptions/purchase
        // ====================================================
        [HttpPost("purchase")]
        public async Task<IActionResult> Purchase([FromBody] PurchaseSubscriptionDto dto)
        {
            // ✅ الكود المستقبلي (لما الـ Auth يشتغل شيل الكومنت من السطر ده)
            //get id from token 
            // int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            //int userId = 3; // مؤقتاً لحد الـ Auth
            int userId = GetUserId();

            var result = await _subService.PurchaseSubscriptionAsync(userId, dto);

            if (!result.IsSuccess) return BadRequest(result);
            return Ok(result);
        }

        // ====================================================
        // 2. عرض قائمة اشتراكاتي
        // GET: api/user/subscriptions/my
        // ====================================================
        [HttpGet("my")]
        public async Task<IActionResult> GetMySubscriptions()
        {
            //int userId = 3;
            int userId = GetUserId();

            var result = await _subService.GetMySubscriptionsAsync(userId);

            if (!result.IsSuccess) return BadRequest(result);
            return Ok(result);
        }

        // ====================================================
        // 3. عرض تفاصيل اشتراك معين + سجل مدفوعاته (NEW ✅)
        // GET: api/user/subscriptions/{id}/details
        // ====================================================
        [HttpGet("{id}/details")]
        public async Task<IActionResult> GetSubscriptionDetails(int id)
        {
            int userId = GetUserId();

            // بننده السيرفيس اللي بترجع تفاصيل الكارت + الجدول
            var result = await _subService.GetSubscriptionDetailsAsync(userId, id);

            if (!result.IsSuccess) return BadRequest(result);
            return Ok(result);
        }

        // ====================================================
        // 4. إلغاء اشتراك
        // PATCH: api/user/subscriptions/1/cancel
        // ====================================================
        [HttpPatch("{id}/cancel")]
        public async Task<IActionResult> Cancel(int id)
        {
            //int userId = 3;
            int userId = GetUserId();

            var result = await _subService.CancelSubscriptionAsync(userId, id);

            if (!result.IsSuccess) return BadRequest(result);
            return Ok(result);
        }
    }
}