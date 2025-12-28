using FitHubBackendAPI.DTOs.Reviews;
using FitHubBackendAPI.Entities.Enums;
using FitHubBackendAPI.Entities.Models;
using FitHubBackendAPI.Repository.Interfaces;
using FitHubBackendAPI.Services.Interfaces.UserServices;
using FitHubBackendAPI.ViewModels;

namespace FitHubBackendAPI.Services.Implementation.UserServices
{
    public class ReviewService : IReviewService
    {
        private readonly IGenericRepository<Review> _reviewRepo;
        private readonly IGenericRepository<Booking> _bookingRepo;

        public ReviewService(
            IGenericRepository<Review> reviewRepo,
            IGenericRepository<Booking> bookingRepo)
        {
            _reviewRepo = reviewRepo;
            _bookingRepo = bookingRepo;
        }

        public async Task<IEnumerable<GetAllBranchRevewsDto>> GetAllBrancheReviewsAsync(int branchId)
        {
            var reviews = await _reviewRepo.GetAsync(r=>r.BranchId == branchId,
                includeProperties:"Users,Booking");
            var dto = reviews
                .Where(r => r.User != null && r.Booking != null)
                .Select(r => new GetAllBranchRevewsDto
                {
                    Id = r.Id,
                    Rating=r.Rating,
                    UserName = r.User!.FullName,
                    BookingDate = r.Booking!.ScheduledDateTime,
                    Comment = r.Comment
                });
            return dto;
        }

        public async Task<ResponseViewModel<bool>> LeaveReviewAsync(int userId, LeaveReviewDto dto)
        {
            try
            {
                // 1. نجيب الحجز عشان نتأكد من بياناته
                // بنعمل Include للريفيو عشان نتأكد انه مقيمش قبل كدة
                // وبنحتاج الـ BranchId اللي جوه الحجز عشان نربط الريفيو بالفرع
                var bookings = await _bookingRepo.GetAsync(
                    filter: b => b.Id == dto.BookingId,
                    includeProperties: "Review"
                );

                var booking = bookings.FirstOrDefault();

                // 2. التحققات (Validations)
                if (booking == null)
                    return ResponseViewModel<bool>.Fail("Booking not found");

                // هل الحجز ده بتاع اليوزر ده؟
                if (booking.UserId != userId)
                    return ResponseViewModel<bool>.Fail("You can only review your own bookings");

                // هل الزيارة تمت بالفعل؟ (حسب الـ Enum اللي بعتهولي)
                if (booking.Status != BookingStatus.COMPLETED)
                    return ResponseViewModel<bool>.Fail("You can only review completed visits");

                // هل عمل ريفيو للحجز ده قبل كدة؟ (عشان المنع)
                if (booking.Review != null)
                    return ResponseViewModel<bool>.Fail("You have already reviewed this booking");

                // التحقق من وجود BranchId
                if (!booking.BranchId.HasValue)
                    return ResponseViewModel<bool>.Fail("Booking data is incomplete");

                // 3. إنشاء الريفيو
                var review = new Review
                {
                    UserId = userId,
                    BookingId = booking.Id,

                    // ✅ نقطة مهمة: بنربط الريفيو بالفرع أوتوماتيك من بيانات الحجز
                    BranchId = booking.BranchId.Value,

                    Rating = dto.Rating,
                    Comment = dto.Comment,
                    IsAnonymous = dto.IsAnonymous,

                    IsAcTive = true,
                    CreatedAt = DateTime.UtcNow
                };

                await _reviewRepo.AddAsync(review);
                await _reviewRepo.SaveChangesAsync();

                return ResponseViewModel<bool>.Success(true, "Review submitted successfully");
            }
            catch (Exception ex)
            {
                return ResponseViewModel<bool>.Fail($"Error submitting review: {ex.Message}");
            }
        }
        public async Task<bool> DeleteReview(int ReviewId)
        {

            return true;
        }
    }
}