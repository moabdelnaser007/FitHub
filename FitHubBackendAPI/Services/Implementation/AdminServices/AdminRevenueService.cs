using FitHubBackendAPI.Entities.Enums;
using FitHubBackendAPI.Entities.Models;
using FitHubBackendAPI.Repository.Interfaces;
using FitHubBackendAPI.Services.Interfaces.AdminServices;
using FitHubBackendAPI.ViewModels;

namespace FitHubBackendAPI.Services.Implementation.AdminServices
{
    public class AdminRevenueService:IAdminRevenue
    {
        private readonly IGenericRepository<GymOwner> _ownerRepo;
        private readonly IGenericRepository<UserCreditTransactions> _transactionRepo;
        public AdminRevenueService(IGenericRepository<GymOwner> ownerRepo,
                     IGenericRepository<UserCreditTransactions> transactionRepo) 
        {
            _ownerRepo = ownerRepo;
            _transactionRepo = transactionRepo;
        }

        public async Task<ResponseViewModel<decimal>> GetAdminAllRevenue(
                    DateTime? startFrom,
                    DateTime? endWith)
        {
            var paymentTrans = await _transactionRepo.GetAsync(
                t => t.IsPaid &&
                     t.Source == TransactionSource.PAYMOB &&
                     (!startFrom.HasValue || t.CreatedAt >= startFrom.Value) &&
                     (!endWith.HasValue || t.CreatedAt <= endWith.Value)
            );
            var DeductTrans = await _transactionRepo.GetAsync(
                t=>t.TransactionType==TransactionType.DEDUCT &&
                     (!startFrom.HasValue || t.CreatedAt >= startFrom.Value) &&
                     (!endWith.HasValue || t.CreatedAt <= endWith.Value)
            );


            decimal Ptrans = paymentTrans.Sum(t => t.CreditsChanged)??0;
            decimal Dtrans = DeductTrans.Sum(t => t.CreditsChanged)??0;
           
            decimal revenue = Ptrans * 1.5m + Dtrans * 0.9m;

            return ResponseViewModel<decimal>.Success(revenue,$"success");
        }


    }
}
