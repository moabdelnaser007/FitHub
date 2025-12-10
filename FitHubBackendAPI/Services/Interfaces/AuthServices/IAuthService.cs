using FitHubBackendAPI.DTOs.AuthDTOs;

namespace FitHubBackendAPI.Services.Interfaces.AuthServices
{
    public interface IAuthService
    {
        Task RegisterUserAsync(RegisterUserDto dto);
        Task RegisterOwnerAsync(RegisterOwnerDto dto);

        Task<string> LoginAsync(LoginDto dto);

        Task SendOtpAsync(SendOtpDto dto);
        Task VerifyOtpAsync(VerifyOtpDto dto);

        Task ForgotPasswordAsync(ForgotPasswordDto dto);
        Task ResetPasswordAsync(ResetPasswordDto dto);

    }
}
