using FitHubBackendAPI.Services.Interfaces.OwnerServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FitHubBackendAPI.Controllers.GymControllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Owner")]
    public class GymOwnerController : ControllerBase
    {
        private readonly IGymOwnerService _gymOwnerService;
        public GymOwnerController(IGymOwnerService gymOwnerService)
        {
            _gymOwnerService = gymOwnerService;
        }
        private int GetUserId()
                => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboardData() {
            var userId = GetUserId();
            var result = await _gymOwnerService.GetOwnerDashboardData(userId);
            return Ok(result);
        }
    }
}
