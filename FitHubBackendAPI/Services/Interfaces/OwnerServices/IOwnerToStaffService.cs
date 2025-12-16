namespace FitHubBackendAPI.Services.Interfaces.GymBranch
{
    public interface IOwnerToStaffService
    {
        Task<bool> DeleteStaffMemberAsync(int staffId);
        Task<bool> AssignStaffToBranch(int staffId, int branchId);
    }
}
