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
        private readonly IGenericRepository<UserCreditTransactions> _transactionRepo;
        private readonly IMapper _mapper;

        public UserWalletService(
            IGenericRepository<UserWallet> walletRepo,
            IGenericRepository<UserCreditTransactions> transactionRepo,
            IMapper mapper)
        {
            _walletRepo = walletRepo;
            _transactionRepo = transactionRepo;
            _mapper = mapper;
        }

        // ==========================================
        // 1. دالة الشحن (Charge) 
        // ==========================================
        public async Task<ResponseViewModel<bool>> ChargeWalletAsync(int userId, ChargeWalletDto dto)
        {
            try
            {
                // 1. بنروح ندور هل اليوزر ده عنده محفظة أصلاً ولا لأ
                var wallets = await _walletRepo.FindAsync(w => w.UserId == userId);
                var wallet = wallets.FirstOrDefault();

                // لو معندوش محفظة (أول مرة يشحن)، بنعمله واحدة جديدة
                if (wallet == null)
                {
                    wallet = new UserWallet
                    {
                        UserId = userId,
                        Balance = 0,
                        LastUpdated = DateTime.UtcNow
                    };
                    // بنضيفها للكونتكس (حالتها دلوقتي Added)
                    await _walletRepo.AddAsync(wallet);
                }

                // ====================================================
                // ✅ حساب الكريديت داخل الباك إند (Security Logic)
                // ====================================================

                // هنا بنحدد سعر التحويل (حالياً 1 جنيه = 1 كريديت)
                int conversionRate = 1;

                // بنحسب عدد الكريديت اللي هيتضاف
                int creditsToAdd = dto.AmountPaid * conversionRate;

                // 2. بنحسب الرصيد القديم والجديد
                int oldBalance = wallet.Balance;
                int newBalance = oldBalance + creditsToAdd;

                // 3. نحدث رصيد المحفظة الفعلي
                wallet.Balance = newBalance;
                wallet.LastUpdated = DateTime.UtcNow;

                // 4. تسجيل العملية في الهيستوري
                var transaction = new UserCreditTransactions
                {
                    UserId = userId,
                    CreditsBefore = oldBalance,
                    CreditsChanged = creditsToAdd,
                    CreditsAfter = newBalance,

                    // ✅✅✅ التعديل الجديد ✅✅✅
                    // بناخد المبلغ من الـ DTO ونخزنه عشان يظهر في الهيستوري
                    PaymentAmount = dto.AmountPaid,

                    TransactionType = TransactionType.RECHARGE,
                    Source = TransactionSource.MANUAL,

                    IsAcTive = true,
                    CreatedAt = DateTime.UtcNow
                };

                await _transactionRepo.AddAsync(transaction);

                // 5. حفظ التغييرات (هنا بيتم الحفظ الفعلي لكل العمليات مرة واحدة)
                await _walletRepo.SaveChangesAsync();

                return ResponseViewModel<bool>.Success(true, $"Wallet charged successfully with {creditsToAdd} credits");
            }
            catch (Exception ex)
            {
                // بنعرض الـ InnerException عشان لو فيه تفاصيل أكتر للإيرور تظهر
                var msg = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return ResponseViewModel<bool>.Fail($"Error charging wallet: {msg}");
            }
        }

        // ==========================================
        // 2. دالة عرض الرصيد (Get Balance)
        // ==========================================
        public async Task<ResponseViewModel<WalletBalanceDto>> GetWalletBalanceAsync(int userId)
        {
            var wallets = await _walletRepo.FindAsync(w => w.UserId == userId);
            var wallet = wallets.FirstOrDefault();

            if (wallet == null)
            {
                return ResponseViewModel<WalletBalanceDto>.Success(new WalletBalanceDto { Balance = 0 });
            }

            var walletDto = _mapper.Map<WalletBalanceDto>(wallet);
            return ResponseViewModel<WalletBalanceDto>.Success(walletDto);
        }


        // ==========================================
        // 3. دالة عرض سجل المعاملات (History)
        // ==========================================
        public async Task<ResponseViewModel<IEnumerable<TransactionHistoryDto>>> GetMyTransactionsAsync(int userId)
        {
            try
            {
                // 1. نجيب المعاملات الخاصة باليوزر
                // ونرتبها تنازلي (الأحدث يظهر فوق) باستخدام CreatedAt
                var transactions = await _transactionRepo.GetAsync(
                    filter: t => t.UserId == userId,
                    orderBy: q => q.OrderByDescending(t => t.CreatedAt)
                );

                // 2. نحولها لـ DTO باستخدام المابنج الذكي اللي لسه عاملينه
                var dtos = _mapper.Map<IEnumerable<TransactionHistoryDto>>(transactions);

                // 3. نرجع النتيجة
                return ResponseViewModel<IEnumerable<TransactionHistoryDto>>.Success(dtos);
            }
            catch (Exception ex)
            {
                return ResponseViewModel<IEnumerable<TransactionHistoryDto>>.Fail($"Error fetching transactions: {ex.Message}");
            }
        }
    }
}