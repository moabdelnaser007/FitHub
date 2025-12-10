using FitHubBackendAPI.DTOs.Subscriptions;
using FitHubBackendAPI.Services.Interfaces.UserServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FitHubBackendAPI.Controllers.User_Controller
{
    [Route("api/subscriptions")]
    [ApiController]
    public class SubscriptionController : ControllerBase
    {
        private readonly ISubscriptionService _subService;

        public SubscriptionController(ISubscriptionService subService)
        {
            _subService = subService;
        }

        // POST: api/subscriptions/purchase
        [HttpPost("purchase")]
        public async Task<IActionResult> Purchase([FromBody] PurchaseSubscriptionDto dto)
        {
            // ✅ الكود المستقبلي (لما الـ Auth يشتغل شيل الكومنت من السطر ده)
            //get id from token 
            // int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            int userId = 3; // مؤقتاً لحد الـ Auth
            var result = await _subService.PurchaseSubscriptionAsync(userId, dto);

            if (!result.IsSuccess) return BadRequest(result);
            return Ok(result);
        }

        // GET: api/subscriptions/my
        [HttpGet("my")]
        public async Task<IActionResult> GetMySubscriptions()
        {
            int userId = 3;
            var result = await _subService.GetMySubscriptionsAsync(userId);

            if (!result.IsSuccess) return BadRequest(result);
            return Ok(result);
        }

        // PATCH: api/subscriptions/1/cancel
        [HttpPatch("{id}/cancel")]
        public async Task<IActionResult> Cancel(int id)
        {
            int userId = 3;
            var result = await _subService.CancelSubscriptionAsync(userId, id);

            if (!result.IsSuccess) return BadRequest(result);
            return Ok(result);
        }
    }
}

