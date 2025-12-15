using FitHubBackendAPI.Entities.Enums;

namespace FitHubBackendAPI.Entities.Models
{
    public class GymStaff : BaseEntity
    {
        public int UserId { get; set; }
        public int BranchId { get; set; }

        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Role { get; set; }

        public StaffStatus Status { get; set; } = StaffStatus.ACTIVE;

        // Navigation
        public User User { get; set; } 
        public GymBranch Branch { get; set; } = null!;
        public List<Visit> VisitsCheckInHandled { get; set; } = new();
    }
}
