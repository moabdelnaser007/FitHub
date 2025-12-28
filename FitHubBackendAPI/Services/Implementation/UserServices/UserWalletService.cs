using AutoMapper;
using FitHubBackendAPI.DTOs.Wallet;
using FitHubBackendAPI.Entities;
using FitHubBackendAPI.Entities.Enums;
using FitHubBackendAPI.Entities.Models;
using FitHubBackendAPI.Repository.Interfaces;
using FitHubBackendAPI.Services.Interfaces.PaymobService;
using FitHubBackendAPI.Services.Interfaces.UserServices;
using FitHubBackendAPI.ViewModels;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace FitHubBackendAPI.Services.Implementation.UserServices
{
    public class UserWalletService : IUserWalletService
    {
        private readonly IGenericRepository<UserWallet> _walletRepo;
        private readonly IGenericRepository<User> _UserRepostory;

        private readonly IGenericRepository<UserCreditTransactions> _transactionRepo;
        private readonly IGenericRepository<FithubPlan> _planRepo;
        private readonly IGenericRepository<FithubUserPlan> _userPlanRepo;
        private readonly IGenericRepository<Booking> _bookingRepo;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;

        public UserWalletService(
            IConfiguration config,
            IGenericRepository<User> UserRepostory,
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
            _UserRepostory = UserRepostory;
            _config = config;
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
        public async Task<ResponseViewModel<UserCreditTransactions>> RechargeAsync(int userId, RechargeWalletDto dto)
        {
            var plan = await _planRepo.GetByIdAsync(dto.PlanId);
            if (plan == null)
                return ResponseViewModel<UserCreditTransactions>.Fail("Plan not found");

            var wallet = (await _walletRepo.FindAsync(w => w.UserId == userId)).FirstOrDefault();
            //check if wallet exists
            //create if not
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
            }
            //create user plan entry
            //set isActive to false until payment confirmation

            await _userPlanRepo.AddAsync(new FithubUserPlan
            {
                UserId = userId,
                PlanId = plan.Id,

                BasePrice = plan.Price,
                TaxAmount = plan.Price * 0.15m,
                TotalAmount = plan.Price * 1.15m,

                IsAcTive = false,
                IsDeleted = false,
                CreatedAt = DateTime.UtcNow
            });
            //create transaction entry
            //set isPaid to false until payment confirmation
            //set credits before and after accordingly
            //set Status to PENDING until payment confirmation
            UserCreditTransactions transaction = new UserCreditTransactions
            {
                UserId = userId,
                CreditsChanged = plan.CreditsValue,
                CreditsBefore = wallet.Balance,
                CreditsAfter = wallet.Balance + plan.CreditsValue,
                TransactionType = TransactionType.RECHARGE,
                Source = TransactionSource.PAYMOB,
                Description = $"Recharge plan {plan.Name}",
                PaymentAmount = plan.Price * 1.15m,
                IsPaid = false,
                CreatedAt = DateTime.UtcNow
            };

            await _transactionRepo.AddAsync(transaction);
            await _walletRepo.SaveChangesAsync();

            return ResponseViewModel<UserCreditTransactions>.Success(transaction, "Charging transaction created successfully, Waiting for payment confirmation.");
        }

        public async Task<ResponseViewModel<UserCreditTransactions>> UpdateWallet(UserCreditTransactions transaction)
        {
            //get wallet
            //validate  wallet
            //if successful payment update wallet balance
            var wallet = (await _walletRepo.FindAsync(w => w.UserId == transaction.UserId)).FirstOrDefault();
            if (wallet == null)
                return ResponseViewModel<UserCreditTransactions>.Fail("Wallet not found");
            if (transaction.IsPaid)
            {
                wallet.Balance = transaction.CreditsAfter;
                wallet.LastUpdated = DateTime.UtcNow;
                _walletRepo.Update(wallet);
                await _walletRepo.SaveChangesAsync();
            }
            return ResponseViewModel<UserCreditTransactions>.Success(transaction, "Wallet updated successfully");
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