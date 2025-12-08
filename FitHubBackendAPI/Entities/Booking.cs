using FitHubBackendAPI.Entities.Enums;

namespace FitHubBackendAPI.Entities
{
    public class Booking : BaseEntity
    {

        public string BookingCode { get; set; } = null!;
        public int UserId { get; set; }
        public int BranchId { get; set; }
        public int? PlanId { get; set; }
        public int? SubscriptionId { get; set; }

        public DateTime ScheduledDateTime { get; set; }
        public int CreditsCost { get; set; }

        public BookingStatus Status { get; set; } = BookingStatus.CONFIRMED;

        public User User { get; set; } = null!;
        public GymBranch Branch { get; set; } = null!;
        public GymPlan? Plan { get; set; }
        public Subscription? Subscription { get; set; }
        public Visit? VisitRecord { get; set; }
        public Review? Review { get; set; }
    }
}
