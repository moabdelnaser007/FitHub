using AutoMapper;
using FitHubBackendAPI.DTOs.Bookings;
using FitHubBackendAPI.Entities.Models;
using FitHubBackendAPI.Repository.Interfaces;
using FitHubBackendAPI.Services.Interfaces.UserServices;
using FitHubBackendAPI.ViewModels;

namespace FitHubBackendAPI.Services.Implementation.UserServices
{
    public class BookingService: IBookingService
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

        public async Task<ResponseViewModel<string>> CreateBookingAsync(int userId, CreateBookingDto dto)
        {
            try
            {
                // 1. نتأكد إن الفرع موجود
                var branch = await _branchRepo.GetByIdAsync(dto.BranchId);
                if (branch == null)
                    return ResponseViewModel<string>.Fail("Gym Branch not found");

                int finalCost = 0; // هنحسبها دلوقتي

                // 2. تحديد نوع الحجز (باشتراك ولا فردي؟)
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
                    // التكلفة = سعر الزيارة المتسجل في الفرع (اللي ضفناه في الداتا بيز)
                    finalCost = branch.VisitCreditsCost;
                }

                // 3. تحويل الـ DTO لـ Entity
                var booking = _mapper.Map<Booking>(dto);

                // 4. ملء البيانات الناقصة يدويًا
                booking.UserId = userId;
                booking.CreditsCost = finalCost; // السعر اللي حسبناه
                booking.BookingCode = GenerateBookingCode(); // بنولد الكود هنا
                booking.IsAcTive = true;

                // 5. الحفظ في الداتا بيز
                await _bookingRepo.AddAsync(booking);
                await _bookingRepo.SaveChangesAsync();

                // نرجع الكود لليوزر عشان يظهره في الشاشة
                return ResponseViewModel<string>.Success(booking.BookingCode, "Booking created successfully");
            }
            catch (Exception ex)
            {
                return ResponseViewModel<string>.Fail($"Error creating booking: {ex.Message}");
            }
        }

        // ==========================================
        // دالة مساعدة لتوليد كود عشوائي (Helper Method)
        // ==========================================
        private string GenerateBookingCode()
        {
            // بنعمل كود عشوائي زي: BKNG-A1B2C3
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            Random random = new Random();
            string randomPart = new string(Enumerable.Repeat(chars, 6)
                .Select(s => s[random.Next(s.Length)]).ToArray());

            return $"BKNG-{randomPart}";
        }
    }
}
