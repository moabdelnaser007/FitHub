using FitHubBackendAPI.DTOs.SettlementDto;
using FitHubBackendAPI.ViewModels;

namespace FitHubBackendAPI.Services.Interfaces.SettlementsService
{
    public interface IOwnerSettlementService
    {
        // Owner
        Task<ResponseViewModel<bool>> CreateSettlementAsync(int ownerId, CreateSettlementDto dto);
        Task<ResponseViewModel<List<SettlementDto>>> GetOwnerSettlementsAsync(int ownerId);

        // Admin
        Task<ResponseViewModel<List<SettlementDto>>> GetAllSettlementsAsync();
        Task<ResponseViewModel<bool>> UpdateSettlementStatusAsync(int settlementId, UpdateSettlementStatusDto dto);
    }
}
