using FitHubBackendAPI.DTOs.GymBranchDTOs;
using FitHubBackendAPI.Services.Interfaces.OwnerServices;
using FitHubBackendAPI.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FitHubBackendAPI.Controllers.User_Controller
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserBranchController : ControllerBase
    {
        
        IGymBranchService _gymBranchService;
        public UserBranchController( IGymBranchService gymBranchService)
        {
            
            _gymBranchService = gymBranchService;
        }
        [HttpGet]
        [Route("GetAllActiveBranches")]
        public async Task<ResponseViewModel<IEnumerable<GetAllBranchDTO>>> GetAllActiveBranches()
        {
            var branches = await _gymBranchService.GetAllActiveBranchesAsync();
            if (branches == null || !branches.Any())
            {
                return ResponseViewModel<IEnumerable<GetAllBranchDTO>>.Fail("No active branches found.",Entities.Enums.ErrorCode.NoContent);
            }
            return ResponseViewModel<IEnumerable<GetAllBranchDTO>>.Success(branches, "Active branches retrieved successfully.");
        }
        [HttpGet]
        [Route("GetActiveBranchById/{branchId}")]
        public async Task<ResponseViewModel<GetGymBranchByIdDTO>> GetActiveBranchById(int branchId)
        {
            var branch = await _gymBranchService.GetActiveGymBranchByIdAsync(branchId);
            if (branch == null)
            {
                return ResponseViewModel<GetGymBranchByIdDTO>.Fail("Branch not found or inactive.", Entities.Enums.ErrorCode.NotFound);
            }
            return ResponseViewModel<GetGymBranchByIdDTO>.Success(branch, "Active branch retrieved successfully.");
        }
    }
}
