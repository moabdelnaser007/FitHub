using FitHubBackendAPI.DTOs.OwnerWallet;
using FitHubBackendAPI.Entities.Enums;
using FitHubBackendAPI.Entities.Models;
using FitHubBackendAPI.Repository.Interfaces;
using FitHubBackendAPI.Services.Interfaces.OwnerServices;
using FitHubBackendAPI.ViewModels;

namespace FitHubBackendAPI.Services.Implementation.GymServices
{
    public class OwnerWalletService: IOwnerWalletService
    {
        private readonly IGenericRepository<GymOwner> _ownerRepo;
        private readonly IGenericRepository<OwnerWallet> _walletRepo;
        private readonly IGenericRepository<UserCreditTransactions> _transactionsRepo;
        private readonly IGenericRepository<GymBranch> _branchRepo;

        public OwnerWalletService(
            IGenericRepository<GymOwner> ownerRepo,
            IGenericRepository<OwnerWallet> walletRepo,
            IGenericRepository<UserCreditTransactions> transactionsRepo,
            IGenericRepository<GymBranch> branchRepo)
        {
            _ownerRepo = ownerRepo;
            _walletRepo = walletRepo;
            _transactionsRepo = transactionsRepo;
            _branchRepo = branchRepo;
        }

        // ================= Balance =================
        public async Task<ResponseViewModel<OwnerWalletBalanceDto>> GetBalanceAsync(int ownerUserId)
        {
            var owner = (await _ownerRepo.FindAsync(o => o.UserId == ownerUserId)).FirstOrDefault();
            if (owner == null)
                return ResponseViewModel<OwnerWalletBalanceDto>.Fail("Owner not found");

            var wallet = (await _walletRepo.FindAsync(w => w.OwnerId == owner.Id)).FirstOrDefault();

            return ResponseViewModel<OwnerWalletBalanceDto>.Success(new OwnerWalletBalanceDto
            {
                OwnerId = owner.Id,
                Balance = wallet?.Balance ?? 0
            });
        }

        // ================= Transactions =================
        public async Task<ResponseViewModel<List<OwnerTransactionDto>>> GetTransactionsAsync(int ownerUserId)
        {
            var owner = (await _ownerRepo.FindAsync(o => o.UserId == ownerUserId)).FirstOrDefault();
            if (owner == null)
                return ResponseViewModel<List<OwnerTransactionDto>>.Fail("Owner not found");

            var branchIds = (await _branchRepo.FindAsync(b => b.OwnerId == owner.Id))
                .Select(b => b.Id)
                .ToList();

            var transactions = await _transactionsRepo.GetAsync(
                t => t.Source != TransactionSource.MANUAL && t.ReferenceId != null,
                orderBy: q => q.OrderByDescending(x => x.CreatedAt)
            );

            var ownerTransactions = transactions
                .Where(t => t.Source == TransactionSource.VISIT || t.Source == TransactionSource.SUBSCRIPTION)
                .Select(t => new OwnerTransactionDto
                {
                    Id = t.Id,
                    Date = t.CreatedAt.ToString("MMM dd, yyyy"),
                    Amount = t.PaymentAmount > 0 ? t.PaymentAmount : Math.Abs(t.CreditsChanged ?? 0),
                    Source = t.Source,
                    Description = t.Description
                })
                .ToList();

            return ResponseViewModel<List<OwnerTransactionDto>>.Success(ownerTransactions);
        }

        // ================= Branch Revenue =================
        public async Task<ResponseViewModel<BranchRevenueDto>> GetBranchRevenueAsync(int ownerUserId, int branchId)
        {
            var owner = (await _ownerRepo.FindAsync(o => o.UserId == ownerUserId)).FirstOrDefault();
            if (owner == null)
                return ResponseViewModel<BranchRevenueDto>.Fail("Owner not found");

            var branch = await _branchRepo.GetByIdAsync(branchId);
            if (branch == null || branch.OwnerId != owner.Id)
                return ResponseViewModel<BranchRevenueDto>.Fail("Branch not found");

            var transactions = await _transactionsRepo.GetAsync(
                t => t.ReferenceId == branchId &&
                     (t.Source == TransactionSource.VISIT || t.Source == TransactionSource.SUBSCRIPTION)
            );

            var total = transactions.Sum(t => t.PaymentAmount > 0
                ? t.PaymentAmount
                : Math.Abs(t.CreditsChanged ?? 0));

            return ResponseViewModel<BranchRevenueDto>.Success(new BranchRevenueDto
            {
                BranchId = branch.Id,
                BranchName = branch.BranchName!,
                TotalRevenue = total
            });
        }
    }
}
