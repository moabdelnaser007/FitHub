using FitHubBackendAPI.Entities.Enums;

namespace FitHubBackendAPI.DTOs.Subscriptions
{
    public class SubscriptionForOwnerDto
    {
        public int SubscriptionId { get; set; }
        public string BranchName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string PlanName { get; set; } = string.Empty;
        public SubscriptionStatus Status { get; set; } 

        public int VisitsAllowed { get; set; }
        public int VisitsUsed { get; set; }
        public int VisitsRemaining => VisitsAllowed - VisitsUsed;

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
