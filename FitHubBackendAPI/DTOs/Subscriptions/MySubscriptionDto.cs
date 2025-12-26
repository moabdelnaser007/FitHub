using FitHubBackendAPI.Entities.Enums;

namespace FitHubBackendAPI.DTOs.Subscriptions
{
    public class MySubscriptionDto
    {
        public int Id { get; set; } // Subscription Id
        public string GymName { get; set; } = string.Empty; // اسم الفرع
        public string PlanName { get; set; } = string.Empty; // اسم الباقة

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public int VisitsAllowed { get; set; }
        public int VisitsUsed { get; set; }
        public int VisitsRemaining => VisitsAllowed - VisitsUsed; // حسبة بسيطة للعرض

        public SubscriptionStatus Status { get; set; }
    }
}
