using FitHubBackendAPI.Entities.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using System.Transactions;

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

        public int? ReferenceId { get; set; }          // Paymob order_id
        public string? PaymentKey { get; set; }        // Paymob payment token
        public bool IsPaid { get; set; }= false;               
        public _TransactionStatus Status { get; set; } = _TransactionStatus.PENDING;
        public DateTime? PaidAt { get; set; }

        public User? User { get; set; }
    }
}
