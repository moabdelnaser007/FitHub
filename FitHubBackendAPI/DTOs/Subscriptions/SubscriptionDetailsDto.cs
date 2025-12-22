namespace FitHubBackendAPI.DTOs.Subscriptions
{
    // ده الكلاس الرئيسي للصفحة كلها
    public class SubscriptionDetailsDto
    {
        // --- 1. بيانات الكارت العلوي (Subscription Info) ---
        public int SubscriptionId { get; set; }
        public string GymName { get; set; } = string.Empty;  // Branch.BranchName
        public string PlanName { get; set; } = string.Empty; // Plan.PlanName
        public string Status { get; set; } = string.Empty;   // Active/Expired

        public int VisitsAllowed { get; set; }
        public int VisitsUsed { get; set; }
        public int VisitsRemaining { get; set; } // حسبة بسيطة

        public DateTime EndDate { get; set; } // عشان نعرض Renews on...

        // --- 2. بيانات الجدول السفلي (Billing History) ---
        public List<SubscriptionTransactionDto> BillingHistory { get; set; } = new();
    }

    public class SubscriptionTransactionDto
    {
        public string Date { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }      // PaymentAmount
        public string Status { get; set; } = "Paid"; // هنثبتها Paid طالما العملية اتسجلت
    }
}

