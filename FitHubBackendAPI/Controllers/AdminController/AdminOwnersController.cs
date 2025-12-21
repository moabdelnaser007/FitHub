using FitHubBackendAPI.DTOs.AdminDtos;
using FitHubBackendAPI.Entities.Enums;
using FitHubBackendAPI.Services.Interfaces.AdminServices;
using FitHubBackendAPI.ViewModels;
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
            return Ok(ResponseViewModel<List<PendingOwnerDto>>.Success(owners,"Pending owners retrieved successfully"));
        }

        // Approve Owner
        [HttpPut("{id}/approve")]
        public async Task<IActionResult> ApproveOwner(int id)
        {
            await _adminOwnerService.ApproveOwnerAsync(id);
            return Ok(ResponseViewModel<string>.Success(null,"Owner approved successfully"));
        }

        // Reject Owner
        [HttpPut("{id}/reject")]
        public async Task<IActionResult> RejectOwner(int id)
        {
            await _adminOwnerService.RejectOwnerAsync(id);
            return Ok(ResponseViewModel<string>.Success(null,"Owner rejected successfully"));
        }
        [HttpGet("GetAllOwners")]
        public async Task<ResponseViewModel<IEnumerable<GetOwnerForAdminDto>>> GetAllOwners()
        {
            var owners = await _adminOwnerService.GetAllGymOwners();
            if (owners == null || !owners.Any())
            {
                return ResponseViewModel<IEnumerable<GetOwnerForAdminDto>>.Fail("No owners found", ErrorCode.NotFound);
            }
            return ResponseViewModel<IEnumerable<GetOwnerForAdminDto>>.Success(owners, "Owners retrieved successfully");
        }

    }
}
