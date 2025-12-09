using FitHubBackendAPI.DTOs.UserDTOs;
using FitHubBackendAPI.Services.Interfaces.UserServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FitHubBackendAPI.Controllers.UserController
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService; 
        }

        // مساعد داخلي عشان نجيب UserId من التوكن
        private int GetCurrentUserId()
        {
            var idString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.Parse(idString);
        }

        
        [HttpGet("me")]
        public async Task<ActionResult<UserProfileDto>> GetMyProfile()
        {
            var userId = GetCurrentUserId();
            var profile = await _userService.GetCurrentUserProfileAsync(userId);
            return Ok(profile);
        }

       
        [HttpPut("me")]
        public async Task<IActionResult> UpdateMyProfile([FromBody] UpdateUserProfileDto dto)
        {
            var userId = GetCurrentUserId();
            await _userService.UpdateCurrentUserProfileAsync(userId, dto);
            return NoContent(); // 204
        }

        
        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            var userId = GetCurrentUserId();
            await _userService.ChangePasswordAsync(userId, dto);
            return NoContent();
        }

    }
}
