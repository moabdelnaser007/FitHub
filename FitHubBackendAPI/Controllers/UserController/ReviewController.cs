using FitHubBackendAPI.DTOs.Reviews;
using FitHubBackendAPI.Services.Interfaces.UserServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FitHubBackendAPI.Controllers.User_Controller
{
    [Route("api/reviews")]
    [ApiController]
    [Authorize] // 🔒 لازم يكون مسجل دخول
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        // POST: api/reviews
        [HttpPost]

        public async Task<IActionResult> LeaveReview([FromBody] LeaveReviewDto dto)
        {
            // استخراج الـ ID من التوكن
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            int userId = int.Parse(userIdClaim.Value);

            // int userId = 3; // لو بتجرب من غير توكن

            var result = await _reviewService.LeaveReviewAsync(userId, dto);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }
        [HttpGet("branch/{branchId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetBranchReviews(int branchId)
        {
            var reviews = await _reviewService.GetAllBrancheReviewsAsync(branchId);
            return Ok(reviews);
        }
    }
}