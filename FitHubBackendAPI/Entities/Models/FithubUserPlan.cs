using System.ComponentModel.DataAnnotations.Schema;

namespace FitHubBackendAPI.Entities.Models
{
    public class FithubUserPlan: BaseEntity
    {
        [ForeignKey("User")]
        public int UserId { get; set; }

        [ForeignKey("FithubPlan")]
        public int PlanId { get; set; }

        // 🟢 بيانات الفاتورة (Snapshot) 
        // بنسجلها هنا عشان لو سعر الباقة اتغير مستقبلاً، السجل القديم يفضل ثابت
        public decimal BasePrice { get; set; }   // السعر الأصلي
        public decimal TaxAmount { get; set; }   // قيمة الضريبة (15%)
        public decimal TotalAmount { get; set; } // المبلغ الكلي المدفوع

        public DateTime PurchaseDate { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public User? User { get; set; }
        public FithubPlan? FithubPlan { get; set; }
    }
}
