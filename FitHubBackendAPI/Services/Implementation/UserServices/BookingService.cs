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
        private readonly IGenericRepository<Subscription> _subscriptionRepo;
        private readonly IMapper _mapper;

        public BookingService(
            IGenericRepository<Booking> bookingRepo,
            IGenericRepository<GymBranch> branchRepo,
            IGenericRepository<Subscription> subscriptionRepo,
            IMapper mapper)
        {
            _bookingRepo = bookingRepo;
            _branchRepo = branchRepo;
            _subscriptionRepo = subscriptionRepo;
            _mapper = mapper;
        }

        // ================= Create Booking =================
        public async Task<ResponseViewModel<string>> CreateBookingAsync(int userId, CreateBookingDto dto)
        {
            // 1️⃣ Validate Branch
            var branch = await _branchRepo.GetByIdAsync(dto.BranchId);
            if (branch == null || branch.Status != BranchStatus.ACTIVE)
                return ResponseViewModel<string>.Fail("Branch not available");
            bool CanRegularVisit = dto.SubscriptionId == null || dto.SubscriptionId <= 0;

            decimal creditsCost = branch.VisitCreditsCost;
            var dateNow = DateTime.UtcNow;
            Subscription? subscription = null;

            // 2️⃣ Subscription booking
            if (dto.SubscriptionId != null|| dto.SubscriptionId>0)
            {
                subscription = await _subscriptionRepo.GetByIdAsync(dto.SubscriptionId.Value);

                if (subscription == null || subscription.UserId != userId)
                    return ResponseViewModel<string>.Fail("Invalid subscription");

                if (subscription.Status != SubscriptionStatus.ACTIVE)
                    return ResponseViewModel<string>.Fail("Subscription expired");

                if (subscription.BranchId != dto.BranchId)
                    return ResponseViewModel<string>.Fail("Subscription not valid for this branch");

                if (subscription.VisitsUsed >= subscription.VisitsAllowed)
                { 
                    subscription.Status = SubscriptionStatus.EXPIRED;
                    _subscriptionRepo.Update(subscription);
                    await _subscriptionRepo.SaveChangesAsync();
                    return ResponseViewModel<string>.Fail("No remaining visits");
                }
                if (subscription.EndDate < dateNow)
                {
                    subscription.Status = SubscriptionStatus.EXPIRED;
                    _subscriptionRepo.Update(subscription);
                    await _subscriptionRepo.SaveChangesAsync();
                    return ResponseViewModel<string>.Fail("Subscription has expired");
                }


                creditsCost = 0; // ❗ لا خصم هنا
            }
            

            // 3️⃣ Create booking
            var booking = new Booking
            {
                UserId = userId,
                BranchId = dto.BranchId,
                SubscriptionId = subscription?.Id,
                ScheduledDateTime = dto.ScheduledDateTime,
                CreditsCost = creditsCost,
                BookingCode = $"BK-{Guid.NewGuid().ToString()[..8].ToUpper()}",
                Status = BookingStatus.CONFIRMED
            };

            await _bookingRepo.AddAsync(booking);
            await _bookingRepo.SaveChangesAsync();

            return ResponseViewModel<string>.Success(
                booking.BookingCode,
                "Booking confirmed successfully"
            );
        }


        // ================= My Bookings =================
        public async Task<ResponseViewModel<IEnumerable<BookingHistoryDto>>> GetMyBookingsAsync(int userId)
        {
            var bookings = await _bookingRepo.GetAsync(
                b => b.UserId == userId,
                includeProperties: "Branch,Review",
                orderBy: q => q.OrderByDescending(x => x.ScheduledDateTime)
            );

            var result = _mapper.Map<IEnumerable<BookingHistoryDto>>(bookings);
            return ResponseViewModel<IEnumerable<BookingHistoryDto>>.Success(result);
        }
        //================= Get Booking by Branch Id =================
        public async Task<ResponseViewModel<IEnumerable<BookingHistoryDto>>> GetBookingsByBranchIdAsync(int branchId)
        {
            var bookings = await _bookingRepo.GetAsync(
                b => b.BranchId == branchId,
                includeProperties: "User,Branch",
                orderBy: q => q.OrderByDescending(x => x.ScheduledDateTime)
            );

            var result = _mapper.Map<IEnumerable<BookingHistoryDto>>(bookings);
            return ResponseViewModel<IEnumerable<BookingHistoryDto>>.Success(result);
        }

        // ================= Booking Details =================
        public async Task<ResponseViewModel<BookingDetailsDto>> GetBookingDetailsAsync(int userId, int bookingId)
        {
            var bookings = await _bookingRepo.GetAsync(
                b => b.Id == bookingId,
                includeProperties: "Branch"
            );

            var booking = bookings.FirstOrDefault();

            

            if (booking.Branch == null)
                return ResponseViewModel<BookingDetailsDto>.Fail("Booking data is incomplete");

            var dto = _mapper.Map<BookingDetailsDto>(booking);
            return ResponseViewModel<BookingDetailsDto>.Success(dto);
        }

        // ================= Cancel Booking =================
        public async Task<ResponseViewModel<bool>> CancelBookingAsync(int userId, int bookingId)
        {
            var booking = await _bookingRepo.GetByIdAsync(bookingId);

            if (booking == null || booking.UserId != userId)
                return ResponseViewModel<bool>.Fail("Booking not found");

            if (booking.Status != BookingStatus.CONFIRMED)
                return ResponseViewModel<bool>.Fail("Cannot cancel this booking");

            if (booking.ScheduledDateTime <= DateTime.UtcNow)
                return ResponseViewModel<bool>.Fail("Cannot cancel after visit time");

            booking.Status = BookingStatus.CANCELLED;
            booking.UpdatedAt = DateTime.UtcNow;

            _bookingRepo.Update(booking);
            await _bookingRepo.SaveChangesAsync();

            // Refund هيحصل لاحقًا في Wallet Module
            return ResponseViewModel<bool>.Success(true, "Booking cancelled successfully");
        }

        // ================= Helper =================
        private string GenerateBookingCode()
        {
            return $"BK-{Guid.NewGuid().ToString()[..8].ToUpper()}";
        }
    }
}
