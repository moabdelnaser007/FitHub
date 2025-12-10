using FitHubBackendAPI.Services.Interfaces.AdminServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitHubBackendAPI.Controllers.AdminController
{

    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class AdminOwnersController : ControllerBase
    {
        private readonly IAdminOwnerService _adminOwnerService;

        public AdminOwnersController(IAdminOwnerService adminOwnerService)
        {
            _adminOwnerService = adminOwnerService;
        }

        // Pending Owners
        [HttpGet("pending")]
        public async Task<IActionResult> GetPendingOwners()
        {
            var owners = await _adminOwnerService.GetPendingOwnersAsync();
            return Ok(owners);
        }

        // Approve Owner
        [HttpPut("{id}/approve")]
        public async Task<IActionResult> ApproveOwner(int id)
        {
            await _adminOwnerService.ApproveOwnerAsync(id);
            return NoContent();
        }

        // Reject Owner
        [HttpPut("{id}/reject")]
        public async Task<IActionResult> RejectOwner(int id)
        {
            await _adminOwnerService.RejectOwnerAsync(id);
            return NoContent();
        }
    }
}
