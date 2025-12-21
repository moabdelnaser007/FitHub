using AutoMapper;
using AutoMapper.QueryableExtensions;
using FitHubBackendAPI.Data;
using FitHubBackendAPI.DTOs.AdminDtos;
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
        private readonly IMapper _mapper;


        public AdminOwnerService(FitHubDbContext context, IEmailService emailService, IMapper mapper)
        {
            _context = context;
            _emailService = emailService;
            _mapper = mapper;
        }

        // Pending Owners
        public async Task<List<PendingOwnerDto>> GetPendingOwnersAsync()
        {
            return await _context.GymOwners.Include(o => o.User)
                .Where(o => o.ApplicationStatus == ApplicationStatus.PENDING)
                .OrderByDescending(o => o.CreatedAt)
                .Select(o => new PendingOwnerDto
                {
                    Id = o.Id,
                    FullName = o.User.FullName,
                    Email = o.User.Email,
                    Phone = o.User.Phone,
                    City = o.User.City,
                    CommercialRegistrationNumber = o.CommercialRegistrationNumber,
                    CreatedAt = o.CreatedAt
                })
                .ToListAsync();
        }

        // Approve
        public async Task ApproveOwnerAsync(int ownerId)
        {
            var owner = await _context.GymOwners.Include(o => o.User).FirstOrDefaultAsync(o => o.Id == ownerId);

            if (owner == null)
                throw new Exception("Owner not found");

            owner.User.Status = AccountStatus.Active;
            owner.ApplicationStatus = ApplicationStatus.APPROVED;

            await _context.SaveChangesAsync();

            await _emailService.SendAsync(owner.User.Email,
            "Gym Approved",
            "Your gym has been approved successfully");
        }

        // Reject
        public async Task RejectOwnerAsync(int ownerId)
        {
            var owner = await _context.GymOwners.Include(o => o.User).FirstOrDefaultAsync(o => o.Id == ownerId);

            if (owner == null)
                throw new Exception("Owner not found");

            owner.User.Status = AccountStatus.Rejected;
            owner.ApplicationStatus = ApplicationStatus.REJECTED;

            await _context.SaveChangesAsync();

            await _emailService.SendAsync(owner.User.Email,
            "Gym Rejected",
            "Your gym registration was rejected");
        }
        public async Task<IEnumerable<GetOwnerForAdminDto>> GetAllGymOwners()
        {
            return await _context.GymOwners
            .AsNoTracking()
            .Include(o => o.User) 
            .ProjectTo<GetOwnerForAdminDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
        }


    }
}
