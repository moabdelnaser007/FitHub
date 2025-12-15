using FitHubBackendAPI.DTOs.GymBranchDTOs;

namespace FitHubBackendAPI.Services.Interfaces.GymBranch
{
    public interface IGymBranchService
    {
        
        Task<GetGymBranchByIdDTO> CreateGymBranchAsync(int userId, CreateGymBranchDTO dto);
        Task UpdateGymBranchAsync(int userId, UpdateGymBranchDTO dto);
        Task<IEnumerable<GetAllBranchDTO>> GetAllBranchesAsync(int userId);
        Task<GetGymBranchByIdDTO> GetGymBranchByIdAsync(int branchId);
        Task DeactivateBranchAsync(int userId, int branchId);
        Task ActivateGymBranchAsync(int userId, int branchId);
        Task DeleteGymBranchAsync(int userId, int branchId);
    }
}
