using FitHubBackendAPI.Data;
using FitHubBackendAPI.DTOs.AdminDtos;
using FitHubBackendAPI.DTOs.UserDTOs;
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

        // ============================
        public async Task<GetUserDataDto> GetUserByIdAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                throw new KeyNotFoundException("User not found");
            var subscriptions = await _context.Subscriptions
                .Where(s => s.UserId == userId)
                .ToListAsync();
            var Reviews = await _context.Reviews
                .Where(r => r.UserId == userId)
                .ToListAsync();

            return new GetUserDataDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Phone = user.Phone,
                City = user.City,
                Role = user.Role,
                Status = user.Status,
                CreatedAt = user.CreatedAt,
                Subscriptions = subscriptions.Select(s => new GetUserSubscriptionDataDto
                {
                    Id = s.Id,
                    PlanId = s.PlanId,
                    BranchId = s.BranchId,
                    StartDate = s.StartDate,
                    EndDate = s.EndDate,
                    VisitsAllowed = s.VisitsAllowed,
                    VisitsUsed = s.VisitsUsed,
                    Status = s.Status
                }).ToList(),
                Reviews = Reviews.Select(r => new GetUserReviewDto
                {
                    Id = r.Id,
                    BranchId = r.BranchId,
                    BookingId = r.BookingId,
                    Rating = r.Rating,
                    Comment = r.Comment,
                    CreatedAt = r.CreatedAt
                }).ToList()
            };
        }
        public async Task<List<AdminUserListItemDto>> GetAllUsersAsync()
        {
            var users = await _context.Users
                .Where(x => x.Role != UserRole.Admin)
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
        //Get all fihub user plan 
        public async Task<List<AdminFitHubPlanDto>> GetUserFitHubPlansAsync(int userId)
        {
            var subscriptions = await _context.FithubUserPlans
                .Where(s => s.UserId == userId)
                .ToListAsync();

            return subscriptions.Select(s => new AdminFitHubPlanDto
            {
                Id = s.Id,
                UserId = s.UserId,
                PlanId = s.PlanId,
                BasePrice = s.BasePrice,
                TaxAmount = s.TaxAmount,
                TotalAmount = s.TotalAmount,
                PurchaseDate = s.PurchaseDate

            }).ToList();
        }
        // get all fihub user plans 
        public async Task<List<AdminFitHubPlanDto>> GetAllFitHubUserPlansAsync()
        {
            var subscriptions = await _context.FithubUserPlans.ToListAsync();

            return subscriptions.Select(s => new AdminFitHubPlanDto
            {
                Id = s.Id,
                UserId = s.UserId,
                PlanId = s.PlanId,
                BasePrice = s.BasePrice,
                TaxAmount = s.TaxAmount,
                TotalAmount = s.TotalAmount,
                PurchaseDate = s.PurchaseDate
            }).ToList();
        }

    }
}
