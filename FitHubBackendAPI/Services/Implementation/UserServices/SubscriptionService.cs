using AutoMapper;
using FitHubBackendAPI.DTOs.Subscriptions;
using FitHubBackendAPI.Entities.Enums;
using FitHubBackendAPI.Entities.Models;
using FitHubBackendAPI.Repository.Interfaces;
using FitHubBackendAPI.Services.Interfaces;
using FitHubBackendAPI.Services.Interfaces.UserServices;
using FitHubBackendAPI.ViewModels;

namespace FitHubBackendAPI.Services.Implementation.UserServices
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly IGenericRepository<Subscription> _subscriptionRepo;
        private readonly IGenericRepository<GymPlan> _planRepo;
        private readonly IGenericRepository<GymBranch> _branchRepo;
        private readonly IGenericRepository<UserWallet> _walletRepo;
        private readonly IGenericRepository<OwnerWallet> _ownerWalletRepo;
        private readonly IGenericRepository<UserCreditTransactions> _txRepo;

        public SubscriptionService(
            IGenericRepository<Subscription> subscriptionRepo,
            IGenericRepository<GymPlan> planRepo,
            IGenericRepository<GymBranch> branchRepo,
            IGenericRepository<UserWallet> walletRepo,
            IGenericRepository<OwnerWallet> ownerWalletRepo,
            IGenericRepository<UserCreditTransactions> txRepo)
        {
            _subscriptionRepo = subscriptionRepo;
            _planRepo = planRepo;
            _branchRepo = branchRepo;
            _walletRepo = walletRepo;
            _ownerWalletRepo = ownerWalletRepo;
            _txRepo = txRepo;
        }

        // ================= Create =================
        public async Task<ResponseViewModel<bool>> CreateAsync(int userId, CreateSubscriptionDto dto)
        {
            var plan = await _planRepo.GetByIdAsync(dto.PlanId);
            if (plan == null || plan.Status != PlanStatus.ACTIVE)
                return ResponseViewModel<bool>.Fail("Plan not available");

            var branch = await _branchRepo.GetByIdAsync(dto.BranchId);
            if (branch == null)
                return ResponseViewModel<bool>.Fail("Branch not found");

            var wallet = (await _walletRepo.FindAsync(w => w.UserId == userId)).FirstOrDefault();
            if (wallet == null || (wallet.Balance ?? 0) < plan.CreditsCost)
                return ResponseViewModel<bool>.Fail("Insufficient balance");

            // خصم
            var before = wallet.Balance ?? 0;
            wallet.Balance = (wallet.Balance ?? 0) - plan.CreditsCost;
            _walletRepo.Update(wallet);

            // Owner Wallet
            var ownerWallet = (await _ownerWalletRepo.FindAsync(o => o.OwnerId == branch.OwnerId))
                .FirstOrDefault();

            if (ownerWallet == null)
            {
                ownerWallet = new OwnerWallet
                {
                    OwnerId = branch.OwnerId,
                    Balance = 0
                };
                await _ownerWalletRepo.AddAsync(ownerWallet);
            }

            ownerWallet.Balance += plan.CreditsCost;
            _ownerWalletRepo.Update(ownerWallet);

            var subscription = new Subscription
            {
                UserId = userId,
                BranchId = dto.BranchId,
                PlanId = dto.PlanId,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(plan.DurationDays ?? 30),
                VisitsAllowed = plan.VisitsLimit ?? 0,
                VisitsUsed = 0,
                Status = SubscriptionStatus.ACTIVE
            };

            await _subscriptionRepo.AddAsync(subscription);
            await _subscriptionRepo.SaveChangesAsync(); // 🔥 مهم جدًا

            await _txRepo.AddAsync(new UserCreditTransactions
            {
                UserId = userId,
                CreditsBefore = before,
                CreditsChanged = -plan.CreditsCost,
                CreditsAfter = wallet.Balance,
                PaymentAmount = (decimal)plan.CreditsCost,
                TransactionType = TransactionType.DEDUCT,
                Source = TransactionSource.SUBSCRIPTION,
                Description = "Subscription purchase",
                ReferenceId = subscription.Id
            });

            await _walletRepo.SaveChangesAsync();

            return ResponseViewModel<bool>.Success(true, "Subscription created successfully");
        }



        // ================= My Subscriptions =================
        public async Task<ResponseViewModel<List<SubscriptionListDto>>> GetMyAsync(int userId)
        {
            var subs = await _subscriptionRepo.GetAsync(
                s => s.UserId == userId,
                includeProperties: "Plan,Branch");

            var result = subs.Select(s => new SubscriptionListDto
            {
                SubscriptionId = s.Id,
                BranchName = s.Branch.BranchName!,
                PlanName = s.Plan.Name!,
                RemainingVisits = s.VisitsAllowed - s.VisitsUsed,
                Status = s.Status,
                EndDate = s.EndDate
            }).ToList();

            return ResponseViewModel<List<SubscriptionListDto>>.Success(result);
        }

        // ================= Details =================
        public async Task<ResponseViewModel<SubscriptionDetailsDto>> GetByIdAsync(int userId, int subscriptionId)
        {
            var sub = (await _subscriptionRepo.GetAsync(
                s => s.Id == subscriptionId,
                includeProperties: "Plan,Branch")).FirstOrDefault();

            if (sub == null || sub.UserId != userId)
                return ResponseViewModel<SubscriptionDetailsDto>.Fail("Not found");

            if (sub.Branch == null || sub.Plan == null)
                return ResponseViewModel<SubscriptionDetailsDto>.Fail("Subscription data is incomplete");

            return ResponseViewModel<SubscriptionDetailsDto>.Success(new SubscriptionDetailsDto
            {
                SubscriptionId = sub.Id,
                BranchName = sub.Branch.BranchName!,
                PlanName = sub.Plan.Name!,
                StartDate = sub.StartDate,
                EndDate = sub.EndDate,
                VisitsAllowed = sub.VisitsAllowed,
                VisitsUsed = sub.VisitsUsed,
                RemainingVisits = sub.VisitsAllowed - sub.VisitsUsed,
                Status = sub.Status
            });
        }

        // ================= Cancel =================
        public async Task<ResponseViewModel<bool>> CancelAsync(int userId, int subscriptionId)
        {
            var sub = await _subscriptionRepo.GetByIdAsync(subscriptionId);
            if (sub == null || sub.UserId != userId)
                return ResponseViewModel<bool>.Fail("Not found");

            sub.Status = SubscriptionStatus.CANCELLED;
            _subscriptionRepo.Update(sub);

            await _subscriptionRepo.SaveChangesAsync();
            return ResponseViewModel<bool>.Success(true);
        }
    }
}