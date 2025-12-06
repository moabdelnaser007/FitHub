using FitHubBackendAPI.Entities.Enums;

namespace FitHubBackendAPI.Entities
{
    public class GymStaff : BaseEntity
    {
        public int StaffId { get; set; }
        public int BranchId { get; set; }

        public string FullName { get; set; } = null!;
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Role { get; set; }

        public StaffStatus Status { get; set; } = StaffStatus.ACTIVE;
        public DateTime CreatedAt { get; set; }

        public GymBranch Branch { get; set; } = null!;
        public List<Visit> VisitsCheckInHandled { get; set; } = new();
    }
}
