using FitHubBackendAPI.Entities.Enums;

namespace FitHubBackendAPI.Entities
{
    public class Subscription : BaseEntity
    {
        public int SubscriptionId { get; set; }
        public int UserId { get; set; }
        public int PlanId { get; set; }
        public int BranchId { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public int VisitsAllowed { get; set; }
        public int VisitsUsed { get; set; }

        public SubscriptionStatus Status { get; set; } = SubscriptionStatus.ACTIVE;

        public User User { get; set; } = null!;
        public GymPlan Plan { get; set; } = null!;
        public GymBranch Branch { get; set; } = null!;
        public List<Booking> Bookings { get; set; } = new();
    }
}
