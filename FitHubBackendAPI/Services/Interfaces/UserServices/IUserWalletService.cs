using FitHubBackendAPI.DTOs.Wallet;
using FitHubBackendAPI.ViewModels;

namespace FitHubBackendAPI.Services.Interfaces.UserServices
{
    public interface IUserWalletService
    {
        Task<ResponseViewModel<WalletBalanceDto>> GetBalanceAsync(int userId);
        Task<ResponseViewModel<List<TransactionHistoryDto>>> GetTransactionsAsync(int userId);
        Task<ResponseViewModel<bool>> RechargeAsync(int userId, RechargeWalletDto dto);
        Task<ResponseViewModel<bool>> RefundBookingAsync(int userId, int bookingId);
    }
}