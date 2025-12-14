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

        }
    }
}