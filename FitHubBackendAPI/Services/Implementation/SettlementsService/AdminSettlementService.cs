using FitHubBackendAPI.DTOs.SettlementDto;
using FitHubBackendAPI.Entities.Enums;
using FitHubBackendAPI.Entities.Models;
using FitHubBackendAPI.Repository.Interfaces;
using FitHubBackendAPI.Services.Interfaces.SettlementsService;
using FitHubBackendAPI.ViewModels;

namespace FitHubBackendAPI.Services.Implementation.SettlementsService
{
    public class AdminSettlementService : IAdminSettlementService
    {
        private readonly IGenericRepository<OwnerSettlement> _settlementRepo;

        public AdminSettlementService(
            IGenericRepository<OwnerSettlement> settlementRepo)
        {
            _settlementRepo = settlementRepo;
        }

        public async Task<IEnumerable<AdminSettlementDto>> GetAllSettlementsAsync()
        {
            var settlements = await _settlementRepo.GetAsync();

            return settlements.Select(s => new AdminSettlementDto
            {
                Id = s.Id,
                OwnerId = s.OwnerId,
                TotalCreditsConsumed = s.TotalCreditsConsumed ?? 0,
                TotalExpectedPayout = s.TotalExpectedPayout ?? 0,
                PayoutStatus = s.PayoutStatus,
                CreatedAt = s.CreatedAt,
                AdminNotes = s.AdminNotes
            });
        }

        // get settlement by id
        public async Task<AdminSettlementDto?> GetSettlementByIdAsync(int settlementId)
        {
            var settlement = await _settlementRepo.GetByIdAsync(settlementId);
            if (settlement == null)
            {
                return null;
            }
            return new AdminSettlementDto
            {
                Id = settlement.Id,
                OwnerId = settlement.OwnerId,
                TotalCreditsConsumed = settlement.TotalCreditsConsumed ?? 0,
                TotalExpectedPayout = settlement.TotalExpectedPayout ?? 0,
                PayoutStatus = settlement.PayoutStatus,
                CreatedAt = settlement.CreatedAt,
                AdminNotes = settlement.AdminNotes
            };

        }
    }
}

