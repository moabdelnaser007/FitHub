using FitHubBackendAPI.DTOs.UserDTOs;
using FitHubBackendAPI.Entities.Enums;

namespace FitHubBackendAPI.Services.Interfaces.UserServices
{
    public interface IUserService
    {
        Task<UserProfileDto> GetCurrentUserProfileAsync(int userId);

        Task UpdateCurrentUserProfileAsync(int userId, UpdateUserProfileDto dto);

        Task ChangePasswordAsync(int userId, ChangePasswordDto dto);


    }
}
