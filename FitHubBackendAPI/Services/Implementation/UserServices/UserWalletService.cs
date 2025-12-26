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
        private readonly IGenericRepository<FithubPlan> _planRepo;
        private readonly IGenericRepository<FithubUserPlan> _userPlanRepo;
        private readonly IGenericRepository<Booking> _bookingRepo;
        private readonly IMapper _mapper;

        public UserWalletService(
            IGenericRepository<UserWallet> walletRepo,
            IGenericRepository<UserCreditTransactions> transactionRepo,
            IGenericRepository<FithubPlan> planRepo,
            IGenericRepository<FithubUserPlan> userPlanRepo,
            IGenericRepository<Booking> bookingRepo,
            IMapper mapper)
        {
            _walletRepo = walletRepo;
            _transactionRepo = transactionRepo;
            _planRepo = planRepo;
            _userPlanRepo = userPlanRepo;
            _bookingRepo = bookingRepo;
            _mapper = mapper;
        }

        // ================= Balance =================
        public async Task<ResponseViewModel<WalletBalanceDto>> GetBalanceAsync(int userId)
        {
            var wallet = (await _walletRepo.FindAsync(w => w.UserId == userId)).FirstOrDefault();

            if (wallet == null)
                return ResponseViewModel<WalletBalanceDto>.Success(
                    new WalletBalanceDto { UserId = userId, Balance = 0 }
                );

            return ResponseViewModel<WalletBalanceDto>.Success(
                _mapper.Map<WalletBalanceDto>(wallet)
            );
        }

        // ================= Transactions =================
        public async Task<ResponseViewModel<List<TransactionHistoryDto>>> GetTransactionsAsync(int userId)
        {
            var transactions = await _transactionRepo.GetAsync(
                t => t.UserId == userId,
                orderBy: q => q.OrderByDescending(x => x.CreatedAt)
            );

            var dtos = _mapper.Map<List<TransactionHistoryDto>>(transactions);
            return ResponseViewModel<List<TransactionHistoryDto>>.Success(dtos);
        }

        // ================= Recharge =================
        public async Task<ResponseViewModel<bool>> RechargeAsync(int userId, RechargeWalletDto dto)
        {
            var plan = await _planRepo.GetByIdAsync(dto.PlanId);
            if (plan == null)
                return ResponseViewModel<bool>.Fail("Plan not found");

            var wallet = (await _walletRepo.FindAsync(w => w.UserId == userId)).FirstOrDefault();

            if (wallet == null)
            {
                wallet = new UserWallet
                {
                    UserId = userId,
                    Balance = 0,
                    IsAcTive = true,
                    IsDeleted = false,
                    CreatedAt = DateTime.UtcNow
                };

                await _walletRepo.AddAsync(wallet);
                await _walletRepo.SaveChangesAsync(); // 🔥 مهم
            }

            var before = wallet.Balance ?? 0;

            wallet.Balance = before + plan.CreditsValue;
            wallet.LastUpdated = DateTime.UtcNow;
            _walletRepo.Update(wallet);

            await _userPlanRepo.AddAsync(new FithubUserPlan
            {
                UserId = userId,
                PlanId = plan.Id,

                BasePrice = plan.Price,
                TaxAmount = plan.Price * 0.15m,
                TotalAmount = plan.Price * 1.15m,

                IsAcTive = true,
                IsDeleted = false,
                CreatedAt = DateTime.UtcNow
            });

            await _transactionRepo.AddAsync(new UserCreditTransactions
            {
                UserId = userId,
                CreditsBefore = before,
                CreditsChanged = plan.CreditsValue,
                CreditsAfter = wallet.Balance,
                TransactionType = TransactionType.RECHARGE,
                Source = TransactionSource.MANUAL,
                Description = $"Purchased {plan.Name} plan",
                CreatedAt = DateTime.UtcNow,
                PaymentAmount = plan.Price * 1.15m, // ✅ المبلغ الحقيقي المدفوع

            });

            await _walletRepo.SaveChangesAsync();

            return ResponseViewModel<bool>.Success(true, "Wallet recharged successfully");
        }



        // ================= Refund =================
        public async Task<ResponseViewModel<bool>> RefundBookingAsync(int userId, int bookingId)
        {
            var booking = await _bookingRepo.GetByIdAsync(bookingId);

            if (booking == null || booking.UserId != userId)
                return ResponseViewModel<bool>.Fail("Booking not found");

            if (booking.SubscriptionId.HasValue)
                return ResponseViewModel<bool>.Fail("Subscription bookings are not refundable");

            if (booking.Status != BookingStatus.CONFIRMED)
                return ResponseViewModel<bool>.Fail("Booking not refundable");

            if (booking.ScheduledDateTime <= DateTime.UtcNow)
                return ResponseViewModel<bool>.Fail("Booking already started");

            var wallet = (await _walletRepo.FindAsync(w => w.UserId == userId)).FirstOrDefault();
            if (wallet == null)
                return ResponseViewModel<bool>.Fail("Wallet not found");

            var before = wallet.Balance ?? 0;

            wallet.Balance = (wallet.Balance ?? 0) + (booking.CreditsCost ?? 0);
            wallet.LastUpdated = DateTime.UtcNow;
            _walletRepo.Update(wallet);

            await _transactionRepo.AddAsync(new UserCreditTransactions
            {
                UserId = userId,
                CreditsBefore = before,
                CreditsChanged = booking.CreditsCost ?? 0,
                CreditsAfter = wallet.Balance,
                TransactionType = TransactionType.REFUND,
                Source = TransactionSource.BOOKING,
                Description = "Booking refund",
                ReferenceId = booking.Id
            });

            booking.Status = BookingStatus.CANCELLED;
            _bookingRepo.Update(booking);

            await _walletRepo.SaveChangesAsync();

            return ResponseViewModel<bool>.Success(true, "Booking refunded successfully");
        }
    }
}