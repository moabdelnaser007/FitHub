using AutoMapper;
using FitHubBackendAPI.Entities.Models;
using FitHubBackendAPI.DTOs.Wallet;

namespace FitHubBackendAPI.Profiles 
{
    public class UserJourneyMappingProfile : Profile
    {
        public UserJourneyMappingProfile()
        {
            // ✅ السطر ده هو اللي هيخلي دالة GetBalance تشتغل
            CreateMap<UserWallet, WalletBalanceDto>();
        }
    }
}