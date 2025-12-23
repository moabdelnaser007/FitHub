using AutoMapper;
using FitHubBackendAPI.DTOs.Bookings;
using FitHubBackendAPI.DTOs.Subscriptions;
using FitHubBackendAPI.DTOs.Wallet;
using FitHubBackendAPI.Entities.Enums;
using FitHubBackendAPI.Entities.Models;

public class UserJourneyMappingProfile : Profile
{
    public UserJourneyMappingProfile()
    {
        // ================= Wallet =================
        CreateMap<UserWallet, WalletBalanceDto>()
            .ForMember(d => d.UserId, o => o.MapFrom(s => s.UserId ?? 0))
            .ForMember(d => d.Balance, o => o.MapFrom(s => s.Balance ?? 0));

        // ================= SYSYEMPlans =================
        CreateMap<FithubPlan, FithubPlanDto>();

      
        // ================= Transactions =================
        CreateMap<UserCreditTransactions, TransactionHistoryDto>()
            .ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
            .ForMember(d => d.Date, o => o.MapFrom(s => s.CreatedAt.ToString("MMM dd, yyyy")))
            .ForMember(d => d.AmountPaid, o => o.MapFrom(s => s.PaymentAmount))
            .ForMember(d => d.Credits, o => o.MapFrom(s =>
                s.TransactionType == TransactionType.DEDUCT
                    ? -s.CreditsChanged
                    : s.CreditsChanged))
            .ForMember(d => d.IsPositive, o => o.MapFrom(s =>
                s.TransactionType == TransactionType.RECHARGE ||
                s.TransactionType == TransactionType.REFUND ||
                (s.TransactionType == TransactionType.ADMIN_ADJUST && s.CreditsChanged > 0)))

            // 👇👇👇 التعديل الجوهري هنا 👇👇👇
            .ForMember(d => d.Description, o => o.MapFrom(s =>
                // 1. الأولوية للكلام المتسجل في الداتا بيز (زي Purchased Gold Plan)
                !string.IsNullOrEmpty(s.Description) ? s.Description :

                // 2. لو مفيش، نستخدم اللوجيك القديم كـ Fallback (عشان العمليات القديمة)
                s.TransactionType == TransactionType.RECHARGE ? "Wallet Top-up" :
                s.TransactionType == TransactionType.DEDUCT && s.Source == TransactionSource.BOOKING ? "Class Booking" :
                s.TransactionType == TransactionType.DEDUCT && s.Source == TransactionSource.SUBSCRIPTION ? "Subscription Payment" :
                s.TransactionType == TransactionType.REFUND ? "Refunded Booking" :
                "Adjustment"))

            .ForMember(d => d.Type, o => o.MapFrom(s => s.TransactionType.ToString()));

        //part of screen : Subscription Details
        // 1. مابنج الاشتراك (الجزء العلوي)
        CreateMap<Subscription, SubscriptionDetailsDto>()
            .ForMember(dest => dest.SubscriptionId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.GymName, opt => opt.MapFrom(src => src.Branch.BranchName)) // Null check important later
            .ForMember(dest => dest.PlanName, opt => opt.MapFrom(src => src.Plan.Name))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.VisitsRemaining, opt => opt.MapFrom(src => src.VisitsAllowed - src.VisitsUsed));

        // 2. مابنج حركات الدفع (الجدول السفلي)
        CreateMap<UserCreditTransactions, SubscriptionTransactionDto>()
            .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.CreatedAt.ToString("MMM dd, yyyy"))) // Oct 15, 2024
            .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.PaymentAmount)) // العمود الجديد اللي ضفناه
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => "Monthly Renewal")); // وصف ثابت للتجديد


        // ==============================================
        // 🔥🔥🔥 Bookings Mappings 🔥🔥🔥
        // ==============================================

        // 1. Create Booking
        CreateMap<CreateBookingDto, Booking>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => BookingStatus.CONFIRMED));

        // 2. Booking History 
        CreateMap<Booking, BookingHistoryDto>()
            .ForMember(dest => dest.BranchName, opt => opt.MapFrom(src => src.Branch != null ? src.Branch.BranchName : "Unknown Gym"))
            .ForMember(dest => dest.HasReview, opt => opt.MapFrom(src => src.Review != null))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

        // 3. Booking Details (للتفاصيل)
        CreateMap<Booking, BookingDetailsDto>()
            .ForMember(dest => dest.BranchName, opt => opt.MapFrom(src => src.Branch != null ? src.Branch.BranchName : ""))
            .ForMember(dest => dest.BranchAddress, opt => opt.MapFrom(src => src.Branch != null ? src.Branch.Address : "No Address"))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));
    }
}
