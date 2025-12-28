using FitHubBackendAPI.DTOs.Subscriptions;
using FitHubBackendAPI.Services.Interfaces; 
using FitHubBackendAPI.Services.Interfaces.UserServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FitHubBackendAPI.Controllers.User_Controller
{
    [ApiController]
    [Route("api/subscriptions")]
    [Authorize]
    public class SubscriptionController : ControllerBase
    {
        private readonly ISubscriptionService _service;

        public SubscriptionController(ISubscriptionService service)
        {
            _service = service;
        }

        private int GetUserId()
            => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        [HttpPost]
        public async Task<IActionResult> Create(CreateSubscriptionDto dto)
            => Ok(await _service.CreateAsync(GetUserId(), dto));

        [HttpGet("my")]
        public async Task<IActionResult> My()
            => Ok(await _service.GetMyAsync(GetUserId()));

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> Details(int id)
            => Ok(await _service.GetByIdAsync(GetUserId(), id));

        [HttpPost("{id}/cancel")]
        public async Task<IActionResult> Cancel(int id)
            => Ok(await _service.CancelAsync(GetUserId(), id));

        [HttpGet("GetActiveSubscriptions")]
        public async Task<IActionResult> GetActiveSubscriptions(int branchId)
            => Ok(await _service.GetActiveSubscriptionsAsync(GetUserId(), branchId));
        [Authorize(Roles = "Owner,Admin")]
        [HttpGet("GetBranchSubscriptions/{branchId}")]
        public async Task<IActionResult> GetBranchSubscriptions(int branchId)
            => Ok(await _service.GetBranchSubscriptionsAsync( branchId));
    }
}