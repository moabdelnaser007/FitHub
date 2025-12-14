using FitHubBackendAPI.DTOs.PlanDTOs;

namespace FitHubBackendAPI.Services.Interfaces.GymBranch
{
    public interface IPlanService
    {
        Task<GetPlanByIdDTO> CreatePlanAsync(CreatePlanDTO createPlanDto);
        Task<GetPlanByIdDTO> UpdatePlanAsync(UpdatePlanDTO updatePlanDto);
        Task<bool> DeletePlanAsync(int planId);
        Task<IEnumerable<GetPlanByBranchIdDTO>> GetPlansByBranchIdAsync(int branchId);
        Task<GetPlanByIdDTO> GetPlanByIdAsync(int planId);
        Task ActivatePlanAsync(int planId);
        Task DeactivatePlanAsync(int planId);
    }
}
