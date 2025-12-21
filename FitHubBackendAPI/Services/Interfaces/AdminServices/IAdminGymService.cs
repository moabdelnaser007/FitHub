using FitHubBackendAPI.DTOs.AdminDtos;
using FitHubBackendAPI.DTOs.GymBranchDTOs;

namespace FitHubBackendAPI.Services.Interfaces.AdminServices
{
    public interface IAdminGymService
    {
        Task<IEnumerable<GetBranchForAdminDto>> GetAllGymBranchesAsync();
        Task<IEnumerable<GetBranchForAdminDto>> GetAllBranchOfOwner(int ownerId);
        Task<IEnumerable<GetBranchForAdminDto>> GetSuspendedBranches();
        Task<GetBranchForAdminDto> GetGymBranchByIdAsync(int branchId);
        Task<bool> SuspendGym(int branchId);
        Task<bool> ResumeGym(int branchId);
        Task<UpdateGymBranchDTO> UpdateGymBranchAsync(UpdateGymBranchDTO dto, int branchId);
    }
}
