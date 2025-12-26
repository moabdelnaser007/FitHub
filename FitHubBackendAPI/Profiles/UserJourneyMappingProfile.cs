using AutoMapper;
using FitHubBackendAPI.DTOs.Bookings;
using FitHubBackendAPI.DTOs.Subscriptions;
using FitHubBackendAPI.DTOs.VisitDTOs;
using FitHubBackendAPI.DTOs.Wallet;
using FitHubBackendAPI.Entities.Enums;
using FitHubBackendAPI.Entities.Models;

namespace FitHubBackendAPI.Profiles
{
    public class UserJourneyMappingProfile : Profile
    {
        public UserJourneyMappingProfile()
        {
            // =========================
            // ✅ User Wallet
            // =========================
            CreateMap<UserWallet, WalletBalanceDto>()
                .ForMember(d => d.UserId, o => o.MapFrom(s => s.UserId ?? 0))
                .ForMember(d => d.Balance, o => o.MapFrom(s => s.Balance ?? 0));

            // =========================
            // ✅ Fithub System Plans
            // =========================
            CreateMap<FithubPlan, FithubPlanDto>();

            // =========================
            // ✅ Wallet Transactions
            // =========================
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
                    !string.IsNullOrEmpty(s.Description)
                        ? s.Description
                        : s.TransactionType == TransactionType.RECHARGE ? "Wallet Recharge"
                        : s.TransactionType == TransactionType.DEDUCT && s.Source == TransactionSource.BOOKING ? "Booking Visit"
                        : s.TransactionType == TransactionType.DEDUCT && s.Source == TransactionSource.SUBSCRIPTION ? "Subscription Purchase"
                        : s.TransactionType == TransactionType.REFUND ? "Refund"
                        : "Adjustment"))
                .ForMember(d => d.Type, o => o.MapFrom(s => s.TransactionType.ToString()));

            // =========================
            // ✅ Subscriptions
            // =========================
            CreateMap<Subscription, SubscriptionDetailsDto>()
                .ForMember(d => d.SubscriptionId, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.BranchName, o => o.MapFrom(s => s.Branch != null ? s.Branch.BranchName : ""))
                .ForMember(d => d.PlanName, o => o.MapFrom(s => s.Plan != null ? s.Plan.Name : ""))
                .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()))
                .ForMember(d => d.RemainingVisits, o => o.MapFrom(s => s.VisitsAllowed - s.VisitsUsed));

            // =========================
            // ✅ Bookings
            // =========================
            CreateMap<CreateBookingDto, Booking>()
                .ForMember(d => d.Status, o => o.MapFrom(_ => BookingStatus.CONFIRMED));

            CreateMap<Booking, BookingHistoryDto>()
                .ForMember(d => d.BranchName, o => o.MapFrom(s => s.Branch != null ? s.Branch.BranchName : ""))
                .ForMember(d => d.HasReview, o => o.MapFrom(s => s.Review != null))
                .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()));

            CreateMap<Booking, BookingDetailsDto>()
                .ForMember(d => d.BranchName, o => o.MapFrom(s => s.Branch != null ? s.Branch.BranchName : ""))
                .ForMember(d => d.BranchAddress, o => o.MapFrom(s => s.Branch != null ? s.Branch.Address : ""))
                .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()));

            // =========================
            // ✅ Visits
            // =========================
            CreateMap<Visit, VisitHistoryDto>()
                .ForMember(d => d.BranchName, o => o.MapFrom(s => s.Branch != null ? s.Branch.BranchName : ""))
                .ForMember(d => d.CheckInTime, o => o.MapFrom(s => s.CheckInTime))
                .ForMember(d => d.CreditsDeducted, o => o.MapFrom(s => s.CreditsDeducted));
        }
    }
}
