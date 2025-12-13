using FitHubBackendAPI.DTOs.UserDTOs;
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
        private readonly IAdminOwnerService _adminOwnerService;

        public AdminUsersController(IAdminOwnerService adminOwnerService)
        {
            _adminOwnerService = adminOwnerService;

        }


        [HttpGet("GetAllUsers")]
        public async Task<ActionResult<List<AdminUserListItemDto>>> GetUsers()
        {
            var users = await _adminOwnerService.GetAllUsersAsync();
            var response = ResponseViewModel<List<AdminUserListItemDto>>.Success(
                    users,
                    "Users retrieved successfully"
                );

            response.ErrorCode = ErrorCode.OK;

            return Ok(response);
        }


    }
}
