using FitHubBackendAPI.DTOs.OwnerWallet;
using FitHubBackendAPI.ViewModels;

namespace FitHubBackendAPI.Services.Interfaces.OwnerServices
{
    public interface IOwnerWalletService
    {
        Task<ResponseViewModel<OwnerWalletBalanceDto>> GetBalanceAsync(int ownerUserId);
        Task<ResponseViewModel<List<OwnerTransactionDto>>> GetTransactionsAsync(int ownerUserId);
        Task<ResponseViewModel<BranchRevenueDto>> GetBranchRevenueAsync(int ownerUserId, int branchId);
    }
}
