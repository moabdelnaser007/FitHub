using FitHubBackendAPI.DTOs.PlanDTOs;
using FitHubBackendAPI.Services.Interfaces.GymBranch;
using FitHubBackendAPI.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FitHubBackendAPI.Controllers.GymControllers
{
    [Route("api/owner/[controller]")]
    [ApiController]
    [Authorize(Roles = "Owner")]
    public class PlansController : ControllerBase
    {
        private readonly IPlanService _planService;

        public PlansController(IPlanService planService) 
        { 
            _planService = planService;
        }


        [HttpPost]
        [Route("{branchId}/Create")]
        public async Task<ResponseViewModel<GetPlanByIdDTO>> CreatePlan(int branchId, [FromBody] CreatePlanDTO createPlanDto)
        {

            if (createPlanDto == null)
            {
                return ResponseViewModel<GetPlanByIdDTO>.Fail("Invalid plan data.");
            }
            var result = await _planService.CreatePlanAsync(branchId, createPlanDto);
            return ResponseViewModel<GetPlanByIdDTO>.Success(result, "Plan created successfully.");
        }


        [HttpDelete]
        [Route("Delete/{planId}")]
        public async Task<ResponseViewModel<bool>> DeletePlan(int planId)
        {
            var result = await _planService.DeletePlanAsync(planId);
            if (result)
                return ResponseViewModel<bool>.Success(true, "Plan deleted successfully.");
            else
                return ResponseViewModel<bool>.Fail("Plan deletion failed.");
        }


        [HttpGet]
        [Route("ByBranch/{branchId}")]
        [AllowAnonymous]
        public async Task<ResponseViewModel<IEnumerable<GetPlanByBranchIdDTO>>> GetPlansByBranchId(int branchId)
        {
            var result = await _planService.GetPlansByBranchIdAsync(branchId);
            if (result == null || !result.Any())
                return ResponseViewModel<IEnumerable<GetPlanByBranchIdDTO>>.Fail("No plans found for the specified branch.");
            return ResponseViewModel<IEnumerable<GetPlanByBranchIdDTO>>.Success(result, "Plans retrieved successfully.");
        }

        [Authorize]
        [HttpGet]
        [Route("{planId}")]
        public async Task<ResponseViewModel<GetPlanByIdDTO>> GetPlanById(int planId)
        {
            var result = await _planService.GetPlanByIdAsync(planId);
            if (result == null)
                return ResponseViewModel<GetPlanByIdDTO>.Fail("Plan not found.");
            return ResponseViewModel<GetPlanByIdDTO>.Success(result, "Plan retrieved successfully.");
        }


        [HttpPut]
        [Route("Update")]
        public async Task<ResponseViewModel<GetPlanByIdDTO>> UpdatePlan(UpdatePlanDTO updatePlanDto)
        {
            var result = await _planService.UpdatePlanAsync(updatePlanDto);
            return ResponseViewModel<GetPlanByIdDTO>.Success(result, "Plan updated successfully.");
        }
        [HttpPut]
        [Route("Activate/{planId}")]
        public async Task<ResponseViewModel<bool>> ActivatePlan(int planId)
        {
            await _planService.ActivatePlanAsync(planId);
            return ResponseViewModel<bool>.Success(true, "Plan activated successfully.");
        }
        [HttpPut]
        [Route("Deactivate/{planId}")]
        public async Task<ResponseViewModel<bool>> DeactivatePlan(int planId)
        {
            await _planService.DeactivatePlanAsync(planId);
            return ResponseViewModel<bool>.Success(true, "Plan deactivated successfully.");
        }

    }
}
