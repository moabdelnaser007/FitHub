namespace FitHubBackendAPI.Entities.Models
{
    public class Review : BaseEntity
    {

        public int? UserId { get; set; }
        public int? BranchId { get; set; }
        public int? BookingId { get; set; }

        public int? Rating { get; set; }
        public string? Comment { get; set; }
        public bool? IsAnonymous { get; set; }

        public User? User { get; set; }
        public GymBranch? Branch { get; set; }
        public Booking? Booking { get; set; }
    }
}
