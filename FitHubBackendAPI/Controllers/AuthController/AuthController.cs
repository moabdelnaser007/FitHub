using FitHubBackendAPI.DTOs.AuthDTOs;
using FitHubBackendAPI.Entities.Enums;
using FitHubBackendAPI.Services.Interfaces.AuthServices;
using FitHubBackendAPI.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FitHubBackendAPI.Controllers.AuthController
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register-user")]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterUserDto dto)
        {
            var user = await _authService.RegisterUserAsync(dto);

            var userResponse = new UserResponseDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Phone = user.Phone,
                City = user.City,
                Role = user.Role.ToString(),
                Status = user.Status.ToString()
            };

            var response = ResponseViewModel<UserResponseDto>.Success(userResponse, "User registered successfully. Please login.");
            response.ErrorCode = ErrorCode.Created;
            return StatusCode((int)ErrorCode.Created, response);
        }

        [HttpPost("register-owner")]

        public async Task<IActionResult> RegisterOwner([FromForm] RegisterOwnerDto dto)
        {
            var owner = await _authService.RegisterOwnerAsync(dto);

            var ownerResponse = new OwnerResponseDto
            {
                Id = owner.Id,
                FullName = owner.User.FullName,
                Email = owner.User.Email,
                Phone = owner.User.Phone,
                CommercialRegistrationNumber = owner.CommercialRegistrationNumber,
                Status = owner.User.Status.ToString()
            };

            var response = ResponseViewModel<OwnerResponseDto>.Success(ownerResponse, "Owner registered. Await admin approval.");
            response.ErrorCode = ErrorCode.Created;
            return StatusCode((int)ErrorCode.Created, response);
        }
        [HttpPost]
        [Authorize(Roles = "Owner")]
        [Route("register-staff")]
        public async Task<IActionResult> RegisterStaffMember([FromForm] RegisterStaffDTO dto)
        {
            int ownerId = int.Parse(
    User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0"
);
            if (ownerId == 0)
            {
                return Unauthorized("Invalid owner ID.");
            }
            await _authService.RegisterStaffAsync(dto,ownerId);
            return Ok("Staff member registered.");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var token = await _authService.LoginAsync(dto);

            var tokenDto = new TokenResponseDto { Token = token };

            var response = ResponseViewModel<TokenResponseDto>.Success(tokenDto, "Login successful.");
            response.ErrorCode = ErrorCode.OK;
            return Ok(response);
        }

        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpDto dto)
        {
            await _authService.VerifyOtpAsync(dto);

            var response = ResponseViewModel<string>.Success(null, "OTP Checked successfully.");
            response.ErrorCode = ErrorCode.OK;
            return Ok(response);
        }

        [HttpPost("Resend-otp")]
        public async Task<IActionResult> ReSendOtp([FromBody] ReSendOtpDto dto)
        {
            await _authService.ReSendOtpAsync(dto.Email);

            var response = ResponseViewModel<string>.Success(null, "OTP sent successfully.");
            response.ErrorCode = ErrorCode.OK;
            return Ok(response);
        }


        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
        {
            await _authService.ForgotPasswordAsync(dto);

            var response = ResponseViewModel<string>.Success(null, "OTP sent for password reset.");
            response.ErrorCode = ErrorCode.OK;
            return Ok(response);
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            await _authService.ResetPasswordAsync(dto);

            var response = ResponseViewModel<string>.Success(null, "Password has been reset successfully.");
            response.ErrorCode = ErrorCode.OK;
            return Ok(response);
        }
    }
}
