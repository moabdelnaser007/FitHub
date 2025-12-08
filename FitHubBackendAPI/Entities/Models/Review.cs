namespace FitHubBackendAPI.Entities.Models
{
    public class Review : BaseEntity
    {

        public int UserId { get; set; }
        public int BranchId { get; set; }
        public int BookingId { get; set; }

        public int Rating { get; set; }
        public string? Comment { get; set; }
        public bool IsAnonymous { get; set; }

        public User User { get; set; } = null!;
        public GymBranch Branch { get; set; } = null!;
        public Booking Booking { get; set; } = null!;
    }
}
