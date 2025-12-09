
using FitHubBackendAPI.Entities.Enums;
using FitHubBackendAPI.Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace FitHubBackendAPI.Data.DataSeeder
{
    public static class DataSeeder
    {
        public static async Task SeedAdminAsync(FitHubDbContext context)
        {
            if (await context.Users.AnyAsync(u => u.Role == UserRole.Admin))
                return;

            var admin = new User
            {
                FullName = "Admin",
                Email = "admin@fithub.com",
                Phone = "1122334455",
                City = "Cairo",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                Role = UserRole.Admin,
                Status = AccountStatus.Active,
                CreatedAt = DateTime.UtcNow
            };

            context.Users.Add(admin);
            await context.SaveChangesAsync();
        }
    }
}
