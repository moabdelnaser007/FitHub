using FitHubBackendAPI.Entities.Enums;

namespace FitHubBackendAPI.DTOs.Subscriptions
{
    public class SubscriptionListDto
    {
        public int SubscriptionId { get; set; }
        public string BranchName { get; set; } = string.Empty;
        public string PlanName { get; set; } = string.Empty;
        public int RemainingVisits { get; set; }
        public SubscriptionStatus Status { get; set; } = SubscriptionStatus.ACTIVE;
        public DateTime EndDate { get; set; }
    }
}
