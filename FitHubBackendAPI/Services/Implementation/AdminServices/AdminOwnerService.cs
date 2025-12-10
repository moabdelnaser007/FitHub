using FitHubBackendAPI.Data;
using FitHubBackendAPI.Entities;
using FitHubBackendAPI.Entities.Enums;
using FitHubBackendAPI.Services.Interfaces.AdminServices;
using FitHubBackendAPI.Services.Interfaces.AuthServices;
using Microsoft.EntityFrameworkCore;

namespace FitHubBackendAPI.Services.Implementation.AdminServices
{
    public class AdminOwnerService : IAdminOwnerService
    {
        private readonly FitHubDbContext _context;
        private readonly IEmailService _emailService;


        public AdminOwnerService(FitHubDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        // Pending Owners
        public async Task<List<GymOwner>> GetPendingOwnersAsync()
        {
            return await _context.GymOwners
                .Where(x => x.ApplicationStatus == ApplicationStatus.PENDING && x.Status == AccountStatus.Active)
                .ToListAsync();
        }

        // Approve
        public async Task ApproveOwnerAsync(int ownerId)
        {
            var owner = await _context.GymOwners.FindAsync(ownerId);

            if (owner == null)
                throw new Exception("Owner not found");

            owner.Status = AccountStatus.Active;
            owner.ApplicationStatus = ApplicationStatus.APPROVED;

            await _context.SaveChangesAsync();

            await _emailService.SendAsync(owner.Email,
            "Gym Approved",
            "Your gym has been approved successfully");
        }

        // Reject
        public async Task RejectOwnerAsync(int ownerId)
        {
            var owner = await _context.GymOwners.FindAsync(ownerId);

            if (owner == null)
                throw new Exception("Owner not found");

            owner.Status = AccountStatus.Rejected;
            owner.ApplicationStatus = ApplicationStatus.REJECTED;

            await _context.SaveChangesAsync();

            await _emailService.SendAsync(owner.Email,
            "Gym Rejected",
            "Your gym registration was rejected");
        }
    }
}
