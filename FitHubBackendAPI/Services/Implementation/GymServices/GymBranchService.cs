using FitHubBackendAPI.Data;
using FitHubBackendAPI.DTOs.GymBranchDTOs;
using FitHubBackendAPI.Entities;
using FitHubBackendAPI.Entities.Enums;
using FitHubBackendAPI.Entities.Models;
using FitHubBackendAPI.Repository.Interfaces;

using FitHubBackendAPI.Services.Interfaces.OwnerServices;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FitHubBackendAPI.Services.Implementation.GymServices
{
    public class GymBranchService : IGymBranchService
    {

        private readonly IGenericRepository<GymBranch> _repository;
        private readonly IGenericRepository<GymOwner> _ownerRepository;
        public GymBranchService(IGenericRepository<GymBranch> repository, IGenericRepository<GymOwner> ownerRepository)
        {
            _ownerRepository = ownerRepository;
            _repository = repository;
        }
        public async Task<Entities.Models.GymBranch> CreateGymBranchAsync(int userId, CreateGymBranchDTO dto)
        {
            var owners = await _ownerRepository.FindAsync(o=>o.UserId==userId);
            var owner = owners.FirstOrDefault();
            Entities.Models.GymBranch branch = new Entities.Models.GymBranch
            {
                OwnerId = owner.Id,
                BranchName = dto.BranchName,
                Phone = dto.Phone,
                Address = dto.Address,
                City = dto.City,
                OpenTime = dto.OpenTime,
                CloseTime = dto.CloseTime,
                GenderType = dto.GenderType,
                Status = dto.Status
            };
            await _repository.AddAsync(branch);
                
            await _repository.SaveChangesAsync();

            return branch;
        }

        public async Task DeactivateBranchAsync(int userId, int branchId)
        {
            var owners = await _ownerRepository.FindAsync(o => o.UserId == userId);
            var owner = owners.FirstOrDefault();
            var branch = await _repository.GetByIdAsync(branchId);
            if (branch == null)
            {
                throw new Exception("Branch not found");
            }
            if (branch.OwnerId != owner.Id)
            {
                throw new Exception("You are not authorized to update this branch");
            }
            branch.Status = BranchStatus.INACTIVE;
            _repository.Update(branch);
            await _repository.SaveChangesAsync();
            

        }

        public async Task<IEnumerable<GetAllBranchDTO>> GetAllBranchesAsync(int userId)
        {
            var owners = await _ownerRepository.FindAsync(o => o.UserId == userId);
            var owner = owners.FirstOrDefault();
            var branches = _repository.GetAllAsync().Result
                .Where(b => b.OwnerId == owner.Id)
                .Select(b => new GetAllBranchDTO
                {
                    //Id = b.Id,
                    //OwnerId = b.OwnerId,
                    BranchName = b.BranchName,
                    Phone = b.Phone,
                    Address = b.Address,
                    City = b.City,
                    OpenTime = b.OpenTime,
                    CloseTime = b.CloseTime,
                    GenderType = b.GenderType,
                    Status = b.Status
                });
            return await branches.ToListAsync();
        }

        public async Task<GetGymBranchByIdDTO> GetGymBranchByIdAsync(int branchId)
        {
            var branch = await _repository.GetByIdAsync(branchId);
            if (branch == null)
                throw new Exception("Branch not found");
            
            return new GetGymBranchByIdDTO
            {
                //Id = branch.Id,
                //OwnerId = branch.OwnerId,
                BranchName = branch.BranchName,
                Phone = branch.Phone,
                Address = branch.Address,
                City = branch.City,
                OpenTime = branch.OpenTime,
                CloseTime = branch.CloseTime,
                GenderType = branch.GenderType,
                Status = branch.Status

            };
        }

        public async Task UpdateGymBranchAsync(int userId, UpdateGymBranchDTO dto, int BranchId)
        {
            // 1️⃣ هات الـ Owner
            var owner = (await _ownerRepository.FindAsync(o => o.UserId == userId))
                        .FirstOrDefault();

            if (owner == null)
                throw new Exception("Owner not found");

            // 2️⃣ هات الفرع
            var branch = await _repository.GetByIdAsync(BranchId);

            if (branch == null)
                throw new Exception("Branch not found");

            // 3️⃣ تأكد إن الفرع تابع لنفس الـ Owner
            if (branch.OwnerId != owner.Id)
                throw new Exception("You are not authorized to update this branch");

            // 4️⃣ Update
            branch.BranchName = dto.BranchName;
            branch.Phone = dto.Phone;
            branch.Address = dto.Address;
            branch.City = dto.City;
            branch.OpenTime = dto.OpenTime;
            branch.CloseTime = dto.CloseTime;
            branch.GenderType = dto.GenderType;
            branch.Status = dto.Status;

            _repository.Update(branch);
            await _repository.SaveChangesAsync();

        }
        public async Task ActivateGymBranchAsync(int userId, int branchId)
        {
            var owners = await _ownerRepository.FindAsync(o => o.UserId == userId);
            var owner = owners.FirstOrDefault();
            var branch = await _repository.GetByIdAsync(branchId);
            if (branch == null)
            {
                throw new Exception("Branch not found");
            }
            if (branch.OwnerId != owner.Id)
            {
                throw new Exception("You are not authorized to update this branch");
            }
            branch.Status = BranchStatus.ACTIVE;
            _repository.Update(branch);
            await _repository.SaveChangesAsync();
        }
        public async Task DeleteGymBranchAsync(int userId, int branchId)
        {
            var owners = await _ownerRepository.FindAsync(o => o.UserId == userId);
            var owner = owners.FirstOrDefault();
            var branch = await _repository.GetByIdAsync(branchId);
            if (branch == null)
            {
                throw new Exception("Branch not found");
            }
            if (branch.OwnerId != owner.Id)
            {
                throw new Exception("You are not authorized to delete this branch");
            }
            await _repository.SoftDelete(branch);
            await _repository.SaveChangesAsync();
        }
    }
}
