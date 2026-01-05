using FitHubBackendAPI.Data;
using FitHubBackendAPI.DTOs.AdminDtos;
using FitHubBackendAPI.DTOs.GymBranchDTOs;
using FitHubBackendAPI.Entities.Enums;
using FitHubBackendAPI.Entities.Models;
using FitHubBackendAPI.Repository.Implementation;
using FitHubBackendAPI.Repository.Interfaces;
using FitHubBackendAPI.Services.Interfaces.AdminServices;
using Microsoft.EntityFrameworkCore;

namespace FitHubBackendAPI.Services.Implementation.AdminServices
{
    
    public class AdminGymService : IAdminGymService
    {
        private readonly IGenericRepository<GymBranch> _branchRepository;
        private readonly FitHubDbContext _context;
        public AdminGymService(IGenericRepository<GymBranch> branchRepository, FitHubDbContext context)
        {
            _branchRepository = branchRepository;
            _context = context;

        }

        public async Task<IEnumerable<GetBranchForAdminDto>> GetAllBranchOfOwner(int ownerId)
        {
            var branches = await _branchRepository.GetAllAsync();
            var ownerBranches = branches.Where(b => b.OwnerId == ownerId);
            var ownerBranchDtos = ownerBranches.Select(b => new GetBranchForAdminDto
            {
                Id = b.Id,
                OwnerId = b.OwnerId,
                BranchName = b.BranchName,
                Phone = b.Phone,
                Address = b.Address,
                City = b.City,
                OpenTime = b.OpenTime,
                CloseTime = b.CloseTime,
                GenderType=b.GenderType,
                Status = b.Status,
                VisitCreditsCost = b.VisitCreditsCost,
                Description = b.Description,
                WorkingDays = b.WorkingDays,
                AmenitiesAvailable = b.AmenitiesAvailable,
                
                CreatedAt = b.CreatedAt,
                UpdatedAt = b.UpdatedAt
            }).ToList();
            return ownerBranchDtos;
        }

        public async Task<IEnumerable<GetBranchForAdminDto>> GetAllGymBranchesAsync()
        {
            var branche = await _context.GymBranches
                .Include(b => b.Owner)
                .ThenInclude(o => o.User)
                .ToListAsync();

            var branches = await _branchRepository.GetAllAsync();
            return branches.Select(b => new GetBranchForAdminDto
            {
                Id = b.Id,
                OwnerId = b.OwnerId,
                //OwnerName = b.Owner.FullName,
                BranchName = b.BranchName,
                Phone = b.Phone,
                Address = b.Address,
                City = b.City,
                OpenTime = b.OpenTime,
                CloseTime = b.CloseTime,
                GenderType = b.GenderType,
                Status = b.Status,
                VisitCreditsCost = b.VisitCreditsCost,
                Description = b.Description,
                WorkingDays = b.WorkingDays,
                AmenitiesAvailable = b.AmenitiesAvailable,
                
                CreatedAt = b.CreatedAt,
                UpdatedAt = b.UpdatedAt
            }).ToList();
        }

        public async Task<GetBranchForAdminDto> GetGymBranchByIdAsync(int branchId)
        {
            var branch = await _branchRepository.GetByIdAsync(branchId);
            if (branch == null) return null;

            return new GetBranchForAdminDto
            {
                Id = branch.Id,
                OwnerId = branch.OwnerId,
                BranchName = branch.BranchName,
                Phone = branch.Phone,
                Address = branch.Address,
                City = branch.City,
                OpenTime = branch.OpenTime,
                CloseTime = branch.CloseTime,
                GenderType = branch.GenderType,
                Status = branch.Status,
                VisitCreditsCost = branch.VisitCreditsCost,
                Description = branch.Description,
                WorkingDays = branch.WorkingDays,
                AmenitiesAvailable = branch.AmenitiesAvailable,
                
                CreatedAt = branch.CreatedAt,
                UpdatedAt = branch.UpdatedAt
            };
        
        }

        public async Task<IEnumerable<GetBranchForAdminDto>> GetSuspendedBranches()
        {
            var branches = await _branchRepository.GetAllAsync();
            var suspendedBranches = branches.Where(b => b.IsAcTive == false);
            return suspendedBranches.Select(b => new GetBranchForAdminDto
            {
                Id = b.Id,
                OwnerId = b.OwnerId,
                BranchName = b.BranchName,
                Phone = b.Phone,
                Address = b.Address,
                City = b.City,
                OpenTime = b.OpenTime,
                CloseTime = b.CloseTime,
                GenderType = b.GenderType,
                Status = b.Status,
                VisitCreditsCost = b.VisitCreditsCost,
                Description = b.Description,
                WorkingDays = b.WorkingDays,
                AmenitiesAvailable = b.AmenitiesAvailable,
                
                CreatedAt = b.CreatedAt,
                UpdatedAt = b.UpdatedAt
            }).ToList();
        }

        public async Task<bool> ResumeGym(int branchId)
        {
            var branch = await _branchRepository.GetByIdAsync(branchId);
            if (branch == null) return false;

            branch.IsAcTive = true;
            branch.Status = BranchStatus.ACTIVE;
            await _branchRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SuspendGym(int branchId)
        {
            var branch = await _branchRepository.GetByIdAsync(branchId);
            if (branch == null) return false;

            branch.IsAcTive = false;
            branch.Status = BranchStatus.INACTIVE;
            await _branchRepository.SaveChangesAsync();
            return true;
        }

        public async Task<UpdateGymBranchDTO> UpdateGymBranchAsync(UpdateGymBranchDTO dto, int branchId)
        {
            var branch = await _branchRepository.GetByIdAsync(branchId);
            if (branch == null) return null;

            branch.BranchName = dto.BranchName;
            branch.Phone = dto.Phone;
            branch.Address = dto.Address;
            branch.City = dto.City;
            branch.OpenTime = dto.OpenTime;
            branch.CloseTime = dto.CloseTime;
            branch.GenderType = dto.GenderType;
            branch.Status = dto.Status;
            branch.Description = dto.Description;
            branch.VisitCreditsCost = dto.VisitCreditsCost;
            branch.WorkingDays = dto.WorkingDays;
            branch.AmenitiesAvailable = dto.AmenitiesAvailable;
            branch.UpdatedAt = DateTime.UtcNow;

            await _branchRepository.SaveChangesAsync();
            return dto;
        }
    }
}
