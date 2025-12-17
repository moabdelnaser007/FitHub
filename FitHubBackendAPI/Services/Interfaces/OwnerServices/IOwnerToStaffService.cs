using FitHubBackendAPI.DTOs.StuffDTOs;

namespace FitHubBackendAPI.Services.Interfaces.GymBranch
{
    public interface IOwnerToStaffService
    {
        Task<GetStuffDTO?> GetStaffMemberByIdAsync(int staffId);
        Task<IEnumerable<GetStuffDTO>> GetAllStaffMembersAsync(int branchId);
        Task<UpdateStaffDTO> UpdateStaff(UpdateStaffDTO dto);
        Task<bool> DeleteStaffMemberAsync(int staffId);
        Task<bool> AssignStaffToBranch(int staffId, int branchId);
    }
}
