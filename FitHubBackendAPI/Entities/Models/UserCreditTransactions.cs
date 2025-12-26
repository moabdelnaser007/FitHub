using FitHubBackendAPI.Entities.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace FitHubBackendAPI.Entities.Models
{
    public class UserCreditTransactions : BaseEntity
    {
        public int? UserId { get; set; }
        public int? BranchId { get; set; }

        public decimal? CreditsBefore { get; set; }
        public decimal? CreditsChanged { get; set; }
        public decimal? CreditsAfter { get; set; }

        public string Description { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal PaymentAmount { get; set; } = 0;
        public TransactionType TransactionType { get; set; }
        public TransactionSource Source { get; set; }

        public int? ReferenceId { get; set; }

        public User? User { get; set; }
    }
}
