using AutoMapper;
using FitHubBackendAPI.DTOs.Bookings;
using FitHubBackendAPI.DTOs.Wallet;
using FitHubBackendAPI.Entities.Enums;
using FitHubBackendAPI.Entities.Models;

namespace FitHubBackendAPI.Profiles 
{
    public class UserJourneyMappingProfile : Profile
    {
        public UserJourneyMappingProfile()
        {
            // ✅ السطر ده هو اللي هيخلي دالة GetBalance تشتغل
            CreateMap<UserWallet, WalletBalanceDto>();

            // Booking Mappings
            CreateMap<CreateBookingDto, Booking>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => FitHubBackendAPI.Entities.Enums.BookingStatus.CONFIRMED)) // الحالة ديفولت "مؤكد"
                .ForMember(dest => dest.BookingCode, opt => opt.Ignore()) // الكود: إحنا اللي هنعمله في السيرفيس
                .ForMember(dest => dest.CreditsCost, opt => opt.Ignore()) // التكلفة: إحنا اللي هنحسبها في السيرفيس
                .ForMember(dest => dest.UserId, opt => opt.Ignore());     // اليوزر: هنجيبه من التوكن

            // Booking History Mapping
            CreateMap<Booking, BookingHistoryDto>()
                .ForMember(dest => dest.BranchName, opt => opt.MapFrom(src => src.Branch.BranchName)) // بنجيب اسم الفرع
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString())) // بنحول الـ Enum لنص
                .ForMember(dest => dest.HasReview, opt => opt.MapFrom(src => src.Review != null)); // لو في ريفيو يبقى true

            // Booking Details Mapping
            CreateMap<Booking, BookingDetailsDto>()
                .ForMember(dest => dest.BranchName, opt => opt.MapFrom(src => src.Branch.BranchName))
                // ✅  دمجنا العنوان والمدينة عشان العنوان يبقى كامل
                .ForMember(dest => dest.BranchAddress, opt => opt.MapFrom(src => $"{src.Branch.Address}, {src.Branch.City}"))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));



            // Transaction History Mapping
            CreateMap<UserCreditTransactions, TransactionHistoryDto>()
                // 1. التاريخ: بنحوله لنص شيك زي الصورة (Oct 15, 2023)
                .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.CreatedAt.ToString("MMM dd, yyyy")))

                // 2. الفلوس: بنجيبها من العمود الجديد اللي ضفناه
                .ForMember(dest => dest.AmountPaid, opt => opt.MapFrom(src => src.PaymentAmount))

                // 3. النقط: لو خصم (DEDUCT) بنحط سالب، غير كده موجب
                .ForMember(dest => dest.Credits, opt => opt.MapFrom(src =>
                    src.TransactionType == TransactionType.DEDUCT ? -src.CreditsChanged : src.CreditsChanged))

                // 4. الإشارة (للألوان): الشحن والاسترجاع موجب (أخضر)، الخصم سالب (أحمر)
                .ForMember(dest => dest.IsPositive, opt => opt.MapFrom(src =>
                    src.TransactionType == TransactionType.RECHARGE ||
                    src.TransactionType == TransactionType.REFUND ||
                    (src.TransactionType == TransactionType.ADMIN_ADJUST && src.CreditsChanged > 0)))

                // 5. الوصف: بنكتب وصف مفهوم لليوزر بدل كلام الداتا بيز الناشف
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src =>
                    src.TransactionType == TransactionType.RECHARGE ? "Wallet Top-up" :
                    src.TransactionType == TransactionType.DEDUCT && src.Source == TransactionSource.BOOKING ? "Class Booking" :
                    src.TransactionType == TransactionType.DEDUCT && src.Source == TransactionSource.SUBSCRIPTION ? "Subscription Payment" :
                    src.TransactionType == TransactionType.REFUND ? "Refunded Booking" :
                    "Adjustment"));
        }
    }
}