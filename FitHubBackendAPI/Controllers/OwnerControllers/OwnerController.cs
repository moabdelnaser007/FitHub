using FitHubBackendAPI.Services.Interfaces.GymBranch;
using FitHubBackendAPI.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FitHubBackendAPI.Controllers.OwnerControllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Owner")]
    public class OwnerController : ControllerBase
    {

        private readonly IOwnerToStaffService _ownerToStaffService;
        public OwnerController(IOwnerToStaffService ownerToStaffService)
        {
            _ownerToStaffService = ownerToStaffService;
        }

        [HttpDelete]
        [Route("DeleteStaffMember/{staffId}")]
        public async Task<ResponseViewModel<bool>> DeleteStaffMember(int staffId)
        {
            if (staffId <= 0)
            {
                return ResponseViewModel<bool>.Fail("Invalid staff member ID.");
            }
            var res = await _ownerToStaffService.DeleteStaffMemberAsync(staffId);

            return ResponseViewModel<bool>.Success(res, "Staff member deleted successfully");
        }
        [HttpGet]
        [Route("AssignStaffToBranch/{staffId}/{branchId}")]
        public async Task<ResponseViewModel<bool>> AssignStaffToBranch(int staffId, int branchId)
        {
            if (staffId <= 0 || branchId <= 0)
            {
                return ResponseViewModel<bool>.Fail("Invalid staff or branch ID.");
            }
            var res = await _ownerToStaffService.AssignStaffToBranch(staffId, branchId);

            return ResponseViewModel<bool>.Success(res, "Staff member assigned to branch successfully");
        }
    }
}
