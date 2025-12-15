using FitHubBackendAPI.Entities.Enums;

namespace FitHubBackendAPI.Entities.Models
{
    public class GymPlan : BaseEntity
    {
        public int BranchId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal? Price { get; set; }
        public int? CreditsCost { get; set; }
        public int? VisitsLimit { get; set; }
        public int? DurationDays { get; set; }

        public PlanStatus Status { get; set; } = PlanStatus.ACTIVE;

        public GymBranch? Branch { get; set; } = null!;
        public List<Subscription>? Subscriptions { get; set; } = new();
        public List<Booking>? Bookings { get; set; } = new();
    }
}
