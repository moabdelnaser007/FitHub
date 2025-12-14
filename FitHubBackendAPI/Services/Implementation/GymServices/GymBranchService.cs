using FitHubBackendAPI.Data;
using FitHubBackendAPI.DTOs.GymBranchDTOs;
using FitHubBackendAPI.Entities.Enums;
using FitHubBackendAPI.Entities.Models;
using FitHubBackendAPI.Repository.Interfaces;
using FitHubBackendAPI.Services.Interfaces.GymBranch;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FitHubBackendAPI.Services.Implementation.GymServices
{
    public class GymBranchService : IGymBranchService
    {

        private readonly IGenericRepository<GymBranch> _repository;
        public GymBranchService(IGenericRepository<GymBranch> repository)
        {

            _repository = repository;
        }
        public async Task<GetGymBranchByIdDTO> CreateGymBranchAsync(int userId, CreateGymBranchDTO dto)
        {
            await _repository.AddAsync(new GymBranch
            {
                OwnerId = userId,
                BranchName = dto.BranchName,
                Phone = dto.Phone,
                Address = dto.Address,
                City = dto.City,
                OpenTime = dto.OpenTime,
                CloseTime = dto.CloseTime,
                GenderType = dto.GenderType,
                Status = dto.Status
            });
            await _repository.SaveChangesAsync();

            return new GetGymBranchByIdDTO();
        }

        public Task DeactivateBranchAsync(int userId, int branchId)
        {
            var branch = _repository.GetByIdAsync(branchId).Result;
            if (branch == null)
            {
                throw new Exception("Branch not found");
            }
            if (branch.OwnerId != userId)
            {
                throw new Exception("You are not authorized to update this branch");
            }
            branch.Status = BranchStatus.INACTIVE;
            _repository.Update(branch);
            _repository.SaveChangesAsync();
            return Task.CompletedTask;

        }

        public async Task<IEnumerable<GetAllBranchDTO>> GetAllBranchesAsync(int userId)
        {
            var branches = _repository.GetAllAsync().Result
                .Where(b => b.OwnerId == userId)
                .Select(b => new GetAllBranchDTO
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
                Id = branch.Id,
                OwnerId = branch.OwnerId,
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

        public Task UpdateGymBranchAsync(int userId, UpdateGymBranchDTO dto)
        {
            var branch = _repository.GetByIdAsync(dto.Id).Result;
            if (branch == null)
            {
                throw new Exception("Branch not found");
            }
            if (branch.OwnerId != userId)
            {
                throw new Exception("You are not authorized to update this branch");
            }
            branch.BranchName = dto.BranchName;
            branch.Phone = dto.Phone;
            branch.Address = dto.Address;
            branch.City = dto.City;
            branch.OpenTime = dto.OpenTime;
            branch.CloseTime = dto.CloseTime;
            branch.GenderType = dto.GenderType;
            branch.Status = dto.Status;
            _repository.Update(branch);
            _repository.SaveChangesAsync();
            return Task.CompletedTask;
        }
        public Task ActivateGymBranchAsync(int userId, int branchId)
        {
            var branch = _repository.GetByIdAsync(branchId).Result;
            if (branch == null)
            {
                throw new Exception("Branch not found");
            }
            if (branch.OwnerId != userId)
            {
                throw new Exception("You are not authorized to update this branch");
            }
            branch.Status = BranchStatus.ACTIVE;
            _repository.Update(branch);
            _repository.SaveChangesAsync();
            return Task.CompletedTask;
        }
        public Task DeleteGymBranchAsync(int userId, int branchId)
        {
            var branch = _repository.GetByIdAsync(branchId).Result;
            if (branch == null)
            {
                throw new Exception("Branch not found");
            }
            if (branch.OwnerId != userId)
            {
                throw new Exception("You are not authorized to delete this branch");
            }
            _repository.Delete(branch);
            _repository.SaveChangesAsync();
            return Task.CompletedTask;
        }
    }
}
