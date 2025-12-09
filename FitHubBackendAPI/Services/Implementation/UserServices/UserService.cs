using FitHubBackendAPI.Data;
using FitHubBackendAPI.DTOs.UserDTOs;
using FitHubBackendAPI.Entities.Enums;
using FitHubBackendAPI.Services.Interfaces.UserServices;
using Microsoft.EntityFrameworkCore;

namespace FitHubBackendAPI.Services.Implementation.UserServices
{
    public class UserService : IUserService
    {
        private readonly FitHubDbContext _context;

        public UserService(FitHubDbContext context)
        {
            _context = context; 
        }

        
        public async Task<UserProfileDto> GetCurrentUserProfileAsync(int userId)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.Id == userId);

            if (user == null)
                throw new Exception("User not found");

            return new UserProfileDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Phone = user.Phone,
                City = user.City,
                Role = user.Role.ToString(),
                Status = user.Status.ToString()
            };
        }

        
        public async Task UpdateCurrentUserProfileAsync(int userId, UpdateUserProfileDto dto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.Id == userId);

            if (user == null)
                throw new Exception("User not found");

            user.FullName = dto.FullName ?? user.FullName;
            user.Phone = dto.Phone ?? user.Phone;
            user.City = dto.City ?? user.City;

            await _context.SaveChangesAsync();
        }

       
        public async Task ChangePasswordAsync(int userId, ChangePasswordDto dto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.Id == userId);

            if (user == null)
                throw new Exception("User not found");

            var isValidCurrent = BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, user.PasswordHash);
            if (!isValidCurrent)
                throw new Exception("Current password is incorrect");

            if (dto.NewPassword != dto.ConfirmPassword)
                throw new Exception("New password and confirmation do not match");

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);

            await _context.SaveChangesAsync();
        }

        
        public async Task<List<AdminUserListItemDto>> GetAllUsersAsync()
        {
            var users = await _context.Users
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

            return users.Select(u => new AdminUserListItemDto
            {
                Id = u.Id,
                FullName = u.FullName,
                Email = u.Email,
                Phone = u.Phone,
                City = u.City,
                Role = u.Role.ToString(),
                Status = u.Status.ToString(),
                CreatedAt = u.CreatedAt
            }).ToList();
        }

        public async Task UpdateUserStatusAsync(int userId, AccountStatus status)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.Id == userId);

            if (user == null)
                throw new Exception("User not found");

            user.Status = status;

            await _context.SaveChangesAsync();
        }

    }
}
