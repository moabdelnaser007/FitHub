using FitHubBackendAPI.Entities.Enums;

namespace FitHubBackendAPI.Entities.Models
{
    public class GymStaff : BaseEntity
    {
        public int BranchId { get; set; }

        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Role { get; set; }

        public StaffStatus Status { get; set; } = StaffStatus.ACTIVE;

        public GymBranch? Branch { get; set; }
        public List<Visit> VisitsCheckInHandled { get; set; } = new();
    }
}
