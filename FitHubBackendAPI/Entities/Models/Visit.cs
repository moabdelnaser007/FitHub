using FitHubBackendAPI.Entities.Enums;

namespace FitHubBackendAPI.Entities.Models
{
    public class Visit : BaseEntity
    {

        public int? BookingId { get; set; }
        public int? UserId { get; set; }
        public int? BranchId { get; set; }
        public int? StaffId { get; set; }

        public DateTime? CheckInTime { get; set; }
        public DateTime? CheckOutAt { get; set; }

        public decimal? CreditsDeducted { get; set; }

        public VisitStatus Status { get; set; } = VisitStatus.CHECKED_IN;

        public Booking? Booking { get; set; } = null!;
        public User? User { get; set; } = null!;
        public GymBranch? Branch { get; set; } = null!;
        public GymStaff? Staff { get; set; } = null!;
    }
}
