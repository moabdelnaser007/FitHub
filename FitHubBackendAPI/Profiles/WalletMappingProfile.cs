using AutoMapper;
using FitHubBackendAPI.DTOs.Wallet;
using FitHubBackendAPI.Entities.Enums;
using FitHubBackendAPI.Entities.Models;

namespace FitHubBackendAPI.Profiles
{
    public class WalletMappingProfile : Profile
    {
        public WalletMappingProfile()
        {
            CreateMap<UserWallet, WalletBalanceDto>()
                .ForMember(d => d.UserId, o => o.MapFrom(s => s.UserId))
                .ForMember(d => d.Balance, o => o.MapFrom(s => s.Balance ?? 0));

            CreateMap<UserCreditTransactions, TransactionHistoryDto>()
                .ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.Date, o => o.MapFrom(s => s.CreatedAt.ToString("dd MMM yyyy")))
                .ForMember(d => d.AmountPaid, o => o.MapFrom(s => s.PaymentAmount))
                .ForMember(d => d.Credits, o => o.MapFrom(s =>
                    s.TransactionType == TransactionType.DEDUCT
                        ? -s.CreditsChanged
                        : s.CreditsChanged))
                .ForMember(d => d.Type, o => o.MapFrom(s => s.TransactionType.ToString()))
                .ForMember(d => d.IsPositive, o => o.MapFrom(s =>
                    s.TransactionType == TransactionType.RECHARGE ||
                    s.TransactionType == TransactionType.REFUND));
        }
    }
}
