using FitHubBackendAPI.DTOs.SettlementDto;
using FitHubBackendAPI.Entities.Enums;
using FitHubBackendAPI.Entities.Models;
using FitHubBackendAPI.Repository.Interfaces;
using FitHubBackendAPI.Services.Interfaces.SettlementsService;
using FitHubBackendAPI.ViewModels;

namespace FitHubBackendAPI.Services.Implementation.SettlementsService
{
    public class OwnerSettlementService : IOwnerSettlementService
    {
        private readonly IGenericRepository<OwnerSettlement> _settlementRepo;
        private readonly IGenericRepository<OwnerWallet> _walletRepo;

        public OwnerSettlementService(
            IGenericRepository<OwnerSettlement> settlementRepo,
            IGenericRepository<OwnerWallet> walletRepo)
        {
            _settlementRepo = settlementRepo;
            _walletRepo = walletRepo;
        }

        // ================= OWNER =================

        public async Task<ResponseViewModel<bool>> CreateSettlementAsync(int ownerId, CreateSettlementDto dto)
        {
            var wallet = (await _walletRepo.FindAsync(w => w.OwnerId == ownerId))
                .FirstOrDefault();

            if (wallet == null || wallet.Balance <= 0)
                return ResponseViewModel<bool>.Fail("Owner wallet is empty");

            if (dto.Amount <= 0 || dto.Amount > wallet.Balance)
                return ResponseViewModel<bool>.Fail("Invalid settlement amount");

            var settlement = new OwnerSettlement
            {
                OwnerId = ownerId,
                TotalExpectedPayout = dto.Amount,
                PayoutStatus = SettlementStatus.PENDING,
                CreatedAt = DateTime.UtcNow
            };

            await _settlementRepo.AddAsync(settlement);
            await _settlementRepo.SaveChangesAsync();

            return ResponseViewModel<bool>.Success(true, "Settlement request created");
        }

        public async Task<ResponseViewModel<List<SettlementDto>>> GetOwnerSettlementsAsync(int ownerId)
        {
            var settlements = await _settlementRepo.GetAsync(s => s.OwnerId == ownerId);

            var result = settlements.Select(s => new SettlementDto
            {
                Id = s.Id,
                Amount = s.TotalExpectedPayout ?? 0,
                Status = s.PayoutStatus,
                CreatedAt = s.CreatedAt,
                PayoutDate = s.PayoutDate,
                AdminNotes = s.AdminNotes
            }).ToList();

            return ResponseViewModel<List<SettlementDto>>.Success(result);
        }

        // ================= ADMIN =================

        public async Task<ResponseViewModel<List<SettlementDto>>> GetAllSettlementsAsync()
        {
            var settlements = await _settlementRepo.GetAsync();

            var result = settlements.Select(s => new SettlementDto
            {
                Id = s.Id,
                Amount = s.TotalExpectedPayout ?? 0,
                Status = s.PayoutStatus,
                CreatedAt = s.CreatedAt,
                PayoutDate = s.PayoutDate,
                AdminNotes = s.AdminNotes
            }).ToList();

            return ResponseViewModel<List<SettlementDto>>.Success(result);
        }

        public async Task<ResponseViewModel<bool>> UpdateSettlementStatusAsync(int settlementId, UpdateSettlementStatusDto dto)
        {
            var settlement = await _settlementRepo.GetByIdAsync(settlementId);
            if (settlement == null)
                return ResponseViewModel<bool>.Fail("Settlement not found");

            if (settlement.PayoutStatus != SettlementStatus.PENDING)
                return ResponseViewModel<bool>.Fail("Settlement already processed");

            if (dto.Approve)
            {
                var wallet = (await _walletRepo.FindAsync(w => w.OwnerId == settlement.OwnerId))
                    .FirstOrDefault();

                if (wallet == null || wallet.Balance < settlement.TotalExpectedPayout)
                    return ResponseViewModel<bool>.Fail("Insufficient owner balance");

                wallet.Balance -= settlement.TotalExpectedPayout;
                wallet.LastUpdated = DateTime.UtcNow;
                _walletRepo.Update(wallet);

                settlement.PayoutStatus = SettlementStatus.PAID;
                settlement.PayoutDate = DateTime.UtcNow;
            }
            else
            {
                settlement.PayoutStatus = SettlementStatus.FAILED;
            }

            settlement.AdminNotes = dto.AdminNotes;

            _settlementRepo.Update(settlement);
            await _settlementRepo.SaveChangesAsync();

            return ResponseViewModel<bool>.Success(true, "Settlement updated");
        }
    }
}
