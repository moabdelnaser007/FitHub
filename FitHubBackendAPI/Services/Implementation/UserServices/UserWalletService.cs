using AutoMapper;
using FitHubBackendAPI.DTOs.Wallet;
using FitHubBackendAPI.Entities.Enums;
using FitHubBackendAPI.Entities.Models;
using FitHubBackendAPI.Repository.Interfaces;
using FitHubBackendAPI.Services.Interfaces.UserServices;
using FitHubBackendAPI.ViewModels;

namespace FitHubBackendAPI.Services.Implementation.UserServices
{
    public class UserWalletService : IUserWalletService
    {
        private readonly IGenericRepository<UserWallet> _walletRepo;
        private readonly IGenericRepository<UserCreditTransactions> _transRepo;
        private readonly IGenericRepository<FithubPlan> _planRepo;
        private readonly IGenericRepository<FithubUserPlan> _userPlanRepo;
        private readonly IMapper _mapper;

        public UserWalletService(
            IGenericRepository<UserWallet> walletRepo,
            IGenericRepository<UserCreditTransactions> transRepo,
            IGenericRepository<FithubPlan> planRepo,
            IGenericRepository<FithubUserPlan> userPlanRepo,
            IMapper _mapper)
        {
            _walletRepo = walletRepo;
            _transRepo = transRepo;
            _planRepo = planRepo;
            _userPlanRepo = userPlanRepo;
            this._mapper = _mapper;
        }

        // ==========================================
        // 1. دالة عرض الباقات (Plans)
        // ==========================================
        public async Task<ResponseViewModel<List<FithubPlanDto>>> GetAllPlansAsync()
        {
            var plans = await _planRepo.GetAsync(p => p.IsAcTive && !p.IsDeleted);
            var dtos = _mapper.Map<List<FithubPlanDto>>(plans);
            return ResponseViewModel<List<FithubPlanDto>>.Success(dtos);
        }

        // ==========================================
        // 2. دالة شراء باقة (Purchase)
        // ==========================================
        public async Task<ResponseViewModel<bool>> PurchasePlanAsync(int userId, int planId)
        {
            try
            {
                // أ: التأكد من وجود الباقة
                var plan = await _planRepo.GetByIdAsync(planId);
                if (plan == null || !plan.IsAcTive)
                    return ResponseViewModel<bool>.Fail("Selected plan not found.");

                // ب: حسابات الفلوس
                decimal basePrice = plan.Price;
                decimal taxRate = 0.15m;
                decimal taxAmount = basePrice * taxRate;
                decimal totalAmount = basePrice + taxAmount;

                // ج: التعامل مع المحفظة
                var wallets = await _walletRepo.FindAsync(w => w.UserId == userId);
                var wallet = wallets.FirstOrDefault();
                int oldBalance = 0;

                if (wallet == null)
                {
                    wallet = new UserWallet
                    {
                        UserId = userId,
                        Balance = 0,
                        IsAcTive = true,
                        CreatedAt = DateTime.UtcNow
                    };
                    await _walletRepo.AddAsync(wallet);
                }

                oldBalance = wallet.Balance ?? 0;

                // د: تحديث الرصيد
                wallet.Balance = oldBalance + plan.CreditsValue;
                wallet.LastUpdated = DateTime.UtcNow;
                _walletRepo.Update(wallet);

                // هـ: تسجيل الفاتورة
                var userPlan = new FithubUserPlan
                {
                    UserId = userId,
                    PlanId = plan.Id,
                    BasePrice = basePrice,
                    TaxAmount = taxAmount,
                    TotalAmount = totalAmount,
                    PurchaseDate = DateTime.UtcNow,
                    IsAcTive = true
                };
                await _userPlanRepo.AddAsync(userPlan);

                // و: تسجيل الحركة في الهيستوري
                var transaction = new UserCreditTransactions
                {
                    UserId = userId,
                    CreditsBefore = oldBalance,
                    CreditsChanged = plan.CreditsValue,
                    CreditsAfter = wallet.Balance,

                    TransactionType = TransactionType.RECHARGE, // نوع العملية: شحن
                    Source = TransactionSource.MANUAL,          // المصدر: يدوي (لأن اليوزر هو اللي اشترى بنفسه)

                    PaymentAmount = totalAmount,
                    Description = $"Purchased {plan.Name} Plan",
                    IsAcTive = true,
                    CreatedAt = DateTime.UtcNow
                };
                await _transRepo.AddAsync(transaction);

                // ز: حفظ التغييرات
                await _walletRepo.SaveChangesAsync();

                return ResponseViewModel<bool>.Success(true, "Plan purchased successfully.");
            }
            catch (Exception ex)
            {
                return ResponseViewModel<bool>.Fail($"Purchase failed: {ex.Message}");
            }
        }

        // ==========================================
        // 3. دالة عرض الرصيد
        // ==========================================
        public async Task<ResponseViewModel<WalletBalanceDto>> GetWalletBalanceAsync(int userId)
        {
            var wallets = await _walletRepo.FindAsync(w => w.UserId == userId);
            var wallet = wallets.FirstOrDefault();

            if (wallet == null)
                return ResponseViewModel<WalletBalanceDto>.Success(new WalletBalanceDto { UserId = userId, Balance = 0 });

            var walletDto = _mapper.Map<WalletBalanceDto>(wallet);
            return ResponseViewModel<WalletBalanceDto>.Success(walletDto);
        }

        // ==========================================
        // 4. دالة سجل المعاملات
        // ==========================================
        public async Task<ResponseViewModel<List<TransactionHistoryDto>>> GetTransactionHistoryAsync(int userId)
        {
            try
            {
                var transactions = await _transRepo.GetAsync(
                    filter: t => t.UserId == userId,
                    orderBy: q => q.OrderByDescending(t => t.CreatedAt)
                );

                var dtos = _mapper.Map<List<TransactionHistoryDto>>(transactions);
                return ResponseViewModel<List<TransactionHistoryDto>>.Success(dtos);
            }
            catch (Exception ex)
            {
                return ResponseViewModel<List<TransactionHistoryDto>>.Fail($"Error fetching transactions: {ex.Message}");
            }
        }
    }
}