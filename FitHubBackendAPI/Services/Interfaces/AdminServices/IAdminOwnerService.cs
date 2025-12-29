using FitHubBackendAPI.DTOs.AdminDtos;
using FitHubBackendAPI.Entities;

namespace FitHubBackendAPI.Services.Interfaces.AdminServices
{
    public interface IAdminOwnerService
    {
        Task<IEnumerable<GetOwnerForAdminDto>> GetAllGymOwners();
        Task<List<PendingOwnerDto>> GetPendingOwnersAsync();
        Task ApproveOwnerAsync(int ownerId);
        Task RejectOwnerAsync(int ownerId);
        //Task<decimal> GetSystemRevenueAsync();



    }
}
