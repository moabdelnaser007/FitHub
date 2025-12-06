using FitHubBackendAPI.Entities.Enums;

namespace FitHubBackendAPI.Entities
{
    public class UserCreditTransactions : BaseEntity
    {
        public int TransactionId { get; set; }
        public int UserId { get; set; }

        public int CreditsBefore { get; set; }
        public int CreditsChanged { get; set; }
        public int CreditsAfter { get; set; }

        public TransactionType TransactionType { get; set; }
        public TransactionSource Source { get; set; }

        public int? ReferenceId { get; set; }
        public DateTime CreatedAt { get; set; }

        public User User { get; set; } = null!;
    }
}
