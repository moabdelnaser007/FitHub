using FitHubBackendAPI.DTOs.AdminDtos;
using FitHubBackendAPI.Entities.Enums;
using FitHubBackendAPI.Services.Interfaces.AdminServices;
using FitHubBackendAPI.Services.Interfaces.UserServices;
using FitHubBackendAPI.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FitHubBackendAPI.Controllers.UserController
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminUsersController : ControllerBase
    {
        private readonly IAdminUserService _adminUserService;

        public AdminUsersController(IAdminUserService adminUserService)
        {
            _adminUserService = adminUserService;

        }

        [HttpGet("GetUser/{id}")]
        public async Task<ResponseViewModel<GetUserDataDto>> GetUser(int id)
        {
            var user = await _adminUserService.GetUserByIdAsync(id);
            if(user == null)
            {
                return ResponseViewModel<GetUserDataDto>.Fail("User not found", ErrorCode.NotFound);
            }
            return ResponseViewModel<GetUserDataDto>.Success(user,"User retrieved successfully");
            
        }

        [HttpGet("GetAllUsers")]
        public async Task<ActionResult<List<AdminUserListItemDto>>> GetUsers()
        {
            var users = await _adminUserService.GetAllUsersAsync();
            var response = ResponseViewModel<List<AdminUserListItemDto>>.Success(users,"Users retrieved successfully");

            response.ErrorCode = ErrorCode.OK;

            return Ok(response);
        }


        [HttpPut("{id}/UpdateUser")]
        public async Task<IActionResult> UpdateUser(int id,[FromBody] AdminUpdateUserDto dto)
        {
            await _adminUserService.UpdateUserAsync(id, dto);

            return Ok(ResponseViewModel<string>.Success(null,"User updated successfully"));
        }


        [HttpDelete("{id}/DeleteUser")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            await _adminUserService.DeleteUserAsync(id);

            return Ok(ResponseViewModel<string>.Success( null,"User suspended successfully" ));
        }
        [Authorize(Roles ="Admin,User")]
        [HttpGet("{id}/GetCreditsPlansHistory")]
        public async Task<ResponseViewModel<List<AdminFitHubPlanDto>>> GetCreditsPlansHistory(int id)
        {
            var plans = await _adminUserService.GetUserFitHubPlansAsync(id);
            if (!plans.Any())
            {
                return ResponseViewModel<List<AdminFitHubPlanDto>>.Fail("no History");
            }
            return ResponseViewModel<List<AdminFitHubPlanDto>>.Success(plans);
        }
        [HttpGet("GetAllUsersCreditHistory")]
        public async Task<ResponseViewModel<List<AdminFitHubPlanDto>>> GetAllUsersCreditHistory()
        {
            var plans = await _adminUserService.GetAllFitHubUserPlansAsync();
            if (!plans.Any())
            {
                return ResponseViewModel<List<AdminFitHubPlanDto>>.Fail("no History");
            }
            return ResponseViewModel<List<AdminFitHubPlanDto>>.Success(plans);
        }
    }
}
