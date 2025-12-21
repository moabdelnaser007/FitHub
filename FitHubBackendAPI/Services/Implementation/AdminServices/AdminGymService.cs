using FitHubBackendAPI.DTOs.AdminDtos;
using FitHubBackendAPI.DTOs.GymBranchDTOs;
using FitHubBackendAPI.Entities.Models;
using FitHubBackendAPI.Repository.Implementation;
using FitHubBackendAPI.Repository.Interfaces;
using FitHubBackendAPI.Services.Interfaces.AdminServices;

namespace FitHubBackendAPI.Services.Implementation.AdminServices
{
    
    public class AdminGymService : IAdminGymService
    {
        private readonly IGenericRepository<GymBranch> _branchRepository;
        public AdminGymService(IGenericRepository<GymBranch> branchRepository)
        {
            _branchRepository = branchRepository;
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
                CreatedAt = b.CreatedAt,
                UpdatedAt = b.UpdatedAt
            }).ToList();
            return ownerBranchDtos;
        }

        public async Task<IEnumerable<GetBranchForAdminDto>> GetAllGymBranchesAsync()
        {
            var branches = await _branchRepository.GetAllAsync();
            return branches.Select(b => new GetBranchForAdminDto
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
                CreatedAt = b.CreatedAt,
                UpdatedAt = b.UpdatedAt
            }).ToList();
        }

        public async Task<bool> ResumeGym(int branchId)
        {
            var branch = await _branchRepository.GetByIdAsync(branchId);
            if (branch == null) return false;

            branch.IsAcTive = true;
            await _branchRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SuspendGym(int branchId)
        {
            var branch = await _branchRepository.GetByIdAsync(branchId);
            if (branch == null) return false;

            branch.IsAcTive = false;
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
            branch.UpdatedAt = DateTime.UtcNow;

            await _branchRepository.SaveChangesAsync();
            return dto;
        }
    }
}
