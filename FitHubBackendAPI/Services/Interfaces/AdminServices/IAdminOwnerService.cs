using FitHubBackendAPI.Entities;

namespace FitHubBackendAPI.Services.Interfaces.AdminServices
{
    public interface IAdminOwnerService
    {
        Task<List<GymOwner>> GetPendingOwnersAsync();
        Task ApproveOwnerAsync(int ownerId);
        Task RejectOwnerAsync(int ownerId);
    }
}
