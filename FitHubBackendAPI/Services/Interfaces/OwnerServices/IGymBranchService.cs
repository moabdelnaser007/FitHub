using FitHubBackendAPI.DTOs.GymBranchDTOs;


namespace FitHubBackendAPI.Services.Interfaces.OwnerServices
{
    public interface IGymBranchService
    {

        Task<Entities.Models.GymBranch> CreateGymBranchAsync(int userId, CreateGymBranchDTO dto/*, List<IFormFile> images*/);
        Task UpdateGymBranchAsync(int userId, UpdateGymBranchDTO dto,int BranchId);
        Task<IEnumerable<GetAllBranchDTO>> GetAllBranchesAsync(int userId);
        Task<GetGymBranchByIdDTO> GetGymBranchByIdAsync(int branchId);
        Task DeactivateBranchAsync(int userId, int branchId);
        Task ActivateGymBranchAsync(int userId, int branchId);
        Task DeleteGymBranchAsync(int userId, int branchId);
        Task<GetGymBranchByIdDTO> GetActiveGymBranchByIdAsync(int branchId);
        Task<IEnumerable<GetAllBranchDTO>> GetAllActiveBranchesAsync();
        Task<bool> AddImagesToBranchAsync(int branchId, List<IFormFile> images);
        Task<IEnumerable<GetBranchImagePathDto>> GetBranchImagesAsync(int  branchId);
        Task<bool> RemoveImageFromBranchAsync(int branchId, string imageName);
        Task<GetBranchImagePathDto> GetBranchImagePathAsync(int branchId, string imageName);
        Task<bool> SetCoverImage(string imageName, int branchId);
    }
}
