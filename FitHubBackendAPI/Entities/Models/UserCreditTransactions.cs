using FitHubBackendAPI.Entities.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace FitHubBackendAPI.Entities.Models
{
    public class UserCreditTransactions : BaseEntity
    {
        public int? UserId { get; set; }

        public int? CreditsBefore { get; set; }
        public int? CreditsChanged { get; set; }
        public int? CreditsAfter { get; set; }

        // ✅ التعديل الجديد: المبلغ المدفوع بالدولار وقت العملية
        [Column(TypeName = "decimal(18,2)")]
        public decimal PaymentAmount { get; set; } = 0;
        public TransactionType TransactionType { get; set; }
        public TransactionSource Source { get; set; }

        public int? ReferenceId { get; set; }

        public User? User { get; set; }
    }
}
