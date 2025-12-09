using FitHubBackendAPI.DTOs.UserDTOs;
using FitHubBackendAPI.Entities.Enums;
using FitHubBackendAPI.Services.Interfaces.UserServices;
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
        private readonly IUserService _userService;

        public AdminUsersController(IUserService userService)
        {
            _userService = userService;
        }


        [HttpGet]
        public async Task<ActionResult<List<AdminUserListItemDto>>> GetUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }


        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateUserStatus(int id, [FromQuery] AccountStatus status)
        {
            await _userService.UpdateUserStatusAsync(id, status);
            return NoContent();
        }
    }
}
