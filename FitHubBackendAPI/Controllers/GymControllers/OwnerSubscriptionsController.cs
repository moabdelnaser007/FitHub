using FitHubBackendAPI.Entities.Models;
using FitHubBackendAPI.Services.Interfaces.OwnerServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FitHubBackendAPI.Controllers.GymControllers
{
    [ApiController]
    [Route("api/owner")]
    [Authorize(Roles = "Owner")]
    public class OwnerSubscriptionsController : ControllerBase
    {
        private readonly IOwnerSubscriptionService _service;

        public OwnerSubscriptionsController(IOwnerSubscriptionService service)
        {
            _service = service;
        }

        private int GetOwnerId()
        {
            return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        }

        // 🔹 GET owner subscriptions
        [HttpGet("subscriptions")]
        public async Task<IActionResult> GetSubscriptions()
        {
            var ownerId = GetOwnerId();
            var result = await _service.GetOwnerSubscriptionsAsync(ownerId);
            return Ok(result);
        }

        // 🔹 GET owner bookings
        [HttpGet("bookings")]
        public async Task<IActionResult> GetBookings()
        {
            var ownerId = GetOwnerId();
            var result = await _service.GetOwnerBookingsAsync(ownerId);
            return Ok(result);
        }
    }

}
