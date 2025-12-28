using FitHubBackendAPI.DTOs.SettlementDto;

namespace FitHubBackendAPI.Services.Interfaces.SettlementsService
{
    public interface IAdminSettlementService
    {
        Task<IEnumerable<AdminSettlementDto>> GetAllSettlementsAsync();
        Task<AdminSettlementDto?> GetSettlementByIdAsync(int settlementId);
    }
}
