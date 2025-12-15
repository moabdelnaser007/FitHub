using AutoMapper;
using FitHubBackendAPI.DTOs.Bookings;
using FitHubBackendAPI.DTOs.Wallet;
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
        }
    }
}