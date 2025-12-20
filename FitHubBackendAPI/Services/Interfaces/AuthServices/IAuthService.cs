using FitHubBackendAPI.DTOs.AuthDTOs;
using FitHubBackendAPI.Entities;
using FitHubBackendAPI.Entities.Models;

namespace FitHubBackendAPI.Services.Interfaces.AuthServices
{
    public interface IAuthService
    {
        Task<User> RegisterUserAsync(RegisterUserDto dto);
        Task<GymOwner> RegisterOwnerAsync(RegisterOwnerDto dto);

        Task<string> LoginAsync(LoginDto dto);

        Task ReSendOtpAsync(string email);
        Task VerifyOtpAsync(VerifyOtpDto dto);

        Task ForgotPasswordAsync(ForgotPasswordDto dto);
        Task ResetPasswordAsync(ResetPasswordDto dto);
        Task RegisterStaffAsync(RegisterStaffDTO dto, int ownerId);

    }
}
