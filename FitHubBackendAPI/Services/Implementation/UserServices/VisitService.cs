using FitHubBackendAPI.DTOs.VisitDTOs;
using FitHubBackendAPI.Entities.Enums;
using FitHubBackendAPI.Entities.Models;
using FitHubBackendAPI.Repository.Interfaces;
using FitHubBackendAPI.Services.Interfaces.UserServices;
using FitHubBackendAPI.ViewModels;

namespace FitHubBackendAPI.Services.Implementation.UserServices
{
    
        public class VisitService : IVisitService
        {
            private readonly IGenericRepository<Visit> _visitRepo;
            private readonly IGenericRepository<Booking> _bookingRepo;
            private readonly IGenericRepository<UserWallet> _userWalletRepo;
            private readonly IGenericRepository<OwnerWallet> _ownerWalletRepo;
            private readonly IGenericRepository<UserCreditTransactions> _transactionRepo;
            private readonly IGenericRepository<GymStaff> _staffRepo;
            private readonly IGenericRepository<GymBranch> _branchRepo;
            private readonly IGenericRepository<Subscription> _subscriptionRepo;

            public VisitService(
                IGenericRepository<Visit> visitRepo,
                IGenericRepository<Booking> bookingRepo,
                IGenericRepository<UserWallet> userWalletRepo,
                IGenericRepository<OwnerWallet> ownerWalletRepo,
                IGenericRepository<UserCreditTransactions> transactionRepo,
                IGenericRepository<GymStaff> staffRepo,
                IGenericRepository<GymBranch> branchRepo,
                IGenericRepository<Subscription> subscriptionRepo)
            {
                _visitRepo = visitRepo;
                _bookingRepo = bookingRepo;
                _userWalletRepo = userWalletRepo;
                _ownerWalletRepo = ownerWalletRepo;
                _transactionRepo = transactionRepo;
                _staffRepo = staffRepo;
                _branchRepo = branchRepo;
                _subscriptionRepo = subscriptionRepo;
            }

        // ======================================================
        // 1️⃣ CHECK-IN (Staff)
        // ======================================================
        public async Task<ResponseViewModel<bool>> CheckInAsync(int staffUserId, CheckInVisitDto dto)
        {
            // 1️⃣ Validate Staff
            var staff = (await _staffRepo.FindAsync(s => s.UserId == staffUserId))
                .FirstOrDefault();

            if (staff == null || staff.Status != StaffStatus.ACTIVE)
                return ResponseViewModel<bool>.Fail("Unauthorized staff");

            // 2️⃣ Get Booking
            var booking = (await _bookingRepo.GetAsync(
                b => b.BookingCode == dto.BookingCode,
                includeProperties: "Subscription"
            )).FirstOrDefault();

            if (booking == null)
                return ResponseViewModel<bool>.Fail("Invalid booking code");

            if (booking.Status != BookingStatus.CONFIRMED)
                return ResponseViewModel<bool>.Fail("Booking already processed");

            // 3️⃣ Validate Staff Branch
            if (staff.BranchId != booking.BranchId)
                return ResponseViewModel<bool>.Fail("Staff not assigned to this branch");

            // 4️⃣ No Show
            //if (booking.ScheduledDateTime < DateTime.UtcNow)
            //{
            //    booking.Status = BookingStatus.NOSHOW;
            //    _bookingRepo.Update(booking);
            //    await _bookingRepo.SaveChangesAsync();

            //    return ResponseViewModel<bool>.Fail("Booking expired (No Show)");
            //}

            decimal deductedCredits = 0;

            // ================= Subscription =================
            if (booking.SubscriptionId.HasValue)
            {
                var sub = await _subscriptionRepo.GetByIdAsync(booking.SubscriptionId.Value);

                if (sub == null || sub.Status != SubscriptionStatus.ACTIVE)
                    return ResponseViewModel<bool>.Fail("Subscription invalid");

                if (sub.VisitsUsed >= sub.VisitsAllowed)
                    return ResponseViewModel<bool>.Fail("No remaining visits in subscription");
                if(sub.VisitsAllowed-sub.VisitsUsed ==1)
                    sub.Status = SubscriptionStatus.EXPIRED;

                sub.VisitsUsed += 1;
                _subscriptionRepo.Update(sub);
            }
            // ================= Pay As You Go =================
            else
            {
                var wallet = (await _userWalletRepo
                    .FindAsync(w => w.UserId == booking.UserId))
                    .FirstOrDefault();

                if (wallet == null || (wallet.Balance ?? 0) < (booking.CreditsCost ?? 0))
                    return ResponseViewModel<bool>.Fail("Insufficient balance");

                var beforeBalance = wallet.Balance ?? 0;

                wallet.Balance = (wallet.Balance ?? 0) - booking.CreditsCost!.Value;
                wallet.LastUpdated = DateTime.UtcNow;
                _userWalletRepo.Update(wallet);

                deductedCredits = booking.CreditsCost.Value;

                await _transactionRepo.AddAsync(new UserCreditTransactions
                {
                    UserId = booking.UserId,
                    CreditsBefore = beforeBalance,
                    CreditsChanged = -deductedCredits,
                    CreditsAfter = wallet.Balance,
                    TransactionType = TransactionType.DEDUCT,
                    Source = TransactionSource.VISIT,
                    Description = "Gym Visit",
                    ReferenceId = booking.Id
                });
            }

            // 5️⃣ Owner Wallet
            var branch = await _branchRepo.GetByIdAsync(booking.BranchId!.Value);

            if (branch == null)
                return ResponseViewModel<bool>.Fail("Branch not found");

            var ownerWallet = (await _ownerWalletRepo
                .FindAsync(w => w.OwnerId == branch.OwnerId))
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

            ownerWallet.Balance += deductedCredits;
            ownerWallet.LastUpdated = DateTime.UtcNow;
            _ownerWalletRepo.Update(ownerWallet);

            // 6️⃣ Create Visit
            await _visitRepo.AddAsync(new Visit
            {
                BookingId = booking.Id,
                UserId = booking.UserId,
                BranchId = booking.BranchId,
                StaffId = staff.Id,
                CheckInTime = DateTime.UtcNow,
                CreditsDeducted = deductedCredits,
                Status = VisitStatus.CHECKED_IN
            });

            // 7️⃣ Complete Booking
            booking.Status = BookingStatus.COMPLETED;
            _bookingRepo.Update(booking);

            await _bookingRepo.SaveChangesAsync();

            return ResponseViewModel<bool>.Success(true, "Check-in completed successfully");
        }

        // ======================================================
        // 2️⃣ USER VISIT HISTORY
        // ======================================================
        public async Task<ResponseViewModel<IEnumerable<VisitHistoryDto>>> GetMyVisitsAsync(int userId)
            {
                var visits = await _visitRepo.GetAsync(
                    v => v.UserId == userId,
                    includeProperties: "Branch,User",
                    orderBy: q => q.OrderByDescending(v => v.CheckInTime)
                );

                var result = visits.Select(v => new VisitHistoryDto
                {
                    VisitId = v.Id,
                    BranchName = v.Branch!.BranchName!,
                    CheckInTime = v.CheckInTime!.Value,
                    CreditsDeducted = v.CreditsDeducted ?? 0
                });

                return ResponseViewModel<IEnumerable<VisitHistoryDto>>.Success(result);
            }

            // ======================================================
            // 3️⃣ BRANCH VISITS (Staff / Owner)
            // ======================================================
            public async Task<ResponseViewModel<IEnumerable<VisitHistoryDto>>> GetBranchVisitsAsync( int requesterUserId)
            {
                var staff = (await _staffRepo.FindAsync(s => s.UserId == requesterUserId)).FirstOrDefault();
                if (staff == null)
                    return ResponseViewModel<IEnumerable<VisitHistoryDto>>.Fail("Staff not found");
                var branchId = staff.BranchId;
            var visits = await _visitRepo.GetAsync(
                    v => v.BranchId == branchId,
                    includeProperties: "Branch,User",
                    orderBy: q => q.OrderByDescending(v => v.CheckInTime)
                );

                var result = visits.Select(v => new VisitHistoryDto
                {
                    VisitId = v.Id,
                    BranchName = v.Branch!.BranchName!,
                    CheckInTime = v.CheckInTime!.Value,
                    CreditsDeducted = v.CreditsDeducted ?? 0,
                    
                });

                return ResponseViewModel<IEnumerable<VisitHistoryDto>>.Success(result);
            }
        }
    }
