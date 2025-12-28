using FitHubBackendAPI.DTOs.UserDTOs;
using FitHubBackendAPI.Entities.Models;

namespace FitHubBackendAPI.Services.Interfaces.PaymobService
{
    public interface IPaymobService
    {
        Task<(string redirectionURL, UserCreditTransactions transcation)> CreatePaymentAsync(int userId, int planID,int TransactionId);
        string ComputeHmacSHA512(string data, string secret);
        Task<UserCreditTransactions> PaymentSuccess(int specialReference);
        Task<UserCreditTransactions> PaymentFailed(int specialReference);
    }

}
