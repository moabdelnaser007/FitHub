using FitHubBackendAPI.DTOs.Wallet;
using FitHubBackendAPI.ViewModels;

namespace FitHubBackendAPI.Services.Interfaces.UserServices
{
    
    public interface IUserWalletService
    {
        // دالة الشحن: بترجع true لو العملية نجحت
        Task<ResponseViewModel<bool>> ChargeWalletAsync(int userId, ChargeWalletDto dto);

        // دالة الاستعلام عن الرصيد: بترجع كلاس الرصيد
        Task<ResponseViewModel<WalletBalanceDto>> GetWalletBalanceAsync(int userId);
    }
}