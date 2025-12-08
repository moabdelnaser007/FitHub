using FitHubBackendAPI.DTOs.AuthDTOs;
using FitHubBackendAPI.Services.Interfaces.AuthServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<IActionResult> RegisterUser(RegisterUserDto dto)
        {
            await _authService.RegisterUserAsync(dto);
            return Ok("User registered. OTP sent.");
        }

        [HttpPost("register-owner")]
        public async Task<IActionResult> RegisterOwner([FromForm] RegisterOwnerDto dto)
        {
            await _authService.RegisterOwnerAsync(dto);
            return Ok("Owner registered. Await admin approval.");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var token = await _authService.LoginAsync(dto);
            return Ok(new { token });
        }

        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp(VerifyOtpDto dto)
        {
            await _authService.VerifyOtpAsync(dto);
            return Ok("Account activated");
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDto dto)
        {
            await _authService.ForgotPasswordAsync(dto);
            return Ok("OTP sent");
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto dto)
        {
            await _authService.ResetPasswordAsync(dto);
            return Ok("Password reset done");



        }
    }
}
