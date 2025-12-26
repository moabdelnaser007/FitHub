using FitHubBackendAPI.Entities.Enums;

namespace FitHubBackendAPI.DTOs.OwnerWallet
{
    public class OwnerTransactionDto
    {
        public int Id { get; set; }
        public string Date { get; set; }
        public decimal Amount { get; set; }
        public TransactionSource Source { get; set; }   // VISIT / SUBSCRIPTION
        public string Description { get; set; }
    }
}
