using FitHubBackendAPI.Data;
using FitHubBackendAPI.DTOs.AdminDtos;
using FitHubBackendAPI.Entities.Enums;
using FitHubBackendAPI.Services.Interfaces.AdminServices;
using Microsoft.EntityFrameworkCore;

namespace FitHubBackendAPI.Services.Implementation.AdminServices
{
    public class AdminUserService : IAdminUserService
    {
        private readonly FitHubDbContext _context;

        public AdminUserService(FitHubDbContext context)
        {
            _context = context;
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

        // ============================
        // UPDATE USER
        // ============================
        public async Task UpdateUserAsync(int userId, AdminUpdateUserDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);

            if (user == null)
                throw new KeyNotFoundException("User not found");

            user.FullName = dto.FullName ?? user.FullName;
            user.Email = dto.Email;
            user.Phone = dto.Phone ?? user.Phone;
            user.City = dto.City ?? user.City;

            if (dto.Role.HasValue)
                user.Role = dto.Role.Value;

            if (dto.Status.HasValue)
                user.Status = dto.Status.Value;

            await _context.SaveChangesAsync();
        }

        // ============================
        // DELETE (SUSPEND) USER
        // ============================
        public async Task DeleteUserAsync(int userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);

            if (user == null)
                throw new KeyNotFoundException("User not found");

            // Soft Delete
            user.Status = AccountStatus.Suspended;

            await _context.SaveChangesAsync();
        }
    }
}
