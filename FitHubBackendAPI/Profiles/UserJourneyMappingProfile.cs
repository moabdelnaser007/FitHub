using AutoMapper;
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
            .ForMember(d => d.Description, o => o.MapFrom(s =>
                s.TransactionType == TransactionType.RECHARGE ? "Wallet Top-up" :
                s.TransactionType == TransactionType.DEDUCT && s.Source == TransactionSource.BOOKING ? "Class Booking" :
                s.TransactionType == TransactionType.DEDUCT && s.Source == TransactionSource.SUBSCRIPTION ? "Subscription Payment" :
                s.TransactionType == TransactionType.REFUND ? "Refunded Booking" :
                "Adjustment"))
            .ForMember(d => d.Type, o => o.MapFrom(s => s.TransactionType.ToString()));
    }
}
