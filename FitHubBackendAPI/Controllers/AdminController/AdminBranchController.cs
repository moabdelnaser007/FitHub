using FitHubBackendAPI.DTOs.AdminDtos;
using FitHubBackendAPI.DTOs.GymBranchDTOs;
using FitHubBackendAPI.Entities.Enums;
using FitHubBackendAPI.Services.Interfaces.AdminServices;
using FitHubBackendAPI.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FitHubBackendAPI.Controllers.AdminController
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminBranchController : ControllerBase
    {
        private readonly IAdminGymService _adminGymService;
        public AdminBranchController(IAdminGymService adminGymService)
        {
            _adminGymService = adminGymService;
        }
        [HttpGet("GetAllBranches")]
        public async Task<ResponseViewModel<IEnumerable<GetBranchForAdminDto>>> GetAllBranches()
        {
            var branches = await _adminGymService.GetAllGymBranchesAsync();
            if (branches == null || !branches.Any())
            {
                return ResponseViewModel<IEnumerable<GetBranchForAdminDto>>.Fail("No branches found", ErrorCode.NotFound);
            }
            return ResponseViewModel<IEnumerable<GetBranchForAdminDto>>.Success(branches, "Branches retrieved successfully");
        }
        [HttpGet("GetBranchesByOwner/{ownerId}")]
        public async Task<ResponseViewModel<IEnumerable<GetBranchForAdminDto>>> GetBranchesByOwner(int ownerId)
        {
            if (ownerId <= 0)
            {
                return ResponseViewModel<IEnumerable<GetBranchForAdminDto>>.Fail("Invalid owner ID", ErrorCode.BadRequest);
            }
            var branches = await _adminGymService.GetAllBranchOfOwner(ownerId);
            if (branches == null || !branches.Any())
            {
                return ResponseViewModel<IEnumerable<GetBranchForAdminDto>>.Fail("No branches found for the specified owner", ErrorCode.NotFound);
            }
            return ResponseViewModel<IEnumerable<GetBranchForAdminDto>>.Success(branches, "Branches retrieved successfully");
        }
        [HttpGet("GetBranchById/{branchId}")]
        public async Task<ResponseViewModel<GetBranchForAdminDto>> GetBranchById(int branchId)
        {
            if (branchId <= 0)
            {
                return ResponseViewModel<GetBranchForAdminDto>.Fail("Invalid branch ID", ErrorCode.BadRequest);
            }
            var branch = await _adminGymService.GetGymBranchByIdAsync(branchId);
            if (branch == null)
            {
                return ResponseViewModel<GetBranchForAdminDto>.Fail("Branch not found", ErrorCode.NotFound);
            }
            return ResponseViewModel<GetBranchForAdminDto>.Success(branch, "Branch retrieved successfully");
        }
        [HttpPost("SuspendBranch/{branchId}")]
        public async Task<ResponseViewModel<bool>> SuspendBranch(int branchId)
        {
            if (branchId <= 0)
            {
                return ResponseViewModel<bool>.Fail("Invalid branch ID", ErrorCode.BadRequest);
            }
            var result = await _adminGymService.SuspendGym(branchId);
            if (!result)
            {
                return ResponseViewModel<bool>.Fail("Failed to suspend branch", ErrorCode.InternalServerError);
            }
            return ResponseViewModel<bool>.Success(result, "Branch suspended successfully");
        }
        [HttpPost("ResumeBranch/{branchId}")]
        public async Task<ResponseViewModel<bool>> ResumeBranch(int branchId)
        {
            if (branchId <= 0)
            {
                return ResponseViewModel<bool>.Fail("Invalid branch ID", ErrorCode.BadRequest);
            }
            var result = await _adminGymService.ResumeGym(branchId);
            if (!result)
            {
                return ResponseViewModel<bool>.Fail("Failed to resume branch", ErrorCode.InternalServerError);
            }
            return ResponseViewModel<bool>.Success(result, "Branch resumed successfully");
        }
        [HttpPut("UpdateBranch/{branchId}")]
        public async Task<ResponseViewModel<UpdateGymBranchDTO>> UpdateBranch(int branchId, [FromBody] UpdateGymBranchDTO dto)
        {
            if (branchId <= 0)
            {
                return ResponseViewModel<UpdateGymBranchDTO>.Fail("Invalid branch ID", ErrorCode.BadRequest);
            }
            if (dto == null)
            {
                return ResponseViewModel<UpdateGymBranchDTO>.Fail("Invalid branch data", ErrorCode.BadRequest);
            }
            var updatedBranch = await _adminGymService.UpdateGymBranchAsync(dto, branchId);
            if (updatedBranch == null)
            {
                return ResponseViewModel<UpdateGymBranchDTO>.Fail("Failed to update branch", ErrorCode.InternalServerError);
            }
            return ResponseViewModel<UpdateGymBranchDTO>.Success(updatedBranch, "Branch updated successfully");
        }
        [HttpGet("GetSuspendedBranches")]
        public async Task<ResponseViewModel<IEnumerable<GetBranchForAdminDto>>> GetSuspendedBranches()
        {
            var branches = await _adminGymService.GetSuspendedBranches();
            if (branches == null || !branches.Any())
            {
                return ResponseViewModel<IEnumerable<GetBranchForAdminDto>>.Fail("No suspended branches found", ErrorCode.NotFound);
            }
            return ResponseViewModel<IEnumerable<GetBranchForAdminDto>>.Success(branches, "Suspended branches retrieved successfully");
        }
    }
}
