using AutoMapper;
using FitHubBackendAPI.DTOs.Bookings;
using FitHubBackendAPI.Entities.Enums;
using FitHubBackendAPI.Entities.Models;
using FitHubBackendAPI.Repository.Interfaces;
using FitHubBackendAPI.Services.Interfaces.UserServices;
using FitHubBackendAPI.ViewModels;

namespace FitHubBackendAPI.Services.Implementation.UserServices
{
    public class BookingService : IBookingService
    {
        private readonly IGenericRepository<Booking> _bookingRepo;
        private readonly IGenericRepository<GymBranch> _branchRepo;
        private readonly IGenericRepository<Subscription> _subRepo;
        private readonly IGenericRepository<UserWallet> _walletRepo;
        private readonly IGenericRepository<UserCreditTransactions> _transactionRepo;
        private readonly IMapper _mapper;

        public BookingService(
            IGenericRepository<Booking> bookingRepo,
            IGenericRepository<GymBranch> branchRepo,
            IGenericRepository<Subscription> subRepo,
            IGenericRepository<UserWallet> walletRepo,
            IGenericRepository<UserCreditTransactions> transactionRepo,
            IMapper mapper)
        {
            _bookingRepo = bookingRepo;
            _branchRepo = branchRepo;
            _subRepo = subRepo;
            _walletRepo = walletRepo;
            _transactionRepo = transactionRepo;
            _mapper = mapper;
        }

        // ==========================================================
        // 1. دالة إنشاء حجز جديد (Create Booking)
        // ==========================================================
        public async Task<ResponseViewModel<string>> CreateBookingAsync(int userId, CreateBookingDto dto)
        {
            // 1️⃣ التأكد إن الفرع موجود
            var branch = await _branchRepo.GetByIdAsync(dto.BranchId);
            if (branch == null)
                return ResponseViewModel<string>.Fail("Gym Branch not found");

            int finalCost;
            Subscription? subscription = null;

            // 2️⃣ تحديد نوع الحجز
            if (dto.SubscriptionId.HasValue)
            {
                // ============================================================
                // الحالة الأولى: الحجز باشتراك
                // ============================================================
                subscription = await _subRepo.GetByIdAsync(dto.SubscriptionId.Value);

                if (subscription == null) return ResponseViewModel<string>.Fail("Subscription not found");
                if (subscription.UserId != userId) return ResponseViewModel<string>.Fail("This subscription does not belong to you");
                if (subscription.Status != SubscriptionStatus.ACTIVE) return ResponseViewModel<string>.Fail("Subscription is not active");

                // نتأكد إن لسه فاضل زيارات (Validation فقط)
                if (subscription.VisitsUsed >= subscription.VisitsAllowed) return ResponseViewModel<string>.Fail("No visits remaining");

                if (subscription.EndDate < DateTime.UtcNow) return ResponseViewModel<string>.Fail("Subscription expired");
                if (subscription.BranchId != dto.BranchId) return ResponseViewModel<string>.Fail("Subscription not valid for this branch");

                finalCost = 0;
                // ⚠️ هام: هنا مخصمناش الزيارة (VisitsUsed مش هتزيد دلوقتي)
                // الخصم هيحصل لما يروح يعمل Check-in
            }
            else
            {
                // ============================================================
                // الحالة الثانية: زيارة طايرة (Pay As You Go)
                // ============================================================
                finalCost = branch.VisitCreditsCost;
            }

            // 3️⃣ التحقق من رصيد المحفظة (لو فيه تكلفة)
            if (finalCost > 0)
            {
                var wallet = (await _walletRepo.FindAsync(w => w.UserId == userId)).FirstOrDefault();

                // بنتأكد بس إنه "معاه فلوس" عشان ميعطلش الدنيا، لكن مش بنخصم
                if (wallet == null || wallet.Balance < finalCost)
                {
                    return ResponseViewModel<string>.Fail($"Insufficient wallet balance. You need {finalCost} credits.");
                }
                // ⚠️ هام: هنا مخصمناش رصيد من المحفظة
                // الخصم هيحصل لما يروح يعمل Check-in
            }

            // 4️⃣ إنشاء الحجز
            var booking = _mapper.Map<Booking>(dto);
            booking.UserId = userId;
            booking.CreditsCost = finalCost; // بنسجل التكلفة عشان السيستم يعرف يخصم كام وقت الـ Check-in
            booking.BookingCode = GenerateBookingCode();
            booking.Status = BookingStatus.CONFIRMED;
            booking.IsAcTive = true;

            if (subscription != null)
                booking.SubscriptionId = subscription.Id;

            await _bookingRepo.AddAsync(booking);
            await _bookingRepo.SaveChangesAsync();

            return ResponseViewModel<string>.Success(
                booking.BookingCode,
                "Booking confirmed. Verification & Deduction will happen upon Check-in."
            );
        }

        // ==========================================================
        // 2. دالة عرض حجوزات اليوزر (Get My Bookings)
        // ==========================================================
        public async Task<ResponseViewModel<IEnumerable<BookingHistoryDto>>> GetUserBookingsAsync(int userId)
        {
            try
            {
                var bookings = await _bookingRepo.GetAsync(
                    filter: b => b.UserId == userId,
                    includeProperties: "Branch,Review",
                    orderBy: q => q.OrderByDescending(b => b.ScheduledDateTime)
                );

                var bookingDtos = _mapper.Map<IEnumerable<BookingHistoryDto>>(bookings);
                return ResponseViewModel<IEnumerable<BookingHistoryDto>>.Success(bookingDtos);
            }
            catch (Exception ex)
            {
                return ResponseViewModel<IEnumerable<BookingHistoryDto>>.Fail($"Error fetching bookings: {ex.Message}");
            }
        }

        // ==========================================
        // Helper Method
        // ==========================================
        private string GenerateBookingCode()
        {
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            Random random = new Random();
            string randomPart = new string(Enumerable.Repeat(chars, 6)
                .Select(s => s[random.Next(s.Length)]).ToArray());

            return $"BKNG-{randomPart}";
        }

        // ==========================================================
        // 3. دالة تفاصيل الحجز (Get Booking Details)
        // ==========================================================
        public async Task<ResponseViewModel<BookingDetailsDto>> GetBookingDetailsAsync(int userId, int bookingId)
        {
            try
            {
                var bookings = await _bookingRepo.GetAsync(
                    filter: b => b.Id == bookingId,
                    includeProperties: "Branch"
                );

                var booking = bookings.FirstOrDefault();

                if (booking == null) return ResponseViewModel<BookingDetailsDto>.Fail("Booking not found");
                if (booking.UserId != userId) return ResponseViewModel<BookingDetailsDto>.Fail("You are not authorized to view this booking");

                var dto = _mapper.Map<BookingDetailsDto>(booking);
                return ResponseViewModel<BookingDetailsDto>.Success(dto);
            }
            catch (Exception ex)
            {
                return ResponseViewModel<BookingDetailsDto>.Fail($"Error fetching details: {ex.Message}");
            }
        }

        // ==========================================================
        // 4. دالة إلغاء الحجز (Cancel Booking)
        // ==========================================================
        public async Task<ResponseViewModel<bool>> CancelBookingAsync(int userId, int bookingId)
        {
            try
            {
                var booking = await _bookingRepo.GetByIdAsync(bookingId);

                if (booking == null) return ResponseViewModel<bool>.Fail("Booking not found");
                if (booking.UserId != userId) return ResponseViewModel<bool>.Fail("Not authorized to cancel this booking");

                if (booking.Status != BookingStatus.CONFIRMED)
                    return ResponseViewModel<bool>.Fail("Cannot cancel this booking because it is already completed or cancelled");

                booking.Status = BookingStatus.CANCELLED;

                // بما إننا مخصمناش حاجة في الحجز، فمفيش حاجة نرجعها (Refund) هنا.

                _bookingRepo.Update(booking);
                await _bookingRepo.SaveChangesAsync();

                return ResponseViewModel<bool>.Success(true, "Booking cancelled successfully");
            }
            catch (Exception ex)
            {
                return ResponseViewModel<bool>.Fail($"Error cancelling: {ex.Message}");
            }
        }
    }
}