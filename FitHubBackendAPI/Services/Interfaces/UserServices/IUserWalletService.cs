using FitHubBackendAPI.DTOs.Wallet;
using FitHubBackendAPI.Entities.Models;
using FitHubBackendAPI.ViewModels;

namespace FitHubBackendAPI.Services.Interfaces.UserServices
{
    public interface IUserWalletService
    {
        Task<ResponseViewModel<WalletBalanceDto>> GetBalanceAsync(int userId);
        Task<ResponseViewModel<List<TransactionHistoryDto>>> GetTransactionsAsync(int userId);
        Task<ResponseViewModel<UserCreditTransactions>> RechargeAsync(int userId, RechargeWalletDto dto);
        Task<ResponseViewModel<bool>> RefundBookingAsync(int userId, int bookingId);
        Task<ResponseViewModel<UserCreditTransactions>> UpdateWallet(UserCreditTransactions transaction);


    }
}