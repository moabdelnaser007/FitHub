using System.ComponentModel.DataAnnotations.Schema;

namespace FitHubBackendAPI.DTOs.AdminDtos
{
    public class AdminFitHubPlanDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }


        public int PlanId { get; set; }

        public decimal BasePrice { get; set; }   // السعر الأصلي
        public decimal TaxAmount { get; set; }   // قيمة الضريبة (15%)
        public decimal TotalAmount { get; set; } // المبلغ الكلي المدفوع

        public DateTime PurchaseDate { get; set; } = DateTime.UtcNow;
    }
}
