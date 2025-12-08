using FitHubBackendAPI.Entities.Enums;

namespace FitHubBackendAPI.Entities.Models
{
    public class GymBranch: BaseEntity
    {
        public int OwnerId { get; set; }

        public string BranchName { get; set; } = null!;
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }

        public TimeSpan? OpenTime { get; set; }
        public TimeSpan? CloseTime { get; set; }

        public GymGenderType GenderType { get; set; } = GymGenderType.Mixed;
        public BranchStatus Status { get; set; } = BranchStatus.ACTIVE;


        // Navigation
        public GymOwner Owner { get; set; } = null!;
        public List<GymStaff> Staff { get; set; } = new();
        public List<GymAmenities> Amenities { get; set; } = new();
        public List<GymPlan> Plans { get; set; } = new();
        public List<Subscription> Subscriptions { get; set; } = new();
        public List<Booking> Bookings { get; set; } = new();
        public List<Visit> Visits { get; set; } = new();
        public List<Review> Reviews { get; set; } = new();
    }
}
