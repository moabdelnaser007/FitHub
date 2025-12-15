using AutoMapper;
using FitHubBackendAPI.DTOs.Bookings;
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
        private readonly IMapper _mapper;

        public BookingService(
            IGenericRepository<Booking> bookingRepo,
            IGenericRepository<GymBranch> branchRepo,
            IGenericRepository<Subscription> subRepo,
            IMapper mapper)
        {
            _bookingRepo = bookingRepo;
            _branchRepo = branchRepo;
            _subRepo = subRepo;
            _mapper = mapper;
        }

        // ==========================================================
        // 1. دالة إنشاء حجز جديد (Create Booking)
        // ==========================================================
        public async Task<ResponseViewModel<string>> CreateBookingAsync(int userId, CreateBookingDto dto)
        {
            try
            {
                // أ) نتأكد إن الفرع موجود
                var branch = await _branchRepo.GetByIdAsync(dto.BranchId);
                if (branch == null)
                    return ResponseViewModel<string>.Fail("Gym Branch not found");

                int finalCost = 0;

                // ب) تحديد نوع الحجز (باشتراك ولا زياره طايره؟)
                if (dto.SubscriptionId.HasValue)
                {
                    // --- حالة الحجز باشتراك ---
                    var sub = await _subRepo.GetByIdAsync(dto.SubscriptionId.Value);

                    // تحققات الاشتراك
                    if (sub == null) return ResponseViewModel<string>.Fail("Subscription not found");
                    if (sub.UserId != userId) return ResponseViewModel<string>.Fail("This subscription does not belong to you");
                    if (sub.Status != FitHubBackendAPI.Entities.Enums.SubscriptionStatus.ACTIVE) return ResponseViewModel<string>.Fail("Subscription is not active");
                    if (sub.VisitsUsed >= sub.VisitsAllowed) return ResponseViewModel<string>.Fail("No visits remaining in this subscription");
                    if (sub.EndDate < DateTime.UtcNow) return ResponseViewModel<string>.Fail("Subscription expired");

                    // هل الاشتراك ده يخص الفرع ده؟
                    if (sub.BranchId != dto.BranchId) return ResponseViewModel<string>.Fail("This subscription is not for this branch");

                    finalCost = 0; // الحجز مجاني، الخصم هيحصل من الزيارات وقت الـ Check-in
                }
                else
                {
                    // --- حالة الحجز الفردي (Pay As You Go) ---
                    // التكلفة = سعر الزيارة المتسجل في الفرع
                    finalCost = branch.VisitCreditsCost;
                }

                // ج) تحويل الـ DTO لـ Entity
                var booking = _mapper.Map<Booking>(dto);

                // د) ملء البيانات الناقصة يدويًا
                booking.UserId = userId;
                booking.CreditsCost = finalCost;
                booking.BookingCode = GenerateBookingCode();
                booking.IsAcTive = true;

                // هـ) الحفظ في الداتا بيز
                await _bookingRepo.AddAsync(booking);
                await _bookingRepo.SaveChangesAsync();

                return ResponseViewModel<string>.Success(booking.BookingCode, "Booking created successfully");
            }
            catch (Exception ex)
            {
                return ResponseViewModel<string>.Fail($"Error creating booking: {ex.Message}");
            }
        }

        // ==========================================================
        // 2. دالة عرض حجوزات اليوزر (Get My Bookings) - 
        // ==========================================================
        public async Task<ResponseViewModel<IEnumerable<BookingHistoryDto>>> GetUserBookingsAsync(int userId)
        {
            try
            {
                // بنجيب الحجوزات الخاصة باليوزر
                // includeProperties: بنجيب بيانات الفرع (عشان الاسم) والريفيو (عشان نعرف قيم ولا لأ)
                var bookings = await _bookingRepo.GetAsync(
                    filter: b => b.UserId == userId,
                    includeProperties: "Branch,Review",
                    orderBy: q => q.OrderByDescending(b => b.ScheduledDateTime) // الأحدث الأول
                );

                // بنحولها للشكل اللي الفرونت محتاجه
                var bookingDtos = _mapper.Map<IEnumerable<BookingHistoryDto>>(bookings);

                return ResponseViewModel<IEnumerable<BookingHistoryDto>>.Success(bookingDtos);
            }
            catch (Exception ex)
            {
                return ResponseViewModel<IEnumerable<BookingHistoryDto>>.Fail($"Error fetching bookings: {ex.Message}");
            }
        }

        // ==========================================
        // دالة مساعدة لتوليد كود عشوائي (Helper Method)
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
                // بنستخدم دالة GetAsync الجوكر عشان نجيب بيانات الفرع (Branch)
                // عشان محتاجين الاسم والعنوان من جوا الفرع
                var bookings = await _bookingRepo.GetAsync(
                    filter: b => b.Id == bookingId,
                    includeProperties: "Branch"
                );

                var booking = bookings.FirstOrDefault();

                // تحققات
                if (booking == null)
                    return ResponseViewModel<BookingDetailsDto>.Fail("Booking not found");

                // لازم نتأكد إن الحجز ده بتاع اليوزر اللي باعت الطلب
                if (booking.UserId != userId)
                    return ResponseViewModel<BookingDetailsDto>.Fail("You are not authorized to view this booking");

                // التحويل باستخدام المابنج اللي ظبطناه
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

                if (booking == null)
                    return ResponseViewModel<bool>.Fail("Booking not found");

                if (booking.UserId != userId)
                    return ResponseViewModel<bool>.Fail("Not authorized to cancel this booking");

                // نسمح بالإلغاء فقط لو الحالة CONFIRMED (يعني لسه مرحش)
                if (booking.Status != FitHubBackendAPI.Entities.Enums.BookingStatus.CONFIRMED)
                    return ResponseViewModel<bool>.Fail("Cannot cancel this booking because it is already completed or cancelled");

                // تغيير الحالة لـ CANCELLED
                booking.Status = FitHubBackendAPI.Entities.Enums.BookingStatus.CANCELLED;

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