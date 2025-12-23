using FitHubBackendAPI.DTOs.Wallet;
using FitHubBackendAPI.ViewModels;

namespace FitHubBackendAPI.Services.Interfaces.UserServices
{
    public interface IUserWalletService
    {
        // 1. دالة عرض الباقات المتاحة (Basic, Premium, Gold)
        // دي اللي الفرونت هيندهها عشان يعرض الكروت
        Task<ResponseViewModel<List<FithubPlanDto>>> GetAllPlansAsync();

        // 2. دالة شراء باقة (بديلة لدالة الشحن القديمة)
        // بتاخد ايدي اليوزر وايدي الباقة وبتحسب الضريبة وتضيف الكريديت
        Task<ResponseViewModel<bool>> PurchasePlanAsync(int userId, int planId);

        // 3. دالة الاستعلام عن الرصيد (زي ما هي)
        Task<ResponseViewModel<WalletBalanceDto>> GetWalletBalanceAsync(int userId);

        // 4. دالة سجل المعاملات (التاريخ)
        // غيرنا الاسم من GetMyTransactionsAsync لـ GetTransactionHistoryAsync عشان يبقى ماشي مع الكونترولر
        Task<ResponseViewModel<List<TransactionHistoryDto>>> GetTransactionHistoryAsync(int userId);
    }
}