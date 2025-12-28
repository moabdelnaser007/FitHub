using FitHubBackendAPI.DTOs.AdminDtos;
using Microsoft.EntityFrameworkCore;

namespace FitHubBackendAPI.Services.Interfaces.AdminServices
{
    public interface IAdminUserService
    {
        Task<GetUserDataDto> GetUserByIdAsync(int userId);
        Task<List<AdminUserListItemDto>> GetAllUsersAsync();
        Task UpdateUserAsync(int userId, AdminUpdateUserDto dto);
        Task DeleteUserAsync(int userId);
        Task<List<AdminFitHubPlanDto>> GetUserFitHubPlansAsync(int userId);
        Task<List<AdminFitHubPlanDto>> GetAllFitHubUserPlansAsync();

        
    }
}
